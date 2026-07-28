//
//	Last mod:	28 July 2026 17:16:50
//
namespace WebWriter.ViewModels
	{
	using System;
	using System.Collections.ObjectModel;
	using System.Data;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Windows;
	using Catel;
	using Catel.Fody;
	using Catel.MVVM;
	using Catel.Services;
	using MySqlConnector;
	using OfficeOpenXml;
	using PJP.Utilities;
	using WebWriter.Models;
	using WebWriter.Utilities;
	using static WebWriter.Models.BcGalleryModel;

	public class BcGalleryViewModel : ViewModelBase
		{
		private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

		private const string excelFilePath = @"C:\Users\Phil\OneDrive\My Documents\Ecclesia\BC videos.xlsx";

		private readonly IMessageService messageService;

		public BcGalleryViewModel(IMessageService messageService)
			{
			Argument.IsNotNull(() => messageService);
			this.messageService = messageService;

			ExcelPackage.License.SetNonCommercialPersonal("Phil J Pearson");

			OkCommand = new Command<object>(OnOkCommandExecute);
			SortCommand = new Command(OnSortCommandExecute);

			Gallery = new BcGalleryModel();
			}

		public override string Title { get { return "BC Gallery View"; } }

		StaffordMySQLConnection? dbCon;
		MySqlDataAdapter? daBcVideos;
		DataSet? dsBcVideos;

		public DataView? BcVideosView { get; set; }

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		[Model]
		[Expose(nameof(Videos))]
		public BcGalleryModel Gallery { get; set; }

		[ViewModelToModel]
		public ObservableCollection<BcVideoModel>? Videos { get; set; }

		/// <summary>
		/// Gets the OkCommand command.
		/// </summary>
		public Command<object> OkCommand { get; private set; }

		/// <summary>
		/// Method to invoke when the OkCommand command is executed.
		/// </summary>
		private async void OnOkCommandExecute()
			{
			if (await SaveAsync())
				{
				await CloseViewModelAsync(true);
				}
			}

		/// <summary>
		/// Gets the ExportCommand command.
		/// </summary>
		public TaskCommand ExportCommand => exportCommand ??= new TaskCommand(ExportCommandExecuteAsync, ExportCommandCanExecute);

		private TaskCommand? exportCommand;

		private bool ExportCommandCanExecute()
			{
			return StaticProperties.EditingEnabled;
			}

		/// <summary>
		/// Method to invoke when the ExportCommand command is executed.
		/// </summary>
		private async Task<bool> ExportCommandExecuteAsync()
			{
			return await SaveVideosToExcelAsync();
			}

		/// <summary>
		/// Gets the ExportCommand command.
		/// </summary>
		public TaskCommand ImportCommand => importCommand ??= new TaskCommand(ImportCommandExecuteAsync, ImportCommandCanExecute);

		private TaskCommand? importCommand;

		private bool ImportCommandCanExecute()
			{
			return StaticProperties.EditingEnabled;
			}

		/// <summary>
		/// Method to invoke when the ImportCommand command is executed.
		/// </summary>
		private async Task ImportCommandExecuteAsync()
			{
			GetVideosFromExcel();
			await Task.CompletedTask;
			}

		/// <summary>
		/// Gets the SortCommand command.
		/// </summary>
		public Command SortCommand { get; private set; }

		/// <summary>
		/// Method to invoke when the SortCommand command is executed.
		/// </summary>
		private void OnSortCommandExecute()
			{
			Gallery?.Sort();
			}

		protected override async Task InitializeAsync()
			{
			await base.InitializeAsync();

			try
				{
				Videos!.Clear();
				using (var wait = new CursorOverride("earth.ani"))
					{
					await LoadDataAsync();
					Gallery.ResetChanges();
					}
				}
			catch (Exception ex)
				{
				await messageService.ShowErrorAsync(ex.Message);
				}
			// TODO: subscribe to events here
			}

		protected override async Task<bool> CancelAsync()
			{
			if (Gallery.HasChanges)
				{
				var answer = await messageService.ShowAsync("Abandon changes?", Application.Current.MainWindow.Title, MessageButton.YesNo, MessageImage.Question);
				if (answer == MessageResult.No)
					return false;
				}
			return await base.CancelAsync();
			}

		protected override async Task<bool> SaveAsync()
			{
			if (StaticProperties.EditingEnabled && Gallery.HasChanges)
				{
				try
					{
					var changes = Gallery.GetChanges();
					foreach (var change in changes)
						{
						switch (change.Value)
							{
						case ChangeType.Edit:
							await UpdateVideoAsync(change.Key);
							break;

						case ChangeType.Add:
							await AddVideo(change.Key);
							break;

						case ChangeType.Delete:
							await DeleteVideo(change.Key);
							break;

						case ChangeType.None:
						default:
							break;
							}
						}
					}
				catch (Exception ex)
					{
					Console.Write(ex.ToString());
					await messageService.ShowErrorAsync(ex.Message);
					return false;
					}
				Gallery.ResetChanges();
				}
			return await base.SaveAsync();
			}

		private async Task LoadDataAsync()
			{
			try
				{
				using (var wait = new CursorOverride("earth.ani"))
				using (dbCon = new StaffordMySQLConnection())
				if (dbCon.Open())
					{
					string query = $"SELECT 'Key', Date, Title, Speaker, Ecclesia, Link FROM BCVideos ORDER BY Date;";
					daBcVideos = new MySqlDataAdapter(query, dbCon.Connection);
					//MySqlCommandBuilder cb = new MySqlCommandBuilder(daBcVideos);

					dsBcVideos = new DataSet();
					daBcVideos.Fill(dsBcVideos, "BCVideos");
					foreach (DataRow row in dsBcVideos.Tables["BCVideos"]!.Rows)
						{
						var video = new BcVideoModel()
							{
							Key = row.Field<string>("Key")!,
							Date = row.Field<MySqlDateTime>("Date").GetDateTime(),
							Title = row.Field<string>("Title")!,
							Speaker = row.Field<string>("Speaker") ?? string.Empty,
							Ecclesia = row.Field<string>("Ecclesia") ?? string.Empty,
							Link = row.Field<string>("Link")!
							};
						Videos!.Add(video);
						}
					}
				}
			catch (Exception ex)
				{
				logger.Error("Error loading Bible Class data from database: {0}", ex.Message);
				await messageService.ShowErrorAsync($"Add failed: {ex.Innermost().Message}", Application.Current.MainWindow.Title);
				}
			}

		private async Task AddVideo(BcVideoModel video)
			{
			string query = @$"INSERT INTO BCVideos (`Key`,`Date`,`Title`,`Speaker`,`Ecclesia`,`Link`)
												VALUES (@key,@date,@title,@speaker,@ecclesia,@link)";
			using (dbCon = new StaffordMySQLConnection())
			if (dbCon.Open())
				{
				using var cmd = dbCon.Connection.CreateCommand();
					cmd.CommandText = query;
					cmd.Parameters.AddWithValue("@key", video.Key);
					cmd.Parameters.AddWithValue("@date", new MySqlDateTime(video.Date));
					cmd.Parameters.AddWithValue("@title", video.Title);
					cmd.Parameters.AddWithValue("@speaker", video.Speaker);
					cmd.Parameters.AddWithValue("@ecclesia", video.Ecclesia);
					cmd.Parameters.AddWithValue("@link", video.Link);
					try
						{
					await cmd.ExecuteNonQueryAsync();
					}
				catch (Exception ex)
					{
					await messageService.ShowErrorAsync($"Add failed: {ex.Innermost().Message}", Application.Current.MainWindow.Title);
					}
				}
			}

		private async Task DeleteVideo(BcVideoModel video)
			{
			string query = @$"DELETE FROM BCVideos WHERE `Key`='{video.Key}'";
			using (dbCon = new StaffordMySQLConnection())
				if (dbCon.Open())
					{
					using var cmd = dbCon.Connection.CreateCommand();
					cmd.CommandText = query;
					try
						{
						await cmd.ExecuteNonQueryAsync();
						}
					catch (Exception ex)
						{
						await messageService.ShowErrorAsync($"Delete failed: {ex.Innermost().Message}", Application.Current.MainWindow.Title);
						}
					}
			}

		private async Task UpdateVideoAsync(BcVideoModel video)
			{
			string query = @$"UPDATE BCVideos SET `Date`=@date,`Title`=@title,`Speaker`=@speaker,`Ecclesia`=@ecclesia,`Link`=@link WHERE `Key`='{video.Key}'";
			using (dbCon = new StaffordMySQLConnection())
				if (dbCon.Open())
					{
					using var cmd = dbCon.Connection.CreateCommand();
					cmd.CommandText = query;
					cmd.Parameters.AddWithValue("@key", video.Key);
					cmd.Parameters.AddWithValue("@date", new MySqlDateTime(video.Date));
					cmd.Parameters.AddWithValue("@title", video.Title);
					cmd.Parameters.AddWithValue("@speaker", video.Speaker);
					cmd.Parameters.AddWithValue("@ecclesia", video.Ecclesia);
					cmd.Parameters.AddWithValue("@link", video.Link);
					try
						{
						await cmd.ExecuteNonQueryAsync();
						}
					catch (Exception ex)
						{
						await messageService.ShowErrorAsync($"Update failed: {ex.Innermost().Message}", Application.Current.MainWindow.Title);
						}
					}
			}

		private void GetVideosFromExcel()
			{
			FileInfo fi = new FileInfo(excelFilePath);
			if (!fi.Exists)
				{
				MessageBox.Show($"Excel file {excelFilePath} not found", Application.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
				}
			using (ExcelPackage ep = new ExcelPackage(fi))
				{
				ExcelWorksheet? ws = (from w in ep.Workbook.Worksheets where w.Name == "BC Videos" select w).FirstOrDefault();
				if (ws is null)
					{
					MessageBox.Show($"Worksheet 'BC Videos' not found in {excelFilePath}", Application.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
					}

				int row = 2;

				while (ws.Cells[row, 1].Value != null)
					{
					string title = string.Empty;
					string speaker = string.Empty;
					string ecclesia = string.Empty;
					string link = string.Empty;

					var date = ws.Cells[row, 1].GetValue<DateTime>();

					if (ws.Cells[row, 2].Value != null)
						title = ws.Cells[row, 2].Value.ToString()!;

					if (ws.Cells[row, 3].Value != null)
						speaker = ws.Cells[row, 3].Value.ToString()!;

					if (ws.Cells[row, 4].Value != null)
						ecclesia = ws.Cells[row, 4].Value.ToString()!;

					if (ws.Cells[row, 5].Value != null)
						link = ws.Cells[row, 5].Value.ToString()!;

					var video = new BcVideoModel()
						{
						Title = title,
						Link = link,
						Date = date,
						Speaker = speaker,
						Ecclesia = ecclesia
						};
					Videos!.Add(video);
					row++;
					}
				}
			//Gallery.ResetChanges();
			}

		private async Task<bool> SaveVideosToExcelAsync()
			{
			FileInfo excelFile;

			try
				{
				excelFile = new FileInfo(excelFilePath);
				excelFile.CopyTo(excelFilePath + ".bak", true);
				excelFile = new FileInfo(excelFilePath);
				}
			catch (IOException ex)
				{
				await messageService.ShowErrorAsync(ex.Message, Application.Current.MainWindow.Title);
				return false;
				}

			try
				{
				using (ExcelPackage ep = new ExcelPackage(excelFile))
					{
					ExcelWorksheet? ws = (from w in ep.Workbook.Worksheets where w.Name == "BC Videos" select w).FirstOrDefault();
					if (ws != null)
						{
						ws.Cells[1, 1, 202, 9].Clear();
						}
					else
						{
						ws = ep.Workbook.Worksheets.Add("BC Videos");
						}

					ws.Cells[1, 1].Value = "Date";
					ws.Column(1).Style.Numberformat.Format = @"[$-809]dd\ mmmm\ yyyy;@";
					ws.Column(1).Width = 18;
					ws.Column(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

					ws.Cells[1, 2].Value = "Title";
					ws.Column(2).Width = 60;

					ws.Cells[1, 3].Value = "Speaker";
					ws.Column(3).Width = 18;

					ws.Cells[1, 4].Value = "Ecclesia";
					ws.Column(4).Width = 24;

					ws.Cells[1, 5].Value = "Url";
					ws.Column(5).Width = 45;

					ws.Cells[1, 1, 1, 5].Style.Font.Bold = true;

					int row = 2;
					foreach (var video in Gallery.Videos.OrderBy(v => v.Date))
						{
						ws.Cells[row, 1].Value = video.Date;
						ws.Cells[row, 2].Value = video.Title;
						ws.Cells[row, 3].Value = video.Speaker;
						ws.Cells[row, 4].Value = video.Ecclesia;
						ws.Cells[row, 5].Value = video.Link;

						row++;
						}

					ep.Workbook.Properties.Company = "Real World Software";
					ep.Workbook.Properties.Comments = "Stafford ecclesia Bible Class videos";

					ep.Save();
					}
				await messageService.ShowAsync($"Video list saved to {excelFilePath}", Application.Current.MainWindow.Title, MessageButton.OK, MessageImage.Information);
				return true;
				}
			catch (Exception ex)
				{
				await messageService.ShowErrorAsync(ex.Message, Application.Current.MainWindow.Title);
				return false;
				}
			}
		}
	}
