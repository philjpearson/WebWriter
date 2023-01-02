//
//	Last mod:	02 January 2023 11:58:19
//

using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.IO;
using MigraDoc.Rendering;
using PdfSharp;
using PdfSharp.Drawing;

namespace WebWriter.Documents
	{
	/// <summary>
	/// Represents a specialized System.Drawing.Printing.PrintDocument for MigraDoc documents.
	/// This component knows about MigraDoc and simplifies printing of MigraDoc documents.
	/// </summary>
	public class MigraDocPrintDocument : PrintDocument
		{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:MigraDoc.Rendering.Printing.MigraDocPrintDocument"/> class. 
		/// </summary>
		public MigraDocPrintDocument()
			{
			DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
			OriginAtMargins = false;
			}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:MigraDoc.Rendering.Printing.MigraDocPrintDocument"/> class
		/// with the specified <see cref="T:MigraDoc.Rendering.DocumentRenderer"/> object.
		/// </summary>
		public MigraDocPrintDocument(DocumentRenderer renderer)
			{
			this.renderer = renderer;
			DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
			OriginAtMargins = false;
			}

		public MigraDocPrintDocument(Document document)
				: this()
			{
			renderer = MakeRenderer(document);
			}

		public MigraDocPrintDocument(string ddl)
				: this()
			{
			var document = DdlReader.DocumentFromString(ddl);
			renderer = MakeRenderer(document);
			}

		private static DocumentRenderer MakeRenderer(Document document)
			{
			DocumentRenderer renderer = new DocumentRenderer(document);
			renderer.PrepareDocument();
			return renderer;
			}

		/// <summary>
		/// Gets or sets the DocumentRenderer that prints the pages of the document.
		/// </summary>
		public DocumentRenderer Renderer
			{
			get { return renderer; }
			set { renderer = value; }
			}
		DocumentRenderer renderer;

		/// <summary>
		/// Gets or sets the page number that identifies the selected page. It it used on printing when 
		/// PrintRange.Selection is set.
		/// </summary>
		public int SelectedPage
			{
			get { return selectedPage; }
			set { selectedPage = value; }
			}
		int selectedPage;

		/// <summary>
		/// Raises the <see cref="E:System.Drawing.Printing.PrintDocument.BeginPrint"/> event. It is called after the <see cref="M:System.Drawing.Printing.PrintDocument.Print"/> method is called and before the first page of the document prints.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Drawing.Printing.PrintEventArgs"/> that contains the event data.</param>
		protected override void OnBeginPrint(PrintEventArgs e)
			{
			if (renderer == null)
				throw new Exception("Cannot print without a MigraDoc.Rendering.DocumentRenderer.");

			base.OnBeginPrint(e);
			if (!e.Cancel)
				{
				switch (PrinterSettings.PrintRange)
					{
				case PrintRange.AllPages:
					pageNumber = 1;
					pageCount = renderer.FormattedDocument.PageCount;
					break;

				case PrintRange.SomePages:
					pageNumber = PrinterSettings.FromPage;
					pageCount = PrinterSettings.ToPage - PrinterSettings.FromPage + 1;
					break;

				case PrintRange.Selection:
					pageNumber = selectedPage;
					pageCount = 1;
					break;

				default:
					// Debug.Assert(false, "Invalid PrinterRange.");
					e.Cancel = true;
					break;
					}
				}
			}
		int pageNumber = -1;
		int pageCount;

		/// <summary>
		/// Raises the <see cref="E:System.Drawing.Printing.PrintDocument.QueryPageSettings"/> event. It is called immediately before each <see cref="E:System.Drawing.Printing.PrintDocument.PrintPage"/> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Drawing.Printing.QueryPageSettingsEventArgs"/> that contains the event data.</param>
		protected override void OnQueryPageSettings(QueryPageSettingsEventArgs e)
			{
			base.OnQueryPageSettings(e);
			if (!e.Cancel)
				{
				PageSettings settings = e.PageSettings;
				PageInfo pageInfo = renderer.FormattedDocument.GetPageInfo(pageNumber);

				// set portrait/landscape
				settings.Landscape = pageInfo.Orientation == PageOrientation.Landscape;
				}
			}

		/// <summary>
		/// Raises the <see cref="E:System.Drawing.Printing.PrintDocument.PrintPage"/> event. It is called before a page prints.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Drawing.Printing.PrintPageEventArgs"/> that contains the event data.</param>
		protected override void OnPrintPage(PrintPageEventArgs e)
			{
			base.OnPrintPage(e);
			if (!e.Cancel)
				{
				PageSettings settings = e.PageSettings;
				try
					{
					Graphics graphics = e.Graphics;
					IntPtr hdc = graphics.GetHdc();
					int xOffset = GetDeviceCaps(hdc, PHYSICALOFFSETX);
					int yOffset = GetDeviceCaps(hdc, PHYSICALOFFSETY);
					graphics.ReleaseHdc(hdc);
					graphics.TranslateTransform(-xOffset * 100 / graphics.DpiX, -yOffset * 100 / graphics.DpiY);
					// Recall: Width and Height are exchanged when settings.Landscape is true.
					XSize size = new XSize(e.PageSettings.Bounds.Width / 100.0 * 72, e.PageSettings.Bounds.Height / 100.0 * 72);
					const float scale = 100f / 72f;
					graphics.ScaleTransform(scale, scale);
					// draw line A4 portrait
					//graphics.DrawLine(Pens.Red, 0, 0, 21f / 2.54f * 72, 29.7f / 2.54f * 72);
#if WPF
//#warning TODO WPFPDF
// TODO WPFPDF
#else
					XGraphics gfx = XGraphics.FromGraphics(graphics, size);
					renderer.RenderPage(gfx, pageNumber);
#endif
					}
				catch
					{
					e.Cancel = true;
					}
				pageNumber++;
				pageCount--;
				e.HasMorePages = pageCount > 0;
				}
			}

		/// <summary>
		/// Raises the <see cref="E:System.Drawing.Printing.PrintDocument.EndPrint"/> event. It is called when the last page of the document has printed.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Drawing.Printing.PrintEventArgs"/> that contains the event data.</param>
		protected override void OnEndPrint(PrintEventArgs e)
			{
			base.OnEndPrint(e);
			pageNumber = -1;
			}

		[DllImport("gdi32.dll")]
		static extern int GetDeviceCaps(IntPtr hdc, int capability);
		// ReSharper disable InconsistentNaming
		const int PHYSICALOFFSETX = 112; // Physical Printable Area x margin
		const int PHYSICALOFFSETY = 113; // Physical Printable Area y margin
																		 // ReSharper restore InconsistentNaming
		}

	/// <summary>
	/// MigraDocPrintDocumentEx does not use any MigraDoc classes in the interface.
	/// This allows consuming the class MigraDocPrintDocumentEx without referencing the GDI build of PDFsharp/MigraDoc.
	/// This allows assemblies that reference the WPF build or other builds of PDFsharp/MigraDoc to use this class for printing.
	/// To make this work, we have to pass the MigraDoc document as an MDDDL string.
	/// </summary>
	public class MigraDocPrintDocumentEx
		{
		public MigraDocPrintDocumentEx(string ddl)
			{
			printDocument = new MigraDocPrintDocument(ddl);
			}

		public PrinterSettings PrinterSettings
			{
			get { return printDocument.PrinterSettings; }
			set { printDocument.PrinterSettings = value; }
			}

		public void Print()
			{
			printDocument.Print();
			}

		private readonly MigraDocPrintDocument printDocument;
		}
	}
