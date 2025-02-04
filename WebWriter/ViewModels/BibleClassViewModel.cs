//
//	Last mod:	04 February 2025 15:07:06
//
namespace WebWriter.ViewModels
	{
	using System;
	using System.Data;
	using System.Threading.Tasks;
	using System.Windows;
	using Catel.MVVM;
	using MySqlConnector;
	using PJP.Utilities;
	using WebWriter.Models;
	using WebWriter.Utilities;

#nullable enable

	public class BibleClassViewModel : ViewModelBase
		{
		private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

		public BibleClassViewModel()
			{
			}

		public override string Title { get { return "Bible Class bookings"; } }

		StaffordMySQLConnection? dbCon;
		MySqlDataAdapter? daBibleClass;
		DataSet? dsBibleClass;

		public DataView? BibleClass { get; set; }

		public bool FutureOnly { get; set; } = true;

		public bool UnprocessedOnly { get; set; }

		/// <summary>
		/// Gets the ExportCommand command.
		/// </summary>
		public TaskCommand ExportCommand => exportCommand ??= new TaskCommand(ExportCommandExecuteAsync, ExportCommandCanExecute);

		private TaskCommand? exportCommand;

		private bool ExportCommandCanExecute()
			{
			return BibleClass?.Count > 0;
			}

		private string filename = @"C:\Users\Phil\OneDrive\My Documents\Ecclesia\Programme\BCProgrammeExport.xlsx";

		private async Task ExportCommandExecuteAsync()
			{
			var prog = new ProgrammeModel(DayOfWeek.Wednesday);
			prog.CreateEmpty(new DateTime(2025, 1, 1), new DateTime(2026, 12, 31));
			prog.Populate(BibleClass!);
			await prog.ExportToExcel(filename, "Bible Class");
			}

		protected override async Task InitializeAsync()
			{
			await base.InitializeAsync();

			LoadData();

			// TODO: subscribe to events here
			}

		private void LoadData()
			{
			try
				{
				using (var wait = new CursorOverride("earth.ani"))
				using (dbCon = new StaffordMySQLConnection())
					if (dbCon.Open())
						{
						string where = "Where True ";
						if (FutureOnly)
							{
							where += $"AND Date > '{DateTime.Now:yyyy-MM-dd}' ";
							}
						if (UnprocessedOnly)
							{
							where += "AND Processed = False ";
							}
						string query = $"SELECT BCD.Id, Date, BCD.Speaker, BCD.Ecclesia, BCD.Email, BCD.Title, TitleId, BCD.Timestamp, Processed, BibleClassSubjects.Title AS Subject FROM (BibleClassDates AS BCD LEFT JOIN BibleClassSubjects ON(BCD.TitleId = BibleClassSubjects.Id)) {where}ORDER BY Date;";
						daBibleClass = new MySqlDataAdapter(query, dbCon.Connection);
						MySqlCommandBuilder cb = new MySqlCommandBuilder(daBibleClass);

						dsBibleClass = new DataSet();
						daBibleClass.Fill(dsBibleClass, "BibleClassDates");
						BibleClass = dsBibleClass.Tables["BibleClassDates"]!.DefaultView;
						}
				}
			catch (Exception ex)
				{
				logger.Error("Error loading Bible Class data from database: {0}", ex.Message);
				MessageBox.Show(ex.Innermost().Message, Title);
				}
			}

		protected override Task<bool> SaveAsync()
			{
			if (dsBibleClass?.HasChanges() == true)
				{
				try
					{
					using (var wait = new CursorOverride("earth.ani"))
					using (dbCon = new StaffordMySQLConnection())
						if (dbCon.Open())
							{
							string query = "SELECT Id, Date, Speaker, Ecclesia, Email, Title, TitleId, Timestamp, Processed FROM BibleClassDates";
							var daTemp = new MySqlDataAdapter(query, dbCon.Connection)
								{
								MissingSchemaAction = MissingSchemaAction.Ignore
								};
							MySqlCommandBuilder cb = new MySqlCommandBuilder(daTemp);
							var dsTemp = new DataSet();
							daTemp.Fill(dsTemp, "BibleClassDates");
							var changesDataset = dsBibleClass.GetChanges();
							dsTemp.Merge(changesDataset!);
							daTemp.Update(dsTemp.Tables["BibleClassDates"]!);
							}
					}
				catch (Exception ex)
					{
					logger.Error("Error saving Bible Class changes to database: {0}", ex.Message);
					MessageBox.Show(ex.Message, Title);
					}
				}
			return base.SaveAsync();
			}

		protected override Task<bool> CancelAsync()
			{
			if (dsBibleClass?.HasChanges() == true)
				{
				var reply = MessageBox.Show("Discard the changes you made?", Title, MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (reply == MessageBoxResult.No)
					{
					return Task.FromResult(false);
					}
				}
			return base.CancelAsync();
			}

		protected override async Task CloseAsync()
			{
			await base.CloseAsync();
			}

		private void OnFutureOnlyChanged()
			{
			LoadData();
			}

		private void OnUnprocessedOnlyChanged()
			{
			LoadData();
			}
		}
	}
