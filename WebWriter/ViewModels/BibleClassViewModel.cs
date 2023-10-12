//
//	Last mod:	12 October 2023 12:13:15
//
namespace WebWriter.ViewModels
	{
	using System;
	using System.Data;
	using System.Threading.Tasks;
	using System.Windows;
	using Catel.MVVM;
	using Models;
	using PJP.Utilities;
	using MySql.Data.MySqlClient;
	using WebWriter.Utilities;

	public class BibleClassViewModel : ViewModelBase
		{
		public BibleClassViewModel()
			{
			}

		public override string Title { get { return "Bible Class bookings"; } }

		StaffordMySQLConnection dbCon;
		MySqlDataAdapter daBibleClass;
		DataSet dsBibleClass;

		// TODO: Register models with the vmpropmodel codesnippet
		// TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets

		public DataView BibleClass { get; set; }

		// TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets

		protected override async Task InitializeAsync()
			{
			await base.InitializeAsync();

			try
				{
				using (var wait = new CursorOverride("earth.ani"))
				using (dbCon = new StaffordMySQLConnection())
					if (dbCon.Open())
						{
						string query = "SELECT BibleClassDates.Id, Date, BibleClassDates.Speaker, BibleClassDates.Ecclesia, BibleClassDates.Email, BibleClassDates.Title, TitleId, BibleClassDates.Timestamp, Processed, BibleClassSubjects.Title AS Subject FROM (BibleClassDates LEFT JOIN BibleClassSubjects ON(BibleClassDates.TitleId = BibleClassSubjects.Id)) ORDER BY Date;";
						// string query = "SELECT Id, Date, Speaker, Ecclesia, Email, Title, TitleId, Timestamp, Processed FROM BibleClassDates ORDER BY Date";
						daBibleClass = new MySqlDataAdapter(query, dbCon.Connection);
						MySqlCommandBuilder cb = new MySqlCommandBuilder(daBibleClass);

						dsBibleClass = new DataSet();
						daBibleClass.Fill(dsBibleClass, "BibleClassDates");
						BibleClass = dsBibleClass.Tables["BibleClassDates"].DefaultView;
						}
				}
			catch (Exception ex)
				{
				MessageBox.Show(ex.Innermost().Message, Title);
				}

			// TODO: subscribe to events here
			}

		protected override Task<bool> SaveAsync()
			{
			if (dsBibleClass.HasChanges())
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
							dsTemp.Merge(changesDataset);
							daTemp.Update(dsTemp.Tables["BibleClassDates"]);
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
			if (dsBibleClass.HasChanges())
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
