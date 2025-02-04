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
	using Models;
	using MySqlConnector;
	using PJP.Utilities;
	using WebWriter.Utilities;

	public class SundaysViewModel : ViewModelBase
		{
		private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

		public SundaysViewModel()
			{
			}

		public override string Title { get { return "Sunday bookings"; } }

		StaffordMySQLConnection? dbCon;
		MySqlDataAdapter? daSundays;
		DataSet? dsSundays;

		public DataView? Sundays { get; set; }

		public bool FutureOnly { get; set; } = true;

		public bool UnprocessedOnly { get; set; }

		/// <summary>
		/// Gets the ExportCommand command.
		/// </summary>
		public TaskCommand ExportCommand => exportCommand ??= new TaskCommand(ExportCommandExecuteAsync, ExportCommandCanExecute);

		private TaskCommand? exportCommand;

		private bool ExportCommandCanExecute()
			{
			return Sundays?.Count > 0;
			}

		private string filename = @"C:\Users\Phil\OneDrive\My Documents\Ecclesia\Programme\SundayProgrammeExport.xlsx";

		private async Task ExportCommandExecuteAsync()
			{
			var prog = new ProgrammeModel(DayOfWeek.Sunday);
			prog.CreateEmpty(new DateTime(2025, 1, 1), new DateTime(2026, 12, 31));
			prog.Populate(Sundays!);
			await prog.ExportToExcel(filename, "Sunday");
			}

		protected override async Task InitializeAsync()
			{
			await base.InitializeAsync();

			LoadData();

			// TODO: subscribe to events here
			}

		private void OnFutureOnlyChanged()
			{
			LoadData();
			}

		private void OnUnprocessedOnlyChanged()
			{
			LoadData();
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
						string query = $"SELECT Id, Date, Speaker, Ecclesia, Email, Timestamp, Processed FROM SundayDates {where}ORDER BY Date";
						daSundays = new MySqlDataAdapter(query, dbCon.Connection);
						var cb = new MySqlCommandBuilder(daSundays);

						dsSundays = new DataSet();
						daSundays.Fill(dsSundays, "SundayDates");
						Sundays = dsSundays.Tables["SundayDates"]!.DefaultView;
						dbCon.Close();
						}
				}
			catch (Exception ex)
				{
				logger.Error("Error loading Sundays data from database: {0}", ex.Message);
				MessageBox.Show(ex.InnerException?.Message);
				}
			}

		protected override Task<bool> SaveAsync()
			{
			if (dsSundays?.HasChanges() == true)
				{
				try
					{
					using (var wait = new CursorOverride("earth.ani"))
					using (dbCon = new StaffordMySQLConnection())
						if (dbCon.Open())
							{
							string query = $"SELECT Id, Date, Speaker, Ecclesia, Email, Timestamp, Processed FROM SundayDates";
							var daTemp = new MySqlDataAdapter(query, dbCon.Connection)
								{
								MissingSchemaAction = MissingSchemaAction.Ignore
								};
							var cb = new MySqlCommandBuilder(daTemp);
							var dsTemp = new DataSet();
							daTemp.Fill(dsTemp, "SundayDates");
							var changesDataset = dsSundays.GetChanges();
							dsTemp.Merge(changesDataset!);
							daTemp.Update(dsTemp.Tables["SundayDates"]!);
							dbCon.Close();
							}
					}
				catch (Exception ex)
					{
					logger.Error("Error saving changes to Sundays: {0}", ex.Message);
					MessageBox.Show(ex.Message, Title);
					}
				}
			return base.SaveAsync();
			}

		protected override Task<bool> CancelAsync()
			{
			if (dsSundays?.HasChanges() == true)
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
		}
	}
