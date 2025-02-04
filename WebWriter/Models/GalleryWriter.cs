//
//	Last mod:	04 February 2025 11:21:36
//
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Xml;

namespace WebWriter.Models
	{
	public class GalleryWriter
		{
		private GalleryModel gallery;
		private XmlDocument? newDoc;
		private const string galleryPage = @"D:\Documents\My Web Sites\StaffordChristadelphians\gallery\index.html";
		private const string newGalleryPage = @"D:\Documents\My Web Sites\StaffordChristadelphians\gallery\index-new.html";

		const string ftpUserName = "u880159079.staffordchristadelphians.org.uk"; // "1007246_code";
		const string ftpPassword = "z6ZjgSn4tfkM_nD"; // "7wtk3Es1zthmBBHWdPyY";

		string galleryFilename { get; } = "Gallery.webw";

		public GalleryWriter(GalleryModel gallery)
			{
			this.gallery = gallery;
			}

		/// <summary>
		/// Write a div with id="BibleHourVideos" containing a div for each video to be published
		/// </summary>
		/// <param name="filename">The name of the original xml file to write</param>
		public void Write(string filename)
			{
			// create a new div for the videos
			// get the document from the design folder
			// replace the original div with the new one
			// write out the modified doc to the new path

			newDoc = new XmlDocument();
			XmlNode rootDiv = newDoc.CreateElement("div");
			XmlAttribute attr = newDoc.CreateAttribute("id");
			attr.Value = "BibleHourVideos";
			rootDiv.Attributes!.Append(attr);
			newDoc.AppendChild(rootDiv);

			foreach (var video in gallery.Videos.Where(v => v.Publish && v.IsBibleHour && !string.IsNullOrWhiteSpace(v.Link)))
				{
				WriteVideo(rootDiv, video);
				}
			newDoc.Save(filename);

			try
				{
				XmlDocument doc = new XmlDocument();
				doc.Load(galleryPage);
				XmlNode? vidDiv = doc.SelectSingleNode("//div[@id='BibleHourVideos']");
				XmlNode newDiv = doc.ImportNode(rootDiv, true);
				vidDiv?.ParentNode?.ReplaceChild(newDiv, vidDiv);
				doc.Save(newGalleryPage);
				MessageBox.Show(string.Format("New gallery index page written to {0}", newGalleryPage), "Web Writer");
				}
			catch (Exception ex)
				{
				MessageBox.Show(string.Format("Error writing new gallery index page: {0}", ex.Message), "Web Writer", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}

		// aiming to write something like this:
		/*
		 *	<div id="apostles">
		 *		<div class="col-md-4">
		 * 			<h4>The message of the 1st century apostles</h4>
		 * 			<p>Our Bible Hour presentation on 21 September 2014</p>
		 * 			<iframe width="640" height="360" src="//www.youtube.com/embed/rbCmozQUjRE?rel=0" frameborder="0" allowfullscreen class="video640"></iframe>
		 * 			<p>The speaker is Paul Newman from the Pershore ecclesia.</p>
		 * 		</div>
		 * 	</div>
		 *
		 * OR
		 *	<div id="apostles">
		 *		<div class="col-md-4">
		 * 			<iframe width="160" height="90" src="//www.youtube.com/embed/rbCmozQUjRE?rel=0" frameborder="0" allowfullscreen class="video-thumbnail"></iframe>
		 * 		</div>
		 * 		<div class="col-md-8">
		 * 			<h4>The message of the 1st century apostles</h4>
		 * 			<p>Our Bible Hour presentation on 21 September 2014</p>
		 * 			<p>The speaker is Paul Newman from the Pershore ecclesia.</p>
		 * 		</div>
		 * 	</div>
		 *
		*/

		private void WriteVideo(XmlNode root, VideoModel video)
			{
			XmlElement div = newDoc!.CreateElement("div");
			if (!string.IsNullOrWhiteSpace(video.Tag))
				{
				AppendAttribute(div, "id", video.Tag);
				AppendAttribute(div, "class", "row");
				}

			XmlElement coldiv = newDoc.CreateElement("div");
			Size size = video.GetSize();

			if (size.Width > 160)
				{
				AppendAttribute(coldiv, "class", "col-md-12");
				WriteVideoTitle(video, coldiv);
				WriteSubhead(video, coldiv);
				WriteVideoIframe(video, coldiv);
				WriteDetails(video, coldiv);
				div.AppendChild(coldiv);
				}
			else
				{
				AppendAttribute(coldiv, "class", "col-md-3");
				WriteVideoIframe(video, coldiv);
				div.AppendChild(coldiv);
				coldiv = newDoc.CreateElement("div");
				AppendAttribute(coldiv, "class", "col-md-9");
				WriteVideoTitle(video, coldiv);
				WriteSubhead(video, coldiv);
				WriteDetails(video, coldiv);
				div.AppendChild(coldiv);
				}
			root.AppendChild(div);
			}

		private void WriteDetails(VideoModel video, XmlElement coldiv)
			{
			if (!string.IsNullOrWhiteSpace(video.Details))
				{
				var p = newDoc!.CreateElement("p");
				p.InnerText = video.Details;
				coldiv.AppendChild(p);
				}

			if (!string.IsNullOrWhiteSpace(video.Speaker))
				{
				var p = newDoc!.CreateElement("p");
				p.InnerText = string.Format("The speaker is {0}", video.Speaker);
				if (!string.IsNullOrWhiteSpace(video.Ecclesia))
					{
					p.InnerText += string.Format(" from the {0} ecclesia", video.Ecclesia);
					}
				coldiv.AppendChild(p);
				}
			}

		private void WriteSubhead(VideoModel video, XmlElement coldiv)
			{
			if (video.IsBibleHour)
				{
				var p = newDoc!.CreateElement("p");
				p.InnerText = string.Format("Our Bible Hour presentation on {0:d MMMM yyyy}", video.Date);
				coldiv.AppendChild(p);
				}
			}

		private void WriteVideoIframe(VideoModel video, XmlElement coldiv)
			{
			Size size = video.GetSize();
			var iframe = newDoc!.CreateElement("iframe");
			///////// doesn't work! ////////// iframe.IsEmpty = false; // ensure it gets a closing tag (self-closed iframe doesn't work)
			iframe.InnerText = " ";	///////////// try this
			AppendAttribute(iframe, "width", size.Width.ToString());
			AppendAttribute(iframe, "height", size.Height.ToString());
			AppendAttribute(iframe, "style", "border:0;");
			AppendAttribute(iframe, "allowfullscreen", null!);
			if (size.Width <= 160)
				{
				AppendAttribute(iframe, "class", "video-thumbnail");
				}
			else
				{
				AppendAttribute(iframe, "class", "video640");
				}
			AppendAttribute(iframe, "src", GetSource(video));
			coldiv.AppendChild(iframe);
			}

		private void WriteVideoTitle(VideoModel video, XmlElement coldiv)
			{
			var h4 = newDoc!.CreateElement("h4");
			h4.InnerText = video.Title;
			coldiv.AppendChild(h4);
			}

		private string GetSource(VideoModel video)
			{
			int ind = video.Link.LastIndexOf('/');
			string code = video.Link.Substring(ind + 1);
			return string.Format("//www.youtube.com/embed/{0}?rel=0", code);
			}

		internal void WriteGalleryFile(string filePath)
			{
			try
				{
				FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://staffordchristadelphians.org.uk/public_html/gallery/{galleryFilename}.new");
				request.Method = WebRequestMethods.Ftp.UploadFile;
				request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);

				// Copy the contents of the file to the request stream.
				using (StreamReader sourceStream = new StreamReader(filePath))
					{
					byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
					sourceStream.Close();
					request.ContentLength = fileContents.Length;

					Stream requestStream = request.GetRequestStream();
					requestStream.Write(fileContents, 0, fileContents.Length);
					requestStream.Close();
					}
				FtpWebResponse response = (FtpWebResponse)request.GetResponse();
				if (response.StatusCode != FtpStatusCode.CommandOK && response.StatusCode != FtpStatusCode.ClosingData)
					{
					MessageBox.Show($"Gallery file ftp upload failed.\r\nResponse status was '{response.StatusDescription}'", "WebWriter", MessageBoxButton.OK, MessageBoxImage.Exclamation);
					}
				response.Close();

				request = (FtpWebRequest)WebRequest.Create($"ftp://staffordchristadelphians.org.uk/public_html/gallery/{galleryFilename}.new");
				request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
				request.Method = WebRequestMethods.Ftp.Rename;
				request.RenameTo = galleryFilename;
				response = (FtpWebResponse)request.GetResponse();
				if (response.StatusCode != FtpStatusCode.FileActionOK)
					{
					MessageBox.Show($"Gallery file ftp rename after upload failed.\r\nResponse status was '{response.StatusDescription}'", "WebWriter", MessageBoxButton.OK, MessageBoxImage.Exclamation);
					}
				response.Close();
				MessageBox.Show("Gallery file written to the server", "Web Writer");
				}
			catch (Exception ex)
				{
				MessageBox.Show($"Exception in WriteGallery: {ex.Message}'", "WebWriter", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				}
			}

		private void AppendAttribute(XmlNode node, string name, string value)
			{
			XmlAttribute a = newDoc!.CreateAttribute(name);
			if (value != null)
				a.Value = value;
			node.Attributes!.Append(a);
			}
		}
	}
