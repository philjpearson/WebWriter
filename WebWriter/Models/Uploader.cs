//
//	Last mod:	12 January 2019 11:55:52
//
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;

namespace WebWriter.Models
	{
	public class Uploader
		{
		const string ftpUserName = "1007246_web"; // "websiteupdater";
		const string ftpPassword = "sandon14Road";

		public static bool Upload(string localFile, string remotePath, bool binary = false)
			{
			int i = 1 + remotePath.LastIndexOf('/');
			string remoteFilename = remotePath.Substring(i);
			string remoteAddress = $"ftp://ftp01.servage.net/staffordchristadelphians.org.uk/public_html/{remotePath}.new";
			FtpWebRequest request = (FtpWebRequest)WebRequest.Create(remoteAddress);
			request.Method = WebRequestMethods.Ftp.UploadFile;
			request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);

			bool success = false;
			FtpWebResponse response;

			if (binary)
				{
				request.UseBinary = true;
				int bufferSize = 4096;
				byte[] buffer = new byte[bufferSize];
				int bytesRead = 0;
				Stream requestStream = request.GetRequestStream();

				try
					{
					using (FileStream fs = File.OpenRead(localFile))
						{
						do
							{
							bytesRead = fs.Read(buffer, 0, bufferSize);
							requestStream.Write(buffer, 0, bytesRead);
							} while (bytesRead > 0);
						}
					}
				catch (Exception ex)
					{
					MessageBox.Show($"Exception during upload: {ex.Message}", "WebWriter", MessageBoxButton.OK, MessageBoxImage.Exclamation);
					}
				finally
					{
					requestStream.Close();
					}
				}
			else
				{
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
				}
			response = (FtpWebResponse)request.GetResponse();
			if (response.StatusCode != FtpStatusCode.CommandOK && response.StatusCode != FtpStatusCode.ClosingData)
				{
				MessageBox.Show($"ftp upload failed.\r\nResponse status was '{response.StatusDescription}'", "WebWriter", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				}
			else
				{
				success = true;
				}
			response.Close();

			if (success)
				{
				request = (FtpWebRequest)WebRequest.Create(remoteAddress);
				request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
				request.Method = WebRequestMethods.Ftp.Rename;
				request.RenameTo = remoteFilename;
				response = (FtpWebResponse)request.GetResponse();
				if (response.StatusCode != FtpStatusCode.FileActionOK)
					{
					MessageBox.Show($"ftp rename after upload failed.\r\nResponse status was '{response.StatusDescription}'", "WebWriter", MessageBoxButton.OK, MessageBoxImage.Exclamation);
					}
				response.Close();
				}
			return success;
			}
		}
	}
