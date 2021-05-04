//
//	Last mod:	04 May 2021 15:40:43
//
namespace WebWriter.ViewModels
	{
	using Catel.Data;
	using Catel.MVVM;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Runtime.InteropServices;
	using System.Threading.Tasks;
	using System.Windows;
	using WebWriter.Workers;

	public class DodgyStuffViewModel : ViewModelBase
		{
		ChangeNotificationWrapper wrapper;

		public DodgyStuffViewModel(List<string> dodgyStuff)
			{
			DodgyStuff = new ObservableCollection<string>(dodgyStuff);
			SelectedThings = new ObservableCollection<string>();
			}

		public override string Title { get { return "Dodgy stuff"; } }

		// TODO: Register models with the vmpropmodel codesnippet
		[Model]
		public ObservableCollection<string> DodgyStuff { get; set; }

		// TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
		public ObservableCollection<string> SelectedThings { get; set; }

		private void OnSelectedThingsChanged()
			{
			if (!(wrapper is null))
				{
				wrapper.CollectionChanged -= Wrapper_CollectionChanged;
				}
			if (!(SelectedThings is null))
				{
				wrapper = new ChangeNotificationWrapper(SelectedThings);
				wrapper.CollectionChanged += Wrapper_CollectionChanged;
				}
			}

		private void Wrapper_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
			{
			DeleteCommand.RaiseCanExecuteChanged();
			MakeLegitimateCommand.RaiseCanExecuteChanged();
			}

		// TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets
		private Command deleteCommand;

		public Command DeleteCommand
			{
			get
				{
				return deleteCommand ??= new Command(DeleteCommandExecute, DeleteCommandCanExecute);
				}
			}

		private void DeleteCommandExecute()
			{
			if (MessageBox.Show("Are you sure you want to delete all the selected files from the web site?", Title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
				var toRemove = new List<string>();
				foreach (string thing in SelectedThings)
					{
					if (AntiHacker.Delete(thing))
						toRemove.Add(thing);
					else
						MessageBox.Show($"Sorry! I can't delete '{thing}'. You will have to do it yourself.", "WebWriter");
					}
				foreach (var item in toRemove)
					{
					DodgyStuff.Remove(item);
					}
				}
			}

		private bool DeleteCommandCanExecute()
			{
			return SelectedThings?.Count > 0;
			}

		private Command makeLegitimateCommand;

		public Command MakeLegitimateCommand
			{
			get
				{
				return makeLegitimateCommand ??= new Command(MakeLegitimateCommandExecute, MakeLegitimateCommandCanExecute);
				}
			}

		private void MakeLegitimateCommandExecute()
			{
			var toRemove = new List<string>();
			foreach (string thing in SelectedThings)
				{
				AntiHacker.MakeLegitimate(thing);
				toRemove.Add(thing);
				}
			foreach (var item in toRemove)
				{
				DodgyStuff.Remove(item);
				}
			}

		private bool MakeLegitimateCommandCanExecute()
			{
			return SelectedThings?.Count > 0;
			}

		protected override async Task InitializeAsync()
			{
			await base.InitializeAsync();

			// TODO: subscribe to events here
			}

		protected override async Task CloseAsync()
			{
			// TODO: unsubscribe from events here

			await base.CloseAsync();
			}
		}
	}
