﻿//
//	Last mod:	04 February 2025 11:38:52
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinSCP;

namespace WebWriter.Workers
	{
	class FtpFileSystemItem
		{
		public readonly RemoteFileInfo? FileInfo;
		public List<FtpFileSystemItem>? Children;

		private static string? rootName;

		public FtpFileSystemItem(string rootName)
			{
			FtpFileSystemItem.rootName = rootName;
			IsRoot = true;
			Children = [];
			}

		public FtpFileSystemItem(RemoteFileInfo inf)
			{
			FileInfo = inf;
			if (IsDirectory)
				Children = [];
			}

		public string Name { get { return FileInfo?.Name ?? "root"; } }

		public string Path { get { return FileInfo?.FullName.Substring(rootName!.LastIndexOf('/') + 1) ?? "root"; } }

		public bool IsRoot { get; private set; } = false;

		public bool IsDirectory
			{
			get { return FileInfo is not null && (IsRoot || FileInfo.IsDirectory && !FileInfo.IsParentDirectory && !FileInfo.IsThisDirectory); }
			}

		public FtpFileSystemItem AddChild(RemoteFileInfo inf)
			{
			return AddChild(new FtpFileSystemItem(inf));
			}

		public FtpFileSystemItem AddChild(FtpFileSystemItem item)
			{
			if (Children is null)
				throw new InvalidOperationException("Not ready for children");

			Children.Add(item);
			return item;
			}

		public List<FtpFileSystemItem> ToFlatList()
			{
			var result = new List<FtpFileSystemItem> { this };
			if (IsDirectory && Children is not null)
				foreach (var item in Children)
					result.AddRange(item.ToFlatList());
			return result;
			}

		public override string ToString()
			{
			return $"{(IsDirectory ? "[d]" : "   ")}{Name} - {Path}";
			}
		}
	}
