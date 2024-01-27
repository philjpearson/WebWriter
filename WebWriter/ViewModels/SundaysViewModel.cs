//
//	Last mod:	13 October 2023 18:42:06
//
namespace WebWriter.ViewModels
	{
	using System;
	using System.Data;
	using System.Threading.Tasks;
	using System.Windows;
	using Catel.MVVM;
	using Models;
	using MySql.Data.MySqlClient;
	using PJP.Utilities;
	using WebWriter.Utilities;

	public class SundaysViewModel : ViewModelBase
		{
		public SundaysViewModel()
			{
			}

		public override string Title { get { return "Sunday bookings"; } }

		StaffordMySQLConnection dbCon;
		MySqlDataAdapter daSundays;
		DataSet dsSundays;

		// TODO: Register models with the vmpropmodel codesnippet
		// TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets

		public DataView Sundays { get; set; }

		public bool FutureOnly { get; set; } = true;

		public bool UnprocessedOnly { get; set; }

		// TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets

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
							where += $"AND Date > '{DateTime.Now.ToString("yyyy-MM-dd")}' ";
							}
						if (UnprocessedOnly)
							{
							where += "AND Processed = False ";
							}
						string query = $"SELECT Id, Date, Speaker, Ecclesia, Email, Timestamp, Processed FROM SundayDates {where}ORDER BY Date";
						daSundays = new MySqlDataAdapter(query, dbCon.Connection);
						MySqlCommandBuilder cb = new MySqlCommandBuilder(daSundays);

						dsSundays = new DataSet();
						daSundays.Fill(dsSundays, "SundayDates");
						Sundays = dsSundays.Tables["SundayDates"].DefaultView;
						}
				}
			catch (Exception ex)
				{
				MessageBox.Show(ex.InnerException.Message);
				}
			}

		protected override Task<bool> SaveAsync()
			{
			if (dsSundays.HasChanges())
				{
				try
					{
					using (var wait = new CursorOverride("earth.ani"))
					using (dbCon = new StaffordMySQLConnection())
						if (dbCon.Open())
							{
							daSundays.Update(dsSundays.Tables["SundayDates"]);
							}
					}
				catch (Exception ex)
					{
					MessageBox.Show(ex.Innermost().Message, Title);
					}
				}
			return base.SaveAsync();
			}

		protected override Task<bool> CancelAsync()
			{
			if (dsSundays.HasChanges())
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
