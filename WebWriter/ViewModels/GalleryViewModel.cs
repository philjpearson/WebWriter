//
//	Last mod:	11 January 2017 15:30:36
//
namespace WebWriter.ViewModels
	{
	using Catel.MVVM;
	using System.Threading.Tasks;
	using WebWriter.Models;
	using Catel.Data;
	using System.Collections.ObjectModel;
	using System.IO;
	using System.Linq;
	using OfficeOpenXml;
	using System;
	using System.Windows;
	using MySql.Data.MySqlClient;
	using System.Data;

	public class GalleryViewModel : ViewModelBase
		{
		const string filePath = @"D:\philj\Documents\OneDrive\My Documents\Ecclesia\Web site\Gallery.webw";
		const string excelFilePath = @"D:\philj\Documents\OneDrive\My Documents\Ecclesia\Stafford videos.xlsx";

		StaffordMySQLConnection dbCon;
		MySqlDataAdapter daVideos;
		DataSet dsVideos;

		public GalleryViewModel()
			{
			OkCommand = new Command<object>(OnOkCommandExecute);
			ImportCommand = new Command<object>(OnImportExecute);
			ExportCommand = new Command<object>(OnExportExecute);
			SortCommand = new Command(OnSortCommandExecute);
			WriteXmlCommand = new Command<object>(OnWriteXmlCommandExecute);

			using (FileStream stream = new FileStream(filePath, FileMode.Open))
				{
				Gallery = GalleryModel.Load(stream, SerializationMode.Xml);
				}
			}

		public override string Title { get { return "Video Gallery View"; } }

		// TODO: Register models with the vmpropmodel codesnippet
		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		[Model]
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

		public DataView Videos { get; set; }

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
			await Save();
			Gallery.SaveAsXml(filePath);
			await CloseViewModel(true);
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

					VideoModel video = new VideoModel()
						{
						Title = title,
						Link = link,
						Date = date,
						IsBibleHour = isBibleHour,
						Speaker = speaker,
						Ecclesia = ecclesia
						};
					Gallery.Videos.Add(video);
					row++;
					}
				}
			}

		/// <summary>
		/// Gets the ExportCommand command.
		/// </summary>
		public Command<object> ExportCommand { get; private set; }

		/// <summary>
		/// Method to invoke when the ExportCommand command is executed.
		/// </summary>
		private void OnExportExecute()
			{
			ExportToExcel();
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

		/// <summary>
		/// Gets the WriteXmlCommand command.
		/// </summary>
		public Command<object> WriteXmlCommand { get; private set; }

		/// <summary>
		/// Method to invoke when the WriteXmlCommand command is executed.
		/// </summary>
		private void OnWriteXmlCommandExecute()
			{
			GalleryWriter gw = new GalleryWriter(Gallery);
			gw.Write(@"D:\Documents\Ecclesia\Web site\GalleryDiv.xml");
			}

		/// <summary>
		/// Gets the WriteGalleryCommand command.
		/// </summary>
		public Command<object> WriteGalleryCommand
			{
			get
				{
				if (_WriteGalleryCommand == null)
					_WriteGalleryCommand = new Command<object>(WriteGalleryCommand_Execute);
				return _WriteGalleryCommand;
				}
			}

		private Command<object> _WriteGalleryCommand;

		/// <summary>
		/// Method to invoke when the WriteGalleryCommand command is executed.
		/// </summary>
		/// <param name="parameter">The parameter of the command.</param>
		private void WriteGalleryCommand_Execute(object parameter)
			{
			Gallery.Sort();
			Gallery.SaveAsXml(filePath);
			GalleryWriter gw = new GalleryWriter(Gallery);
			gw.WriteGalleryFile(filePath);
			}


		protected override async Task Initialize()
			{
			await base.Initialize();

			try
				{
				dbCon = StaffordMySQLConnection.Instance();
				if (dbCon.IsConnect())
					{
					string query = "SELECT Id,Date,Title,Tag,Link,Details,Size,Speaker,Ecclesia,IsBibleHour,Publish FROM Videos ORDER BY Date";
					daVideos = new MySqlDataAdapter(query, dbCon.Connection);
					MySqlCommandBuilder cb = new MySqlCommandBuilder(daVideos);

					dsVideos = new DataSet();
					daVideos.Fill(dsVideos, "Videos");
					Videos = dsVideos.Tables["Videos"].DefaultView;
					}
				}
			catch (Exception ex)
				{
				MessageBox.Show(ex.Message);
				}
			// TODO: subscribe to events here
			}

		protected override Task<bool> Save()
			{
			if (dsVideos != null)
				{
				try
					{
					daVideos.Update(dsVideos, "Videos");
					dsVideos.Clear();
					daVideos.Fill(dsVideos, "Videos");
					}
				catch (Exception ex)
					{
					MessageBox.Show(ex.Message);
					}
				}

			return base.Save();
			}

		protected override async Task Close()
			{
			// TODO: unsubscribe from events here

			dbCon?.Close();
			await base.Close();
			}

		private bool ExportToExcel()
			{
			FileInfo excelFile;

			try
				{
				excelFile = new FileInfo(excelFilePath);
				}
			catch (IOException ex)
				{
				MessageBox.Show(ex.Message, Application.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);
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
				MessageBox.Show($"Video list saved to {excelFilePath}", Application.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Information);
				return true;
				}
			catch (Exception ex)
				{
				MessageBox.Show(ex.Message, Application.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
				}
			}
		}
	}
