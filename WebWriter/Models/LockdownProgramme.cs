//
//	Last mod:	02 January 2023 16:08:25
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
		public string ExcelFilePath { get; set; } = @"D:\Users\philj\OneDrive\My Documents\Ecclesia\Programme\Ecclesial Programme.xlsx";

		public List<LockdownProgrammeItem> Programme { get; set; }

		public List<LockdownProgrammeItem> Sunday
			{
			get
				{
				return Programme.Where(p => p.Type == 1).ToList();
				}
			}

		public List<LockdownProgrammeItem> BibleClass
			{
			get
				{
				return Programme.Where(p => p.Type == 3).ToList();
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

			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
			using var ep = new ExcelPackage(excelFile);
			try
				{
				ExcelWorksheet ws = (from w in ep.Workbook.Worksheets where w.Name == "2023" select w).FirstOrDefault();
				if (ws != null)
					{
					int row = 4;
					while (ws.Cells[row, 2].Value is DateTime dt && dt.Year == 2023)
						{
						var sunday = Programme.Where(p => p.Date.Date == dt.Date).FirstOrDefault();
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
				MessageBox.Show($"Error getting collections: {ex.Message}", Application.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);
				}

			return result;
			}

		public bool CreatePdf(string filePath)
			{
			bool result = true;

			try
				{
				var doc = new ProgrammeDocument(this);
				doc.CreatePDF(filePath);
				result = true;
				}
			catch (Exception ex)
				{
				MessageBox.Show($"Saving to PDF failed: {ex.Message}");
				}
			return result;
			}
		}
	}
