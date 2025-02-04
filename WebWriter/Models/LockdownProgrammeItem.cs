//
//	Last mod:	04 February 2025 13:50:26
//
using System;
using CsvHelper.Configuration.Attributes;

#nullable disable

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
