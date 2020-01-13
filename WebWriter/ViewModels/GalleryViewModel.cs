//
//	Last mod:	11 November 2019 11:35:03
//
namespace WebWriter.ViewModels
	{
	using Catel;
	using Catel.Data;
	using Catel.Fody;
	using Catel.MVVM;
	using Catel.Services;
	using HtmlAgilityPack;
	using OfficeOpenXml;
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Data;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Net.Http;
	using System.Net.Http.Headers;
	using System.Text;
	using System.Threading.Tasks;
	using System.Windows;
	using WebWriter.Models;

	public class GalleryViewModel : ViewModelBase
		{
		const string filePath = @"D:\Users\philj\OneDrive\My Documents\Ecclesia\Web site\Gallery.webw";
		const string excelFilePath = @"D:\Users\philj\OneDrive\My Documents\Ecclesia\Stafford videos.xlsx";
		private readonly HttpClient httpClient;

		private readonly IMessageService messageService;

		public GalleryViewModel(IMessageService messageService)
			{
			Argument.IsNotNull(() => messageService);
			this.messageService = messageService;

			OkCommand = new Command<object>(OnOkCommandExecute);
			ImportCommand = new Command<object>(OnImportExecute);
			ExportCommand = new TaskCommand<object>(OnExportExecuteAsync);
			SortCommand = new Command(OnSortCommandExecute);

			Gallery = new GalleryModel();

			httpClient = new HttpClient();
			var byteArray = Encoding.UTF8.GetBytes("phil:stafford54%");
			var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
			httpClient.DefaultRequestHeaders.Authorization = header;
			}

		public override string Title { get { return "Video Gallery View"; } }

		// TODO: Register models with the vmpropmodel codesnippet
		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		[Model]
		[Expose("Videos")]
		public GalleryModel Gallery
			{
			get { return GetValue<GalleryModel>(GalleryProperty); }
			private set { SetValue(GalleryProperty, value); }
			}

		/// <summary>
		/// Register the Gallery property so it is known in the class.
		/// </summary>
		public static readonly PropertyData GalleryProperty = RegisterProperty("Gallery", typeof(GalleryModel));

		// TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets

		[ViewModelToModel]
		public ObservableCollection<VideoModel> Videos { get; set; }

		// TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets

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
		/// Gets the ImportCommand command.
		/// </summary>
		public Command<object> ImportCommand { get; private set; }

		/// <summary>
		/// Method to invoke when the ImportCommand command is executed.
		/// </summary>
		private void OnImportExecute()
			{
			FileInfo fi = new FileInfo(excelFilePath);
			using (ExcelPackage ep = new ExcelPackage(fi))
				{
				ExcelWorksheet ws = ep.Workbook.Worksheets[1];
				int row = 2;

				while (ws.Cells[row, 1].Value != null)
					{
					bool isBibleHour = false;
					string title = string.Empty;
					string speaker = string.Empty;
					string ecclesia = string.Empty;
					string link = string.Empty;

					DateTime date = DateTime.FromOADate((double)ws.Cells[row, 1].Value);

					if (ws.Cells[row, 2].Value != null)
						isBibleHour = ws.Cells[row, 2].Value.ToString() == "Bible Hour";

					if (ws.Cells[row, 3].Value != null)
						title = ws.Cells[row, 3].Value.ToString();

					if (ws.Cells[row, 4].Value != null)
						{
						speaker = ws.Cells[row, 4].Value.ToString();
						int ind = speaker.IndexOf('(');
						if (ind != -1)
							{
							ecclesia = speaker.Substring(ind + 1);
							ecclesia = ecclesia.Replace(')', ' ').Trim();
							speaker = speaker.Substring(0, ind - 1).Trim();
							}
						}

					if (ws.Cells[row, 5].Value != null)
						link = ws.Cells[row, 5].Value.ToString();

					var video = new VideoModel()
						{
						Title = title,
						Link = link,
						Date = date,
						IsBibleHour = isBibleHour,
						Speaker = speaker,
						Ecclesia = ecclesia
						};
					Videos.Add(video);
					row++;
					}
				}
			}

		/// <summary>
		/// Gets the ExportCommand command.
		/// </summary>
		public TaskCommand<object> ExportCommand { get; private set; }

		/// <summary>
		/// Method to invoke when the ExportCommand command is executed.
		/// </summary>
		private Task OnExportExecuteAsync(object param)
			{
			return ExportToExcelAsync();
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
			if (Gallery != null)
				{
				Gallery.Sort();
				}
			}

		protected override async Task InitializeAsync()
			{
			await base.InitializeAsync();

			try
				{
				var response = await httpClient.GetAsync("https://staffordchristadelphians.org.uk/manage/index.php");
				if (response.StatusCode != HttpStatusCode.OK)
					await messageService.ShowWarningAsync($"Reading videos table failed: {response.ReasonPhrase}", Application.Current.MainWindow.Title);
				else
					{
					var html = await response.Content.ReadAsStringAsync();
					var doc = new HtmlDocument();
					doc.LoadHtml(html);
					var table = doc.DocumentNode.SelectSingleNode("//table[@class='videosTable']")
											.Descendants("tr")
											.Where(tr => tr.Elements("td").Count() > 1)
											.Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
											.ToList();
					Videos.Clear();

					foreach (var row in table)
						{
						Videos.Add(new VideoModel()
							{ Id = int.Parse(row[0]), Date = DateTime.Parse(row[1]), Title = row[2], Tag = row[3], Link = row[4], Speaker = row[5], Ecclesia = row[6], Details = row[7] });
						}
					Gallery.ResetChanges();
					}
				}
			catch (Exception ex)
				{
				await messageService.ShowErrorAsync(ex.Message);
				}
			// TODO: subscribe to events here
			}

		protected override async Task<bool> SaveAsync()
			{
			if (Videos.Any(v => v.HasDuplicateTag))
				{
				var answer = await messageService.ShowAsync("Save with duplicate tags?", Application.Current.MainWindow.Title, MessageButton.YesNo, MessageImage.Question);
				if (answer == MessageResult.No)
					return false;
				}
			try
				{
				var changes = Gallery.GetChanges();
				foreach (var change in changes)
					{
					switch (change.Value)
						{
						case GalleryModel.ChangeType.Edit:
							await UpdateVideoAsync(change.Key);
							break;

						case GalleryModel.ChangeType.Add:
							await AddVideo(change.Key);
							break;

						case GalleryModel.ChangeType.Delete:
							await DeleteVideo(change.Key);
							break;

						case GalleryModel.ChangeType.None:
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
			return await base.SaveAsync();
			}

		private async Task AddVideo(VideoModel video)
			{
			var values = new Dictionary<string, string>
				{
					{ "PhilsWebWriter", "yes" },
					{ "new", "1" },
					{ "date", video.Date.ToString("yyyy-MM-dd") },
					{ "title", video.Title },
					{ "tag", video.Tag },
					{ "link", video.Link },
					{ "speaker", video.Speaker },
					{ "ecclesia", video.Ecclesia },
					{ "details", video.Details }
				};

			var result = await PostWebRequest("insert", values);
			if (result.statusCode != HttpStatusCode.OK)
				await messageService.ShowWarningAsync($"Insert failed: {result.reasonPhrase}", Application.Current.MainWindow.Title);
			}

		private async Task DeleteVideo(VideoModel video)
			{
			var values = new Dictionary<string, string>
				{
					{ "PhilsWebWriter", "yes" },
					{ "id", video.Id.ToString() }
				};

			var result = await PostWebRequest("delete", values);
			if (result.statusCode != HttpStatusCode.OK)
				await messageService.ShowWarningAsync($"Delete failed: {result.reasonPhrase}", Application.Current.MainWindow.Title);
			}

		private async Task UpdateVideoAsync(VideoModel video)
			{
			var values = new Dictionary<string, string>
				{
					{ "PhilsWebWriter", "yes" },
					{ "new", "1" },
					{ "id", video.Id.ToString() },
					{ "date", video.Date.ToString("yyyy-MM-dd") },
					{ "title", video.Title },
					{ "tag", video.Tag },
					{ "link", video.Link },
					{ "speaker", video.Speaker },
					{ "ecclesia", video.Ecclesia },
					{ "details", video.Details }
				};

			var result = await PostWebRequest("edit", values);
			if (result.statusCode != HttpStatusCode.OK)
				await messageService.ShowWarningAsync($"Update failed: {result.reasonPhrase}", Application.Current.MainWindow.Title);
			}

		private async Task<(HttpStatusCode statusCode, string reasonPhrase, string responseString)> PostWebRequest(string pageName, Dictionary<string, string> parameters)
			{
			var content = new FormUrlEncodedContent(parameters);
			var response = await httpClient.PostAsync($"https://staffordchristadelphians.org.uk/manage/{pageName}.php", content);
			var responseString = await response.Content.ReadAsStringAsync();
			return (response.StatusCode, response.ReasonPhrase, responseString);
			}

		protected override async Task<bool> CancelAsync()
			{
			if (Gallery.GetChanges().Count > 0)
				{
				var answer = await messageService.ShowAsync("Abandon changes?", Application.Current.MainWindow.Title, MessageButton.YesNo, MessageImage.Question);
				if (answer == MessageResult.No)
					return false;
				}
			return await base.CancelAsync();
			}

		protected override async Task CloseAsync()
			{
			// TODO: unsubscribe from events here

			httpClient.Dispose();
			await base.CloseAsync();
			}

		private async Task<bool> ExportToExcelAsync()
			{
			FileInfo excelFile;

			try
				{
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
					ExcelWorksheet ws = (from w in ep.Workbook.Worksheets where w.Name == "Videos" select w).FirstOrDefault();
					if (ws != null)
						{
						ws.Cells[1, 1, 202, 9].Clear();
						}
					else
						{
						ws = ep.Workbook.Worksheets.Add("Videos");
						}

					ws.Cells[1, 1].Value = "Date";
					ws.Column(1).Style.Numberformat.Format = @"[$-809]dd\ mmmm\ yyyy;@";
					ws.Column(1).Width = 18;
					ws.Column(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

					ws.Cells[1, 2].Value = "Type";
					ws.Column(2).Width = 10;

					ws.Cells[1, 3].Value = "Title";
					ws.Column(3).Width = 43;

					ws.Cells[1, 4].Value = "Size";
					ws.Column(4).Width = 10;

					ws.Cells[1, 5].Value = "Speaker";
					ws.Column(5).Width = 18;

					ws.Cells[1, 6].Value = "Ecclesia";
					ws.Column(6).Width = 24;

					ws.Cells[1, 7].Value = "Url";
					ws.Column(7).Width = 30;

					ws.Cells[1, 8].Value = "Publish";
					ws.Column(8).Width = 10;

					ws.Cells[1, 9].Value = "Details";
					ws.Column(9).Width = 50;

					ws.Cells[1, 10].Value = "Notes";
					ws.Column(10).Width = 50;

					ws.Cells[1, 11].Value = "Code";
					ws.Column(11).Width = 14;

					ws.Cells[1, 1, 1, 11].Style.Font.Bold = true;

					int row = 2;
					foreach (var video in Gallery.Videos.OrderBy(v => v.Date))
						{
						string size = video.Size.ToString();
						if (size.StartsWith("Size"))
							size = size.Substring(4);

						string code = string.Empty;
						if (!string.IsNullOrWhiteSpace(video.Link))
							code = video.Link.Substring(video.Link.LastIndexOf('/') + 1);

						ws.Cells[row, 1].Value = video.Date;
						ws.Cells[row, 2].Value = video.IsBibleHour ? "Bible Hour" : "Special";
						ws.Cells[row, 3].Value = video.Title;
						ws.Cells[row, 4].Value = size;
						ws.Cells[row, 5].Value = video.Speaker;
						ws.Cells[row, 6].Value = video.Ecclesia;
						ws.Cells[row, 7].Value = video.Link;
						ws.Cells[row, 8].Value = video.Publish ? "Yes" : "No";
						ws.Cells[row, 9].Value = video.Details;
						ws.Cells[row, 11].Value = code;

						row++;
						}

					ep.Workbook.Properties.Company = "Real World Software";
					ep.Workbook.Properties.Comments = "Stafford ecclesia web gallery videos";

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
