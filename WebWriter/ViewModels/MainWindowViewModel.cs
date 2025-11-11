//
//	Last mod:	11 November 2025 16:55:12
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
	using RWS.UIClasses;
	using WebWriter.Email;
	using WebWriter.Models;

	public class MainWindowViewModel : ViewModelBase
		{
		private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

		private readonly IUIVisualizerService uiVisualiserService;

		public MainWindowViewModel(IUIVisualizerService uiVisualiserService)
			{
			logger.Info("Starting Version {0}", AssemblyInfo.AssemblyFileVersion);

			Argument.IsNotNull(() => uiVisualiserService);
			this.uiVisualiserService = uiVisualiserService;

			ExitCommand = new Command<object?>(OnExitExecute);
			TitlesCommand = new TaskCommand<object?>(OnTitlesExecute);
			GalleryCommand = new TaskCommand<object?>(OnGalleryExecute);
			CampaignGalleryCommand = new TaskCommand<object?>(OnCampaignGalleryExecute);
			}

		public override string Title { get { return "Phil's Web Writer"; } }

		public bool UserIsEntitled { get { return Environment.UserName == "Phil"; } }

		/// <summary>
		/// Gets the EnableEditingCommand command.
		/// </summary>
		public Command<object?> EnableEditingCommand => enableEditingCommand ??= new Command<object?>(EnableEditingCommand_Execute);

		private Command<object?>? enableEditingCommand;

		/// <summary>
		/// Method to invoke when the EnableEditingCommand command is executed.
		/// </summary>
		/// <param name="parameter">The parameter of the command.</param>
		private void EnableEditingCommand_Execute(object? parameter)
			{
			if (UserIsEntitled)
				{
				StaticProperties.EditingEnabled = !StaticProperties.EditingEnabled;
				LockdownProgrammeCommand.RaiseCanExecuteChanged();
				SettingsCommand.RaiseCanExecuteChanged();
				}
			}

		/// <summary>
		/// Gets the ExitCommand command.
		/// </summary>
		public Command<object?> ExitCommand { get; private set; }

		/// <summary>
		/// Method to invoke when the ExitCommand command is executed.
		/// </summary>
		private void OnExitExecute(object? parameter)
			{
			Application.Current.Shutdown();
			}

		/// <summary>
		/// Gets the TitlesCommand command.
		/// </summary>
		public TaskCommand<object?> TitlesCommand { get; private set; }

		/// <summary>
		/// Method to invoke when the TitlesCommand command is executed.
		/// </summary>
		private async Task<bool?> OnTitlesExecute(object? parameter)
			{
			var vm = TypeFactory.Default.CreateInstanceWithParametersAndAutoCompletion<TitlesViewModel>();
			return (await uiVisualiserService.ShowDialogAsync(vm!)).DialogResult;
			}

		/// <summary>
		/// Gets the GalleryCommand command.
		/// </summary>
		public TaskCommand<object?> GalleryCommand { get; private set; }

		/// <summary>
		/// Method to invoke when the GalleryCommand command is executed.
		/// </summary>
		private async Task<bool?> OnGalleryExecute(object? parameter)
			{
			var gvm = TypeFactory.Default.CreateInstanceWithParametersAndAutoCompletion<GalleryViewModel>();
			return (await uiVisualiserService.ShowDialogAsync(gvm!)).DialogResult;
			}

		/// <summary>
		/// Gets the SettingsCommand command.
		/// </summary>
		public TaskCommand SettingsCommand => settingsCommand ??= new TaskCommand(SettingsCommandExecuteAsync, SettingsCommandCanExecute);

		private TaskCommand? settingsCommand;

		private bool SettingsCommandCanExecute()
			{
			return StaticProperties.EditingEnabled;
			}

		private async Task SettingsCommandExecuteAsync()
			{
			//var ant = new AntiHacker();
			//await ant.CheckDirectoriesAsync();
			if (StaticProperties.EditingEnabled)
				{
				if (!UserIsEntitled)
					{
					MessageBox.Show("You are not entitled to access the settings.", Application.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
					}
				IUIVisualizerService? uIVisualiserService = ServiceLocator.Default.ResolveType<IUIVisualizerService>();
				if (uIVisualiserService is not null)
					{
					await uIVisualiserService.ShowDialogAsync<EmailRecipientsViewModel>();
					}
				}
			}

		/// <summary>
		/// Gets the CampaignGalleryCommand command.
		/// </summary>
		public TaskCommand<object?> CampaignGalleryCommand { get; private set; }

		/// <summary>
		/// Method to invoke when the CampaignGalleryCommand command is executed.
		/// </summary>
		private async Task<bool?> OnCampaignGalleryExecute(object? parameter)
			{
			var cgvm = TypeFactory.Default.CreateInstanceWithParametersAndAutoCompletion<CampaignGalleryViewModel>();
			return (await uiVisualiserService.ShowDialogAsync(cgvm!)).DialogResult;
			}

		/// <summary>
		/// Gets the RecordingsCommand command.
		/// </summary>
		public TaskCommand<object?> RecordingsCommand
			{
			get
				{
				return recordingsCommand ??= new TaskCommand<object?>(RecordingsCommand_Execute);
				}
			}

		private TaskCommand<object?>? recordingsCommand;

		/// <summary>
		/// Method to invoke when the RecordingsCommand command is executed.
		/// </summary>
		/// <param name="parameter">The parameter of the command.</param>
		private async Task<bool?> RecordingsCommand_Execute(object? parameter)
			{
			var vm = TypeFactory.Default.CreateInstanceWithParametersAndAutoCompletion<RecordingsViewModel>();
			return (await uiVisualiserService.ShowDialogAsync(vm!)).DialogResult;
			}

		/// <summary>
		/// Gets the SundayCommand command.
		/// </summary>
		public TaskCommand<object?> SundayCommand
			{
			get
				{
				return sundayCommand ??= new TaskCommand<object?>(SundayCommand_Execute);
				}
			}

		private TaskCommand<object?>? sundayCommand;

		/// <summary>
		/// Method to invoke when the RundayCommand command is executed.
		/// </summary>
		/// <param name="parameter">The parameter of the command.</param>
		private async Task<bool?> SundayCommand_Execute(object? parameter)
			{
			var vm = TypeFactory.Default.CreateInstanceWithParametersAndAutoCompletion<SundaysViewModel>();
			return (await uiVisualiserService.ShowDialogAsync(vm!)).DialogResult;
			}

		/// <summary>
		/// Gets the SundayCommand command.
		/// </summary>
		public TaskCommand<object?> BibleClassCommand
			{
			get
				{
				return bibleCassCommand ??= new TaskCommand<object?>(BibleClassCommand_Execute);
				}
			}

		private TaskCommand<object?>? bibleCassCommand;

		/// <summary>
		/// Method to invoke when the RundayCommand command is executed.
		/// </summary>
		/// <param name="parameter">The parameter of the command.</param>
		private async Task<bool?> BibleClassCommand_Execute(object? parameter)
			{
			var vm = TypeFactory.Default.CreateInstanceWithParametersAndAutoCompletion<BibleClassViewModel>();
			return (await uiVisualiserService.ShowDialogAsync(vm!)).DialogResult;
			}

		/// <summary>
		/// Gets the LockdownProgrammeCommand command.
		/// </summary>
		public TaskCommand<object?, object?> LockdownProgrammeCommand
			{
			get
				{
				return lockdownProgrammeCommand ??= new TaskCommand<object?, object?>(LockdownProgrammeCommand_Execute, LockdownProgrammeCommand_CanExecute);
				}
			}

		private bool LockdownProgrammeCommand_CanExecute(object? param)
			{
			return StaticProperties.EditingEnabled;
			}

		private TaskCommand<object?, object?>? lockdownProgrammeCommand;

		/// <summary>
		/// Method to invoke when the lockdownProgrammeCommand command is executed.
		/// </summary>
		/// <param name="parameter">The parameter of the command.</param>
		private async Task<bool?> LockdownProgrammeCommand_Execute(object? parameter)
			{
			bool reportedError = false;

			var filePath = @"C:\Users\Phil\OneDrive\My Documents\Ecclesia\Programme\LockdownProgramme.csv";

			logger.Info("Uploading programme CSV file");
			var result = Uploader.Upload(filePath, "private/programme/LockdownProgramme.csv");

			if (result)
				{
				try
					{
					var prog = LockdownProgramme.Load(filePath);
					var sundays = prog.Sunday;
					var bibleClass = prog.BibleClass;

					filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Ecclesia\Programme\Ecclesial programme.pdf");
					result = prog.CreatePdf(filePath, new DateTime(2024, 9, 1));
					if (result)
						{
						result = Uploader.Upload(filePath, "programme/Ecclesial programme.pdf", true);
						if (result)
							logger.Info("Programme PDF uploaded successfully.");
						else
							logger.Error("Programme PDF upload failed.");
						}
					}
				catch (Exception ex)
					{
					logger.Error("Error creating PDF: {0}", ex.Message);
					MessageBox.Show($"Error creating PDF: {ex.Message}", Application.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);
					reportedError = true;
					}
				}

			if (result)
				{
				var emailed = await MailSender.SendMailAsync(MailSender.CreateProgrammeUpdateEmail());

				if (emailed)
					{
					logger.Info("Programme CSV uploaded successfully and email sent");
					MessageBox.Show("Programme update uploaded successfully and email sent", Application.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Information);
					}
				else
					{
					logger.Info("Programme CSV uploaded successfully but email sending failed");
					MessageBox.Show("Programme update uploaded successfully but email sending failed", Application.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
					}
				}
			else if (!reportedError)
				{
				logger.Error("Error uploading programme CSV");
				MessageBox.Show("Oops! Something went wrong", Application.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);
				}
			return await Task.FromResult(result);
			}

		protected override Task CloseAsync()
			{
			return base.CloseAsync();
			}
		}
	}
