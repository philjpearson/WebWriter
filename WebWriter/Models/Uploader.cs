//
//	Last mod:	04 February 2025 15:07:05
//
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using NLog;

namespace WebWriter.Models
	{
	public class Uploader
		{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();

		const string ftpUserName = "u880159079.staffordchristadelphians.org.uk";
		const string ftpPassword = "z6ZjgSn4tfkM_nD";

		public static bool Upload(string localFile, string remotePath, bool binary = false)
			{
			int i = 1 + remotePath.LastIndexOf('/');
			string remoteFilename = remotePath[i..];
			string remoteAddress = $"ftp://staffordchristadelphians.org.uk/{remotePath}.new";
			FtpWebRequest request = (FtpWebRequest)WebRequest.Create(remoteAddress);
			request.Method = WebRequestMethods.Ftp.UploadFile;
			request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);

			bool success = false;
			FtpWebResponse response;
			Stream? requestStream = null;

			try
				{
				if (binary)
					{
					request.UseBinary = true;
					int bufferSize = 4096;
					byte[] buffer = new byte[bufferSize];
					int bytesRead = 0;
					requestStream = request.GetRequestStream();

					using (FileStream fs = File.OpenRead(localFile))
						{
						do
							{
							bytesRead = fs.Read(buffer, 0, bufferSize);
							requestStream.Write(buffer, 0, bytesRead);
							} while (bytesRead > 0);
						}
					requestStream.Close();
					}
				else
					{
					// Copy the contents of the file to the request stream.
					using (StreamReader sourceStream = new StreamReader(localFile))
						{
						byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
						sourceStream.Close();
						request.ContentLength = fileContents.Length;

						requestStream = request.GetRequestStream();
						requestStream.Write(fileContents, 0, fileContents.Length);
						requestStream.Close();
						}
					}
				response = (FtpWebResponse)request.GetResponse();
				if (response.StatusCode != FtpStatusCode.CommandOK && response.StatusCode != FtpStatusCode.ClosingData)
					{
					logger.Error("ftp upload failed.\r\nResponse status was '{0}'", response.StatusDescription);
					MessageBox.Show($"ftp upload failed.\r\nResponse status was '{response.StatusDescription}'", "WebWriter", MessageBoxButton.OK, MessageBoxImage.Exclamation);
					}
				else
					{
					success = true;
					}
				response.Close();
				}
			catch (Exception ex)
				{
				logger.Error("Exception during upload: {0}", ex.Message);
				MessageBox.Show($"Exception during upload: {ex.Message}", "WebWriter", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				}
			finally
				{
				requestStream?.Close();
				}

			if (success)
				{
				request = (FtpWebRequest)WebRequest.Create(remoteAddress);
				request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
				request.Method = WebRequestMethods.Ftp.Rename;
				request.RenameTo = remoteFilename;
				response = (FtpWebResponse)request.GetResponse();
				if (response.StatusCode != FtpStatusCode.FileActionOK)
					{
					logger.Error("ftp rename after upload failed.\r\nResponse status was '{0}'", response.StatusDescription);
					MessageBox.Show($"ftp rename after upload failed.\r\nResponse status was '{response.StatusDescription}'", "WebWriter", MessageBoxButton.OK, MessageBoxImage.Exclamation);
					}
				response.Close();
				}
			return success;
			}
		}
	}
