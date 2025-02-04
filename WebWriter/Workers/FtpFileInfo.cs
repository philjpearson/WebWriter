//
//	Last mod:	04 February 2025 11:38:52
//
using System;

public class FtpFileInfo
  {
  public bool IsDirectory { get; set; }
  public char[]? Permissions { get; set; }
  public int NrOfInodes { get; set; }
  public string? User { get; set; }
  public string? Group { get; set; }
  public long FileSize { get; set; }
  public DateTime LastModifiedDate { get; set; }
  public string? FileName { get; set; }
  }
