//
//	Last mod:	02 January 2023 15:57:53
//
using System;
using System.Drawing.Printing;
using System.Reflection;
using System.Windows.Forms;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.Rendering;

#nullable enable

namespace WebWriter.Documents
	{
	public abstract class DocumentBase
		{
		private string? suggestedFileName = null;

		PrinterSettings printerSettings = null!;

		public Document MigraDocument { get; private set; } = new Document();

		public DocumentBase(string title, string subtitle)
			{
			Title = title;
			Subtitle = subtitle;

			// by default, the resource for the header image is expected to be in the calling assembly
			ResourceAssembly = Assembly.GetCallingAssembly();
			}

		public string Title { get; set; } = "Document";

		public string Subtitle { get; set; } = "";

		public string Author { get; set; } = "Phil J Pearson";

		public string Comment { get; set; } = "";

		public string FooterLeftText { get; set; } = "";

		public string HeaderImageResourceName { get; set; } = "";

		public string SuggestedFileName { get { return suggestedFileName ??= GetSuggestedFileName(); } }

		protected Section? MainSection { get; set; }

		protected Paragraph? HeaderParagraph { get; set; }

		protected Paragraph? FooterParagraph { get; set; }

		protected Assembly ResourceAssembly { get; set; }

		public virtual void CreatePDF(string outputFileName)
			{
			try
				{
				CreateDocument();
				var pdfRenderer = new PdfDocumentRenderer(true) { Document = MigraDocument };
				pdfRenderer.RenderDocument();
				pdfRenderer.PdfDocument.Save(outputFileName);
				}
			catch (DocumentHandlingException)
				{
				throw;
				}
			catch (Exception ex)
				{
				var ex2 = ex.InnerException;
				while (ex2 != null)
					{
					ex2 = ex2.InnerException;
					}
				throw new DocumentHandlingException($"Error rendering document to PDF: {ex.Message}", ex);
				}
			}

		public DialogResult PrintSetup()
			{
			DialogResult result = DialogResult.None;

			printerSettings = new PrinterSettings();

			using (PrintDialog dialog = new PrintDialog())
				{
				dialog.PrinterSettings = printerSettings;
				dialog.AllowSelection = true;
				dialog.AllowSomePages = true;
				result = dialog.ShowDialog();
				if (result == DialogResult.OK)
					printerSettings = dialog.PrinterSettings;
				}
			return result;
			}

		public void Print(bool showSetupUI = true)
			{
			if (!showSetupUI || (PrintSetup() == DialogResult.OK))
				{
				try
					{
					if (printerSettings == null)
						printerSettings = new PrinterSettings();

					CreateDocument();
					using (var printDocument = new MigraDocPrintDocument(MigraDocument) { PrinterSettings = printerSettings })
						{
						printDocument.Print();
						}
					}
				catch (DocumentHandlingException)
					{
					throw;
					}
				catch (Exception ex)
					{
					var ex2 = ex.InnerException;
					while (ex2 != null)
						{
						ex2 = ex2.InnerException;
						}
					throw new DocumentHandlingException($"Error printing document: {ex.Message}", ex);
					}
				}
			}

		protected internal void CreateDocument()
			{
			try
				{
				MigraDocument.Info.Title = Title;
				MigraDocument.Info.Subject = Subtitle;
				MigraDocument.Info.Author = Author;
				MigraDocument.Info.Comment = Comment;

				DefineStyles();
				CreatePage();
				FillContent();
				}
			catch (Exception ex)
				{
				var ex2 = ex.InnerException;
				while (ex2 != null)
					{
					ex2 = ex2.InnerException;
					}
				throw new DocumentHandlingException($"Error creating document: {ex.Message}", ex);
				}
			}

		protected virtual void DefineStyles()
			{
			// Get the predefined style Normal.
			Style style = MigraDocument.Styles["Normal"];
			style.Font.Name = "GillSans";

			style = MigraDocument.Styles[StyleNames.Header];
			style.ParagraphFormat.AddTabStop("8cm", MigraDoc.DocumentObjectModel.TabAlignment.Center);
			style.Font.Size = 16;
			style.ParagraphFormat.Alignment = ParagraphAlignment.Center;

			style = MigraDocument.Styles[StyleNames.Footer];
			style.ParagraphFormat.AddTabStop("16cm", MigraDoc.DocumentObjectModel.TabAlignment.Right);

			// Create a new style called Table based on style Normal
			style = MigraDocument.Styles.AddStyle("Table", "Normal");
			style.Font.Size = 9;

			style = MigraDocument.Styles.AddStyle("Heading1", "Normal");
			style.Font.Size = 14;
			style.Font.Bold = true;
			}

		protected virtual void CreatePage()
			{
			// Each MigraDoc document needs at least one section.
			MainSection = MigraDocument.AddSection();
			MainSection.PageSetup.TopMargin = "3.5cm";
			MainSection.PageSetup.LeftMargin = "10mm";
			MainSection.PageSetup.RightMargin = "10mm";

			// Put a logo in the header
			if (!string.IsNullOrWhiteSpace(HeaderImageResourceName))
				{
				Image image = MainSection.Headers.Primary.AddImageFromResource(ResourceAssembly, HeaderImageResourceName);
				image.Height = "2cm";
				image.LockAspectRatio = true;
				image.RelativeVertical = RelativeVertical.Line;
				image.RelativeHorizontal = RelativeHorizontal.Margin;
				image.Top = ShapePosition.Top;
				image.Left = ShapePosition.Right;
				image.WrapFormat.Style = WrapStyle.Through;
				}
			HeaderParagraph = MainSection.Headers.Primary.AddParagraph(Title);
			HeaderParagraph.Format.Font.Size = 28;
			HeaderParagraph.Format.Font.Bold = true;
			HeaderParagraph = MainSection.Headers.Primary.AddParagraph(Subtitle);
			HeaderParagraph.Format.Font.Size = 12;

			// Create footer
			FooterParagraph = MainSection.Footers.Primary.AddParagraph();
			FooterParagraph.Format.Font.Size = 9;
			FooterParagraph.AddText(FooterLeftText);
			FooterParagraph.AddTab();
			FooterParagraph.AddText("Page ");
			FooterParagraph.AddPageField();
			FooterParagraph.AddText(" of ");
			FooterParagraph.AddNumPagesField();

			// Create the content
			CreateContent();
			}

		protected abstract void CreateContent();

		protected abstract void FillContent();

		protected virtual string GetSuggestedFileName() { return "Document.pdf"; }
		}
	}
