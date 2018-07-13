//
//	Last mod:	13 January 2017 16:50:46
//
namespace WebWriter.ViewModels
	{
	using Catel.Data;
	using Catel.MVVM;
	using Microsoft.Win32;
	using Models;
	using MySql.Data.MySqlClient;
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Data;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Windows;

	public class RecordingsViewModel : ViewModelBase
		{
		public class Recording : ModelBase
			{
			public DateTime Date { get; set; }
			public string Type { get; set; }
			public string File { get; set; }
			public string Text { get; set; }
			public string Speaker { get; set; }
			public string Ecclesia { get; set; }
			}

		public RecordingsViewModel()
			{
			}

		public override string Title { get { return "Recordings"; } }

		StaffordMySQLConnection dbCon;
		MySqlDataAdapter daRecordings;
		DataSet dsRecordings;
		MySqlDataAdapter daRecordingTypes;
		DataSet dsRecordingTypes;

		const string filePath = @"D:\Users\philj\OneDrive\My Documents\Ecclesia\Web site\recordings.xml"; // @"D:\philj\Documents\OneDrive\My Documents\Ecclesia\Web site\recordings.xml";

		List<string> addedFiles = new List<string>();

		// TODO: Register models with the vmpropmodel codesnippet
		// TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets

		public ObservableCollection<Recording> oldRecordings { get; set; }

		public string NewRecording { get; set; }

		public DataView Recordings { get; set; }

		public static DataView RecordingTypes { get; set; }


		// TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets

		/// <summary>
		/// Gets the BrowseCommand command.
		/// </summary>
		public Command<object> BrowseCommand
			{
			get
				{
				if (_BrowseCommand == null)
					_BrowseCommand = new Command<object>(BrowseCommand_Execute);
				return _BrowseCommand;
				}
			}

		private Command<object> _BrowseCommand;

		/// <summary>
		/// Method to invoke when the BrowseCommand command is executed.
		/// </summary>
		/// <param name="parameter">The parameter of the command.</param>
		private void BrowseCommand_Execute(object parameter)
			{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "mp3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
			if (ofd.ShowDialog() == true)
				{
				NewRecording = ofd.FileName;
				}
			}

		/// <summary>
		/// Gets the AddCommand command.
		/// </summary>
		public Command<object, object> AddCommand
			{
			get
				{
				if (_AddCommand == null)
					_AddCommand = new Command<object, object>(AddCommand_Execute, AddCommand_CanExecute);
				return _AddCommand;
				}
			}

		private Command<object, object> _AddCommand;

		/// <summary>
		/// Method to check whether the AddCommand command can be executed.
		/// </summary>
		/// <param name="parameter">The parameter of the command.</param>
		private bool AddCommand_CanExecute(object parameter)
			{
			return !string.IsNullOrWhiteSpace(NewRecording);
			}

		/// <summary>
		/// Method to invoke when the AddCommand command is executed.
		/// </summary>
		/// <param name="parameter">The parameter of the command.</param>
		private void AddCommand_Execute(object parameter)
			{
			var t = dsRecordings.Tables["Recordings"];
			if (!t.AsEnumerable().Any(r=>(string)r["File"] == NewRecording))
				{
				var row = t.NewRow();
				row["Date"] = DateTime.Now;
				row["File"] = "lib/" + Path.GetFileName(NewRecording);
				t.Rows.Add(row);
				addedFiles.Add(NewRecording);
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
					string query = "SELECT Id,Date,TypeId,File,Speaker,Ecclesia,Text FROM Recordings ORDER BY Date";
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
				MessageBox.Show(ex.Message);
				}

			// TODO: subscribe to events here
			}

		protected override Task<bool> SaveAsync()
			{
			// upload all the added files
			foreach (var file in addedFiles)
				{
				string remotePath = "private/lib/" + Path.GetFileName(file);
				Uploader.Upload(file, remotePath, true);
				}

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
					MessageBox.Show(ex.Message);
					}
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
