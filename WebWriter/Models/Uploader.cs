//
//	Last mod:	30 December 2016 19:07:22
//
using System.IO;
using System.Net;
using System.Text;
using System.Windows;

namespace WebWriter.Models
	{
	public class Uploader
		{
		const string ftpUserName = "websiteupdater";
		const string ftpPassword = "sandon14road";

		public static void Upload(string localFile, string remotePath)
			{
			int i = remotePath.LastIndexOf('/');
			if (i == -1)
				i = 0;
			string remoteFilename = remotePath.Substring(i);

			FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://ftp.servage.net/staffordchristadelphians.org.uk/{remotePath}.new");
			request.Method = WebRequestMethods.Ftp.UploadFile;
			request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);

			// Copy the contents of the file to the request stream.
			using (StreamReader sourceStream = new StreamReader(localFile))
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
				MessageBox.Show($"ftp upload failed.\r\nResponse status was '{response.StatusDescription}'", "WebWriter", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				}
			response.Close();

			request = (FtpWebRequest)WebRequest.Create($"ftp://ftp.servage.net/staffordchristadelphians.org.uk/{remotePath}.new");
			request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
			request.Method = WebRequestMethods.Ftp.Rename;
			request.RenameTo = remoteFilename;
			response = (FtpWebResponse)request.GetResponse();
			if (response.StatusCode != FtpStatusCode.FileActionOK)
				{
				MessageBox.Show($"ftp rename after upload failed.\r\nResponse status was '{response.StatusDescription}'", "WebWriter", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				}
			response.Close();
			MessageBox.Show("file written to the server", "Web Writer");
			}
		}
	}
