//
//	Last mod:	04 February 2025 12:07:59
//
using System.Threading.Tasks;
using Catel.MVVM;
using Catel.Services;

namespace WebWriter.ViewModels
	{
	public class AddRecordingViewModel : ViewModelBase
		{
		private readonly IOpenFileService openFileService;
		private readonly DetermineOpenFileContext fileOpenContext = new();

		public AddRecordingViewModel(IOpenFileService openFileService)
			{
			HideValidationResults = false;
			this.openFileService = openFileService;
			}

		public override string Title { get { return "Add Recording"; } }

		public uint TypeId { get; set; }

		public string? FilePath { get; set; }

		public string? Text { get; set; }

		public string? Speaker { get; set; }

		public string? Ecclesia { get; set; }

		/// <summary>
		/// Gets the BrowseCommand command.
		/// </summary>
		public TaskCommand<object> BrowseCommand
			{
			get
				{
				browseCommand ??= new TaskCommand<object>(BrowseCommand_ExecuteAsync);
				return browseCommand;
				}
			}

		private TaskCommand<object>? browseCommand;

		/// <summary>
		/// Method to invoke when the BrowseCommand command is executed.
		/// </summary>
		/// <param name="parameter">The parameter of the command.</param>
		private async Task BrowseCommand_ExecuteAsync(object parameter)
			{
			fileOpenContext.Title = "Select the audio recording";
			fileOpenContext.Filter = "mp3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
			var dfresult = await openFileService.DetermineFileAsync(fileOpenContext);
			if (dfresult.Result)
				{
				FilePath = dfresult.FileName;
				}
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
