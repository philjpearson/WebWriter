//
//	Last mod:	30 December 2016 20:09:50
//
namespace WebWriter.ViewModels
	{
	using System.Threading.Tasks;
	using System.Windows;
	using Catel;
	using Catel.IoC;
	using Catel.MVVM;
	using Catel.Services;

	public class MainWindowViewModel : ViewModelBase
		{
		private IUIVisualizerService uiVisualiserService;

		public MainWindowViewModel(IUIVisualizerService uiVisualiserService)
			{
			Argument.IsNotNull(() => uiVisualiserService);
			this.uiVisualiserService = uiVisualiserService;

			ExitCommand = new Command<object>(OnExitExecute);
			TitlesCommand = new TaskCommand<object>(OnTitlesExecute);
			GalleryCommand = new TaskCommand<object>(OnGalleryExecute);
			SettingsCommand = new Command(OnSettingsExecute);
			CampaignGalleryCommand = new TaskCommand<object>(OnCampaignGalleryExecute);
			}

		public override string Title { get { return "Phil's Web Writer"; } }

		// TODO: Register models with the vmpropmodel codesnippet
		// TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets

		// TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets
		/// <summary>
		/// Gets the ExitCommand command.
		/// </summary>
		public Command<object> ExitCommand { get; private set; }

		/// <summary>
		/// Method to invoke when the ExitCommand command is executed.
		/// </summary>
		private void OnExitExecute(object parameter)
			{
			Application.Current.Shutdown();
			}

		/// <summary>
		/// Gets the TitlesCommand command.
		/// </summary>
		public TaskCommand<object> TitlesCommand { get; private set; }

		/// <summary>
		/// Method to invoke when the TitlesCommand command is executed.
		/// </summary>
		private async Task<bool?> OnTitlesExecute(object parameter)
			{
			TitlesViewModel vm = TypeFactory.Default.CreateInstanceWithParametersAndAutoCompletion<TitlesViewModel>();
			return await uiVisualiserService.ShowDialogAsync(vm);
			}

		/// <summary>
		/// Gets the GalleryCommand command.
		/// </summary>
		public TaskCommand<object> GalleryCommand { get; private set; }

		/// <summary>
		/// Method to invoke when the GalleryCommand command is executed.
		/// </summary>
		private async Task<bool?> OnGalleryExecute(object parameter)
			{
			GalleryViewModel gvm = TypeFactory.Default.CreateInstanceWithParametersAndAutoCompletion<GalleryViewModel>();
			return await uiVisualiserService.ShowDialogAsync(gvm);
			}

		/// <summary>
		/// Gets the SettingsCommand command.
		/// </summary>
		public Command SettingsCommand { get; private set; }

		/// <summary>
		/// Method to invoke when the Settings command is executed.
		/// </summary>
		private void OnSettingsExecute()
			{
			// TODO: Handle command logic here
			}

		/// <summary>
		/// Gets the CampaignGalleryCommand command.
		/// </summary>
		public TaskCommand<object> CampaignGalleryCommand { get; private set; }

		/// <summary>
		/// Method to invoke when the CampaignGalleryCommand command is executed.
		/// </summary>
		private async Task<bool?> OnCampaignGalleryExecute(object parameter)
			{
			CampaignGalleryViewModel cgvm = TypeFactory.Default.CreateInstanceWithParametersAndAutoCompletion<CampaignGalleryViewModel>();
			return await uiVisualiserService.ShowDialogAsync(cgvm);
			}

		/// <summary>
		/// Gets the RecordingsCommand command.
		/// </summary>
		public TaskCommand<object> RecordingsCommand
			{
			get
				{
				if (_RecordingsCommand == null)
					_RecordingsCommand = new TaskCommand<object>(RecordingsCommand_Execute);
				return _RecordingsCommand;
				}
			}

		private TaskCommand<object> _RecordingsCommand;

		/// <summary>
		/// Method to invoke when the RecordingsCommand command is executed.
		/// </summary>
		/// <param name="parameter">The parameter of the command.</param>
		private async Task<bool?> RecordingsCommand_Execute(object parameter)
			{
			RecordingsViewModel vm = TypeFactory.Default.CreateInstanceWithParametersAndAutoCompletion<RecordingsViewModel>();
			return await uiVisualiserService.ShowDialogAsync(vm);
			}

		protected override Task CloseAsync()
			{
			return base.CloseAsync();
			}
		}
	}
