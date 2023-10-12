//
//	Last mod:	11 October 2023 16:59:27
//
namespace WebWriter.ViewModels
	{
	using System;
	using System.Data;
	using System.Threading.Tasks;
	using System.Windows;
	using Catel.Data;
	using Catel.MVVM;
	using Models;
	using MySql.Data.MySqlClient;

	public class SundaysViewModel : ViewModelBase
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

		// TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets

		protected override async Task InitializeAsync()
			{
			await base.InitializeAsync();

			try
				{
				using (dbCon = new StaffordMySQLConnection())
					if (dbCon.Open())
						{
						string query = "SELECT Id, Date, Speaker, Ecclesia, Email, Timestamp, Processed FROM SundayDates ORDER BY Date";
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

			// TODO: subscribe to events here
			}

		protected override Task<bool> SaveAsync()
			{
			if (dsSundays.HasChanges())
				{
				var reply = MessageBox.Show("Save the changes you made", Title, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
				switch (reply)
					{
				case MessageBoxResult.Yes:
					try
						{
						using (dbCon = new StaffordMySQLConnection())
							if (dbCon.Open())
								{
								daSundays.Update(dsSundays.Tables["SundayDates"]);
								}
						}
					catch (Exception ex)
						{
						MessageBox.Show(ex.InnerException.Message);
						}
					break;

				case MessageBoxResult.No:
					dsSundays.Clear();
					break;

				case MessageBoxResult.Cancel:
				default:
					return Task.FromResult(false);
					}
				}
			return base.SaveAsync();
			}

		protected override Task<bool> CancelAsync()
			{
			if (dsSundays.HasChanges())
				{
				var reply = MessageBox.Show("Discard the changes you made", Title, MessageBoxButton.YesNo, MessageBoxImage.Question);
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
