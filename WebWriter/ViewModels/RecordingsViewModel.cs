//
//	Last mod:	04 February 2025 15:07:06
//
namespace WebWriter.ViewModels
	{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Data;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Windows;
	using Catel.Data;
	using Catel.IoC;
	using Catel.MVVM;
	using Catel.Services;
	using Models;
	using MySqlConnector;
	using PJP.Utilities;

	public class RecordingsViewModel : ViewModelBase
		{
		private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

		private readonly UIVisualizerService uiVisualiserService;

		public class Recording : ModelBase
			{
			public DateTime Date { get; set; }
			public string? Type { get; set; }
			public string? File { get; set; }
			public string? Text { get; set; }
			public string? Speaker { get; set; }
			public string? Ecclesia { get; set; }
			}

		public RecordingsViewModel()
			{
			uiVisualiserService = ServiceLocator.Default.ResolveType<UIVisualizerService>();
			}

		public override string Title { get { return "Recordings"; } }

		private StaffordMySQLConnection? dbCon;
		private MySqlDataAdapter? daRecordings;
		private DataSet? dsRecordings;
		private MySqlDataAdapter? daRecordingTypes;
		private DataSet? dsRecordingTypes;

//		private const string filePath = @"C:\Users\Phil\OneDrive\My Documents\Ecclesia\Web site\recordings.xml";

		private readonly List<RecordingToAdd> addedRecordings = [];

		public ObservableCollection<Recording>? OldRecordings { get; set; }

		public DataView? Recordings { get; set; }

		public static DataView? RecordingTypes { get; set; }


		// TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets

		/// <summary>
		/// Gets the AddCommand command.
		/// </summary>
		public TaskCommand<object, object> AddCommand
			{
			get
				{
				return addCommand ??= new TaskCommand<object, object>(AddCommand_ExecuteAsync, AddCommand_CanExecute);
				}
			}

		private TaskCommand<object, object>? addCommand;

		/// <summary>
		/// Method to check whether the AddCommand command can be executed.
		/// </summary>
		/// <param name="parameter">The parameter of the command.</param>
		private bool AddCommand_CanExecute(object parameter)
			{
			return true;
			}

		/// <summary>
		/// Method to invoke when the AddCommand command is executed.
		/// </summary>
		/// <param name="parameter">The parameter of the command.</param>
		private async Task AddCommand_ExecuteAsync(object parameter)
			{
			var vm = TypeFactory.Default.CreateInstanceWithParametersAndAutoCompletion<AddRecordingViewModel>();
			if (await uiVisualiserService.ShowDialogAsync(vm) == true)
				{
				var t = dsRecordings!.Tables["Recordings"]!;
				if (!t.AsEnumerable().Any(r => (string)r["File"] == vm.FilePath))
					{
					var recordingToAdd = new RecordingToAdd
						{
						TypeId = vm.TypeId,
						FilePath = vm.FilePath!,
						Text = vm.Text ?? string.Empty,
						Speaker = vm.Speaker ?? string.Empty,
						Ecclesia = vm.Ecclesia ?? string.Empty,
						};
					addedRecordings.Add(recordingToAdd);
					}
				}
			}

		protected override async Task InitializeAsync()
			{
			await base.InitializeAsync();

			try
				{
				using (dbCon = new StaffordMySQLConnection())
					if (dbCon.Open())
						{
						string query = "SELECT Id, Date, Recordings.TypeId, Type, File, Text, Speaker, Ecclesia FROM Recordings JOIN RecordingTypes ON Recordings.TypeId = RecordingTypes.TypeId ORDER BY Date";
						daRecordings = new MySqlDataAdapter(query, dbCon.Connection);
						MySqlCommandBuilder cb = new MySqlCommandBuilder(daRecordings);

						dsRecordings = new DataSet();
						daRecordings.Fill(dsRecordings, "Recordings");
						Recordings = dsRecordings.Tables["Recordings"]!.DefaultView;

						query = "SELECT TypeId, Type FROM RecordingTypes";
						daRecordingTypes = new MySqlDataAdapter(query, dbCon.Connection);
						MySqlCommandBuilder cbT = new MySqlCommandBuilder(daRecordingTypes);
						dsRecordingTypes = new DataSet();
						daRecordingTypes.Fill(dsRecordingTypes, "RecordingTypes");
						RecordingTypes = dsRecordingTypes.Tables["RecordingTypes"]!.DefaultView;
						}
				}
			catch (Exception ex)
				{
				logger.Error("Error retrieving records from database: {0}", ex.Innermost().Message);
				MessageBox.Show(ex.Innermost().Message);
				}

			// TODO: subscribe to events here
			}

		protected override Task<bool> SaveAsync()
			{
			// upload all the added files
			foreach (var rec in addedRecordings)
				{
				string remotePath = "private/lib/" + Path.GetFileName(rec.FilePath);
				Uploader.Upload(rec.FilePath, remotePath, true);
				}

			try
				{
				using (dbCon = new StaffordMySQLConnection())
					if (dbCon.Open())
						{
						foreach (var rec in addedRecordings)
							{
							string query = "INSERT INTO Recordings (Date,TypeId,File,Speaker,Ecclesia,Text) VALUES(@date,@typeid,@file,@speaker,@ecclesia,@text)";

							MySqlCommand cmd = new MySqlCommand(query, dbCon);
							cmd.Parameters.AddWithValue("@date", DateTime.UtcNow);
							cmd.Parameters.AddWithValue("@typeid", rec.TypeId);
							cmd.Parameters.AddWithValue("@file", "lib/" + Path.GetFileName(rec.FilePath));
							cmd.Parameters.AddWithValue("@speaker", rec.Speaker);
							cmd.Parameters.AddWithValue("@ecclesia", rec.Ecclesia);
							cmd.Parameters.AddWithValue("@text", rec.Text);
							cmd.ExecuteNonQuery();
							}
						}
				}
			catch (Exception ex)
				{
				logger.Error("Error saving changes to database: {0}", ex.Message);
				MessageBox.Show(ex.Innermost().Message);
				}

			return base.SaveAsync();
			}

		protected override async Task CloseAsync()
			{
			// TODO: unsubscribe from events here

			dbCon?.Close();
			await base.CloseAsync();
			}
		}
	}
