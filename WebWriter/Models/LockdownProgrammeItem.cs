//
//	Last mod:	02 January 2023 16:01:30
//
using System;
using CsvHelper.Configuration.Attributes;

namespace WebWriter.Models
	{
	public class LockdownProgrammeItem
		{
		public DateTime Date { get; set; }

		public int Type { get; set; }

		public string President { get; set; }

		public string Speaker { get; set; }

		public string Information { get; set; }

		[Optional]
		public string Collection2 { get; set; }

		[Optional]
		public string Collection3 { get; set; }
		}
	}
