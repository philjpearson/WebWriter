//
//	Last mod:	12 October 2023 10:35:48
//
namespace WebWriter.ViewModels
	{
	using System;
	using System.IO;
	using System.Threading.Tasks;
	using System.Windows;
	using Catel;
	using Catel.IoC;
	using Catel.MVVM;
	using Catel.Services;
	using WebWriter.Models;
	using WebWriter.Workers;

	public class MainWindowViewModel : ViewModelBase
		{
		private readonly IUIVisualizerService uiVisualiserService;

		public MainWindowViewModel(IUIVisualizerService uiVisualiserService)
			{
			Argument.IsNotNull(() => uiVisualiserService);
			this.uiVisualiserService = uiVisualiserService;

			ExitCommand = new Command<object>(OnExitExecute);
			TitlesCommand = new TaskCommand<object>(OnTitlesExecute);
			GalleryCommand = new TaskCommand<object>(OnGalleryExecute);
			SettingsCommand = new TaskCommand(OnSettingsExecuteAsync);
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
		public TaskCommand SettingsCommand { get; private set; }

		/// <summary>
		/// Method to invoke when the Settings command is executed.
		/// </summary>
		private async Task OnSettingsExecuteAsync()
			{
			var ant = new AntiHacker();
			await ant.CheckDirectoriesAsync();
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
				return recordingsCommand ??= new TaskCommand<object>(RecordingsCommand_Execute);
				}
			}

		private TaskCommand<object> recordingsCommand;

		/// <summary>
		/// Method to invoke when the RecordingsCommand command is executed.
		/// </summary>
		/// <param name="parameter">The parameter of the command.</param>
		private async Task<bool?> RecordingsCommand_Execute(object parameter)
			{
			RecordingsViewModel vm = TypeFactory.Default.CreateInstanceWithParametersAndAutoCompletion<RecordingsViewModel>();
			return await uiVisualiserService.ShowDialogAsync(vm);
			}

		/// <summary>
		/// Gets the SundayCommand command.
		/// </summary>
		public TaskCommand<object> SundayCommand
			{
			get
				{
				return sundayCommand ??= new TaskCommand<object>(SundayCommand_Execute);
				}
			}

		private TaskCommand<object> sundayCommand;

		/// <summary>
		/// Method to invoke when the RundayCommand command is executed.
		/// </summary>
		/// <param name="parameter">The parameter of the command.</param>
		private async Task<bool?> SundayCommand_Execute(object parameter)
			{
			var vm = TypeFactory.Default.CreateInstanceWithParametersAndAutoCompletion<SundaysViewModel>();
			return await uiVisualiserService.ShowDialogAsync(vm);
			}

		/// <summary>
		/// Gets the SundayCommand command.
		/// </summary>
		public TaskCommand<object> BibleClassCommand
			{
			get
				{
				return bibleCassCommand ??= new TaskCommand<object>(BibleClassCommand_Execute);
				}
			}

		private TaskCommand<object> bibleCassCommand;

		/// <summary>
		/// Method to invoke when the RundayCommand command is executed.
		/// </summary>
		/// <param name="parameter">The parameter of the command.</param>
		private async Task<bool?> BibleClassCommand_Execute(object parameter)
			{
			var vm = TypeFactory.Default.CreateInstanceWithParametersAndAutoCompletion<BibleClassViewModel>();
			return await uiVisualiserService.ShowDialogAsync(vm);
			}

		/// <summary>
		/// Gets the LockdownProgrammeCommand command.
		/// </summary>
		public TaskCommand<object> LockdownProgrammeCommand
			{
			get
				{
				return lockdownProgrammeCommand ??= new TaskCommand<object>(LockdownProgrammeCommand_Execute);
				}
			}

		private TaskCommand<object> lockdownProgrammeCommand;

		/// <summary>
		/// Method to invoke when the RecordingsCommand command is executed.
		/// </summary>
		/// <param name="parameter">The parameter of the command.</param>
		private async Task<bool?> LockdownProgrammeCommand_Execute(object parameter)
			{
			bool reportedError = false;

			var filePath = @"C:\Users\Phil\OneDrive\My Documents\Ecclesia\Programme\LockdownProgramme.csv";

			var result = Uploader.Upload(filePath, "private/programme/LockdownProgramme.csv");

			if (result)
				{
				try
					{
					var prog = LockdownProgramme.Load(filePath);
					var sundays = prog.Sunday;
					var bibleClass = prog.BibleClass;

					filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Ecclesia\Programme\Ecclesial programme.pdf");
					result = prog.CreatePdf(filePath);
					if (result)
						{
						result = Uploader.Upload(filePath, "programme/Ecclesial programme.pdf", true);
						}
					}
				catch (Exception ex)
					{
					MessageBox.Show($"Error creating PDF: {ex.Message}", Application.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);
					reportedError = true;
					}
				}

			if (result)
				MessageBox.Show("Programme update uploaded successfully", Application.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Information);
			else if (!reportedError)
				MessageBox.Show("Oops! Something went wrong", Application.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);
			return await Task.FromResult(result);
			}

		protected override Task CloseAsync()
			{
			return base.CloseAsync();
			}
		}
	}
