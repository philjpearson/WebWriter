//
//	Last mod:	07 November 2020 11:41:10
//
using System;

public class FtpFileInfo
  {
  public bool IsDirectory { get; set; }
  public char[] Permissions { get; set; }
  public int NrOfInodes { get; set; }
  public string User { get; set; }
  public string Group { get; set; }
  public long FileSize { get; set; }
  public DateTime LastModifiedDate { get; set; }
  public string FileName { get; set; }
  }
