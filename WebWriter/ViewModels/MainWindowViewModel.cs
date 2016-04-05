//
//	Last mod:	18 July 2015 12:47:15
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
			TitlesCommand = new Command<object>(OnTitlesExecute);
			GalleryCommand = new Command<object>(OnGalleryExecute);
			SettingsCommand = new Command(OnSettingsCommandExecute);
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
		public Command<object> TitlesCommand { get; private set; }

		/// <summary>
		/// Method to invoke when the TitlesCommand command is executed.
		/// </summary>
		private void OnTitlesExecute(object parameter)
			{
			// TODO: Handle command logic here
			}

		/// <summary>
		/// Gets the GalleryCommand command.
		/// </summary>
		public Command<object> GalleryCommand { get; private set; }

		/// <summary>
		/// Method to invoke when the GalleryCommand command is executed.
		/// </summary>
		private void OnGalleryExecute(object parameter)
			{
			GalleryViewModel gvm = TypeFactory.Default.CreateInstanceWithParametersAndAutoCompletion<GalleryViewModel>();
			uiVisualiserService.ShowDialog(gvm);
			}

		/// <summary>
		/// Gets the SettingsCommand command.
		/// </summary>
		public Command SettingsCommand { get; private set; }

		/// <summary>
		/// Method to invoke when the SettingsCommand command is executed.
		/// </summary>
		private void OnSettingsCommandExecute()
			{
			// TODO: Handle command logic here
			}

		/// <summary>
		/// Method to invoke when the Settings command is executed.
		/// </summary>
		private void OnSettingsExecute()
			{
			// TODO: Handle command logic here
			}

		protected override async Task Initialize()
			{
			await base.Initialize();

			// TODO: subscribe to events here
			}

		protected override async Task Close()
			{
			// TODO: unsubscribe from events here

			await base.Close();
			}
		}
	}
