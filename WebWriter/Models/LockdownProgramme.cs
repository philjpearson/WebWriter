//
//	Last mod:	10 November 2025 17:14:25
//
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using CsvHelper;
using OfficeOpenXml;
using WebWriter.Documents;

namespace WebWriter.Models
	{
	public class LockdownProgramme
		{
		private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

		public string ExcelFilePath { get; set; } = @"C:\Users\Phil\OneDrive\My Documents\Ecclesia\Programme\Ecclesial Programme.xlsm";

		public List<LockdownProgrammeItem>? Programme { get; set; }

		public List<LockdownProgrammeItem> Sunday
			{
			get
				{
				return Programme!.Where(p => p.Type == 1).ToList();
				}
			}

		public List<LockdownProgrammeItem> BibleClass
			{
			get
				{
				return Programme!.Where(p => p.Type == 3).ToList();
				}
			}

		public static LockdownProgramme Load(string filePath)
			{
			var prog = new LockdownProgramme();

			using (var reader = new StreamReader(filePath))
			using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
				{
				prog.Programme = csv.GetRecords<LockdownProgrammeItem>().ToList();
				prog.GetCollections();
				}
			return prog;
			}

		private bool GetCollections()
			{
			bool result = false;
			FileInfo excelFile;

			try
				{
				excelFile = new FileInfo(ExcelFilePath);
				}
			catch (IOException ex)
				{
				MessageBox.Show(ex.Message, Application.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
				}

			ExcelPackage.License.SetNonCommercialPersonal("Stafford Christadelphians");
			using var ep = new ExcelPackage(excelFile);
			try
				{
				ExcelWorksheet? ws;
				int startYear = DateTime.Now.Year;

#if false
				ws = (from w in ep.Workbook.Worksheets where w.Name == "2024" select w).FirstOrDefault();
				if (ws != null)
					{
					int row = 4;
					while (ws.Cells[row, 2].Value is DateTime dt && dt.Year == 2024)
						{
						var sunday = Programme!.Where(p => p.Date.Date == dt.Date).FirstOrDefault();
						if (sunday != null)
							{
							sunday.Collection2 = ws.Cells[row, 21].Value as string;
							sunday.Collection3 = ws.Cells[row, 22].Value as string;
							}
						row++;
						}
					}
#endif
				ws = (from w in ep.Workbook.Worksheets where w.Name == startYear.ToString() select w).FirstOrDefault();
				if (ws != null)
					{
					int row = 4;

					while (ws.Cells[row, 2].Value is DateTime dt && dt.Year == startYear)
						{
						var sunday = Programme!.Where(p => p.Date.Date == dt.Date).FirstOrDefault();
						if (sunday != null)
							{
							sunday.Collection2 = ws.Cells[row, 21].Value as string;
							sunday.Collection3 = ws.Cells[row, 22].Value as string;
							}
						row++;
						}
					}
				startYear++;
				ws = (from w in ep.Workbook.Worksheets where w.Name == startYear.ToString() select w).FirstOrDefault();
				if (ws != null)
					{
					int row = 4;

					while (ws.Cells[row, 2].Value is DateTime dt && dt.Year == startYear)
						{
						var sunday = Programme!.Where(p => p.Date.Date == dt.Date).FirstOrDefault();
						if (sunday != null)
							{
							sunday.Collection2 = ws.Cells[row, 21].Value as string;
							sunday.Collection3 = ws.Cells[row, 22].Value as string;
							}
						row++;
						}
					}
				result = true;
				}
			catch (Exception ex)
				{
				logger.Error("Error getting collections: {0}", ex.Message);
				MessageBox.Show($"Error getting collections: {ex.Message}", Application.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);
				}

			return result;
			}

		public bool CreatePdf(string filePath, DateTime startDate)
			{
			bool result = true;

			try
				{
				var doc = new ProgrammeDocument(this, startDate);
				doc.CreatePDF(filePath);
				result = true;
				}
			catch (Exception ex)
				{
				logger.Error("Saving to PDF failed: {0}", ex.Message);
				MessageBox.Show($"Saving to PDF failed: {ex.Message}");
				result = false;
				}
			return result;
			}
		}
	}
