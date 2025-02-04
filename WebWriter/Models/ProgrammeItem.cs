//
//	Last mod:	04 February 2025 11:21:36
//
using System;

namespace WebWriter.Models;

public class ProgrammeItem
	{
	public DateTime Date { get; set; }

	public int Type { get; set; }

	public string? President { get; set; }

	public string? Speaker { get; set; }

	public string? Ecclesia { get; set; }

	public string? Email { get; set; }

	public string? Subject { get; set; }

	public string? Information { get; set; }

	public ProgrammeItem(DateTime date)
		{
		Date = date;
		}
	}
