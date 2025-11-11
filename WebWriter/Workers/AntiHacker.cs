//
//	Last mod:	05 February 2025 15:55:15
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Catel.IoC;
using Catel.Services;
using WebWriter.Utilities;
using WebWriter.ViewModels;
using WinSCP;

namespace WebWriter.Workers
	{
	internal class AntiHacker
		{
#if false
    const string remoteAddress = "ftp://staffordchristadelphians.org.uk/public_html/";
#endif
    const string serverAddress = "staffordchristadelphians.org.uk";
    const string ftpUserName = "u880159079.staffordchristadelphians.org.uk"; // "1007246_code";
    const string ftpPassword = "z6ZjgSn4tfkM_nD"; // "7wtk3Es1zthmBBHWdPyY";
    const string rootDirectory = "/public_html/"; // "staffordchristadelphians.org.uk/public_html/";


    private static readonly SessionOptions sessionOptions = new()
			{
      Protocol = Protocol.Ftp,
      HostName = serverAddress,
      UserName = ftpUserName,
      Password = ftpPassword
      };


    protected static Regex ftpListingRegex = new(@"^([d-])((?:[rwxt-]{3}){3})\s+(\d{1,})\s+(\w+)?\s+(\w+)?\s+(\d{1,})\s+(\w+)\s+(\d{1,2})\s+(\d{4})?(\d{1,2}:\d{2})?\s+(.+?)\s?$",
																							RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
    protected static readonly string Timeformat = "MMM dd yyyy HH:mm";

    static readonly System.Collections.Specialized.StringCollection legitDirectories;

    private Session? session;


    static AntiHacker()
			{
      legitDirectories = Properties.Settings.Default.LegitDirectories;
      }

    internal async Task CheckDirectoriesAsync()
			{
      var dodgy = new List<string>();
      using (new CursorOverride(Cursors.Wait))
        {
        var files = ListFiles();
        foreach (var file in files)
          {
          if (!legitDirectories.Contains(file.Path))
            dodgy.Add(file.Path);
          }
        }
      if (dodgy.Count == 0)
        MessageBox.Show("Nothing dodgy", "WebWriter AntiHacker", MessageBoxButton.OK, MessageBoxImage.Information);
      else
        {
        IUIVisualizerService? uIVisualiserService = ServiceLocator.Default.ResolveType<IUIVisualizerService>();
				if (uIVisualiserService is not null)
					{
          await uIVisualiserService.ShowDialogAsync<DodgyStuffViewModel>(dodgy);
					}
        }
      }

    public static bool Delete(string thing)
			{
      try
        {
        using (Session session = new Session())
          {
          session.Open(sessionOptions);
          session.RemoveFiles($"{rootDirectory}{thing}").Check();
          return true;
          }
        }
      catch (Exception ex)
				{
        Debug.WriteLine($"{ex.GetType().Name} in AntiHacker.Delete: {ex.Message}");
        return false;
				}
      }

    public static void MakeLegitimate(string name)
      {
      legitDirectories.Add(name);
      Properties.Settings.Default.LegitDirectories = legitDirectories;
      Properties.Settings.Default.Save();
      }

    public static void ResetLegitimate()
			{
      Properties.Settings.Default.Reset();
			}

    private List<FtpFileSystemItem> ListFiles()
      {
      try
        {
        
        var rootItem = new FtpFileSystemItem(rootDirectory);

        using (session = new Session())
          {
          session.Open(sessionOptions);
          AddDirectory(rootItem, rootDirectory);
          }
        return rootItem.ToFlatList();
#if false
        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(remoteAddress);
        request.Method = WebRequestMethods.Ftp.ListDirectory;

        request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
        FtpWebResponse response = (FtpWebResponse)request.GetResponse();
        Stream responseStream = response.GetResponseStream();
        StreamReader reader = new StreamReader(responseStream);
        string listing = reader.ReadToEnd();

        reader.Close();
        response.Close();

        return listing.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
#endif
        }
      catch (Exception ex)
        {
        Debug.WriteLine($"{ex.GetType().Name} in AntiHacker.ListFiles: {ex.Message}");
        return new List<FtpFileSystemItem>();
        }
      }

    private void AddDirectory(FtpFileSystemItem parentItem, string path)
			{
      RemoteDirectoryInfo directory = session!.ListDirectory(path);
			foreach (RemoteFileInfo item in directory.Files)
				{
        if (!item.IsParentDirectory && !item.IsThisDirectory)
          {
          var child = parentItem.AddChild(item);
          if (child.IsDirectory)
            AddDirectory(child, child.FileInfo!.FullName);
          }
				}
      }
    }
	}
