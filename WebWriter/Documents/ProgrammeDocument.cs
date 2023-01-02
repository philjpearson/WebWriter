//
//	Last mod:	02 January 2023 16:28:27
//
using System;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using WebWriter.Models;

namespace WebWriter.Documents
	{
	public class ProgrammeDocument : DocumentBase
		{
		Table table;
		Table bcTable;

		public ProgrammeDocument(LockdownProgramme programme)
			: base("Stafford Christadelphian Ecclesia", "Programme 2023 (God willing)")
			{
			Programme = programme;
			HeaderImageResourceName = "";
			FooterLeftText = $"Issued: {DateTime.Now:d MMMM yyyy}";
			}

		public Color BorderColour { get; set; } = Colors.Blue;

		public LockdownProgramme Programme { get; private set; }

		protected override string GetSuggestedFileName()
			{
			return "Ecclesial programme.pdf";
			}

		protected override void CreatePage()
			{
			base.CreatePage();
			}

		protected override void CreateContent()
			{
			MainSection.AddParagraph("Sundays", "Heading1");

			// Create the table
			table = MainSection.AddTable();
			table.Style = "Table";
			table.Borders.Width = 0;
			table.Rows.LeftIndent = 0;
			Column column = table.AddColumn("15mm");
			column.Format.Alignment = ParagraphAlignment.Right;

			foreach (double width in new double[] { 30.0, 30.0, 30.0, 40.0, 40.0 })
				{
				column = table.AddColumn($"{width}mm");
				column.Format.Alignment = ParagraphAlignment.Left;
				}

			MainSection.AddPageBreak();
			MainSection.AddParagraph("Bible Class", "Heading1");
			bcTable = MainSection.AddTable();
			bcTable.Style = "Table";
			bcTable.Borders.Width = 0;
			bcTable.Rows.LeftIndent = 0;
			column = bcTable.AddColumn("15mm");
			column.Format.Alignment = ParagraphAlignment.Right;

			foreach (double width in new double[] { 30.0, 30.0, 30.0, 80.0 })
				{
				column = bcTable.AddColumn($"{width}mm");
				column.Format.Alignment = ParagraphAlignment.Left;
				}

			}

		protected override void FillContent()
			{
			var headerRow = table.AddRow();
			headerRow.Format.Font.Bold = true;
			headerRow.Borders.Bottom.Width = "0.05cm";
			headerRow.Borders.Bottom.Color = BorderColour;
			headerRow.Borders.DistanceFromTop = headerRow.Borders.DistanceFromBottom = "0mm";
			headerRow.Format.SpaceBefore = "1.0mm";
			headerRow.Format.SpaceAfter = "0mm";
			int col = 0;
			foreach (string s in new string[] { "Date", "President", "Speaker", "Ecclesia", "2nd and 3rd Collections", "" })
				{
				var para = headerRow.Cells[col++].AddParagraph(s);
				}

			foreach (var sunday in Programme.Sunday)
				{
				var row = table.AddRow();
				var para = row.Cells[0].AddParagraph(sunday.Date.ToString("d MMM"));
				para.Format.Alignment = ParagraphAlignment.Right;

				string speaker = sunday.Speaker;
				string ecclesia = "";
				var i = speaker.IndexOf('(');
				if (i > 0)
					{
					ecclesia = speaker.Substring(i + 1, speaker.IndexOf(')') - (i + 1));
					speaker = speaker.Substring(0, i - 1);
					}
				row.Cells[1].AddParagraph(sunday.President);
				row.Cells[2].AddParagraph(speaker);
				row.Cells[3].AddParagraph(ecclesia);
				row.Cells[4].AddParagraph(sunday.Collection2 ?? "");
				row.Cells[5].AddParagraph(sunday.Collection3 ?? "");
				}

			headerRow = bcTable.AddRow();
			headerRow.Format.Font.Bold = true;
			headerRow.Borders.Bottom.Width = "0.05cm";
			headerRow.Borders.Bottom.Color = BorderColour;
			headerRow.Borders.DistanceFromTop = headerRow.Borders.DistanceFromBottom = "0mm";
			headerRow.Format.SpaceBefore = "1.0mm";
			headerRow.Format.SpaceAfter = "0mm";
			col = 0;
			foreach (string s in new string[] { "Date", "President", "Speaker", "Ecclesia", "Subject" })
				{
				var para = headerRow.Cells[col++].AddParagraph(s);
				}

			foreach (var bc in Programme.BibleClass)
				{
				var row = bcTable.AddRow();
				var para = row.Cells[0].AddParagraph(bc.Date.ToString("d MMM"));
				para.Format.Alignment = ParagraphAlignment.Right;

				string speaker = bc.Speaker;
				string ecclesia = "";
				var i = speaker.IndexOf('(');
				if (i > 0)
					{
					ecclesia = speaker.Substring(i + 1, speaker.IndexOf(')') - (i + 1));
					speaker = speaker.Substring(0, i - 1);
					}
				row.Cells[1].AddParagraph(bc.President);
				row.Cells[2].AddParagraph(speaker);
				row.Cells[3].AddParagraph(ecclesia);
				row.Cells[4].AddParagraph(bc.Information);
				}
			}
		}
	}
