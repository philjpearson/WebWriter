//
//	Last mod:	27 January 2023 16:23:09
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
	using MySql.Data.MySqlClient;

	public class RecordingsViewModel : ViewModelBase
		{
		private readonly UIVisualizerService uiVisualiserService;

		public class Recording : ModelBase
			{
			public DateTime Date { get; set; }
			public string Type { get; set; }
			public string File { get; set; }
			public string Text { get; set; }
			public string Speaker { get; set; }
			public string Ecclesia { get; set; }
			}

		public RecordingsViewModel(/*UIVisualizerService uiVisualiserService*/)
			{
			// this.uiVisualiserService = uiVisualiserService;
			this.uiVisualiserService = ServiceLocator.Default.ResolveType<UIVisualizerService>();
			}

		public override string Title { get { return "Recordings"; } }

		StaffordMySQLConnection dbCon;
		MySqlDataAdapter daRecordings;
		DataSet dsRecordings;
		MySqlDataAdapter daRecordingTypes;
		DataSet dsRecordingTypes;

		const string filePath = @"C:\Users\Phil\OneDrive\My Documents\Ecclesia\Web site\recordings.xml"; // @"D:\philj\Documents\OneDrive\My Documents\Ecclesia\Web site\recordings.xml";

		List<RecordingToAdd> addedRecordings = new();

		// TODO: Register models with the vmpropmodel codesnippet
		// TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets

		public ObservableCollection<Recording> oldRecordings { get; set; }

		public DataView Recordings { get; set; }

		public static DataView RecordingTypes { get; set; }


		// TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets

		/// <summary>
		/// Gets the AddCommand command.
		/// </summary>
		public TaskCommand<object, object> AddCommand
			{
			get
				{
				addCommand ??= new TaskCommand<object, object>(AddCommand_ExecuteAsync, AddCommand_CanExecute);
				return addCommand;
				}
			}

		private TaskCommand<object, object> addCommand;

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
				var t = dsRecordings.Tables["Recordings"];
				if (!t.AsEnumerable().Any(r => (string)r["File"] == vm.FilePath))
					{
					var recordingToAdd = new RecordingToAdd
						{
						TypeId = vm.TypeId,
						FilePath = vm.FilePath,
						Text = vm.Text,
						Speaker = vm.Speaker,
						Ecclesia= vm.Ecclesia,
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
				dbCon = StaffordMySQLConnection.Instance();
				if (dbCon.IsConnect())
					{
//					string query = "SELECT Id,Date,TypeId,File,Speaker,Ecclesia,Text FROM Recordings ORDER BY Date";
					string query = "SELECT Id, Date, Recordings.TypeId, Type, File, Text, Speaker, Ecclesia FROM Recordings JOIN RecordingTypes ON Recordings.TypeId = RecordingTypes.TypeId ORDER BY Date";
					daRecordings = new MySqlDataAdapter(query, dbCon.Connection);
					MySqlCommandBuilder cb = new MySqlCommandBuilder(daRecordings);

					dsRecordings = new DataSet();
					daRecordings.Fill(dsRecordings, "Recordings");
					Recordings = dsRecordings.Tables["Recordings"].DefaultView;

					query = "SELECT TypeId, Type FROM RecordingTypes";
					daRecordingTypes = new MySqlDataAdapter(query, dbCon.Connection);
					MySqlCommandBuilder cbT = new MySqlCommandBuilder(daRecordingTypes);
					dsRecordingTypes = new DataSet();
					daRecordingTypes.Fill(dsRecordingTypes, "RecordingTypes");
					RecordingTypes = dsRecordingTypes.Tables["RecordingTypes"].DefaultView;
					}
				}
			catch (Exception ex)
				{
				MessageBox.Show(ex.InnerException.Message);
				}
			finally
				{
				dbCon.Close();
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
				if (dbCon.IsConnect())
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
				MessageBox.Show(ex.InnerException.Message);
				}
			finally
				{
				dbCon.Close();
				}

			/*
			if (dsRecordings != null)
				{
				try
					{
					daRecordings.Update(dsRecordings, "Recordings");
					dsRecordings.Clear();
					daRecordings.Fill(dsRecordings, "Recordings");
					}
				catch (Exception ex)
					{
					MessageBox.Show(ex.InnerException.Message);
					}
				}
			*/
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
