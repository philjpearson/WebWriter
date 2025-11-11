//
//	Last mod:	04 February 2025 16:38:59
//
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Catel.IoC;
using Catel.Services;

namespace WebWriter.Models;

public class ProgrammeModel
	{
	private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

	private readonly IMessageService? messageService;
	private readonly DayOfWeek day;

	public List<ProgrammeItem> Programme { get; set; } = [];

	public ProgrammeModel(DayOfWeek day)
		{
		this.day = day;
		messageService = ServiceLocator.Default.ResolveType<IMessageService>();
		}

	public void CreateEmpty(DateTime startDate, DateTime endDate)
		{
		var dt = startDate;

		while (dt.DayOfWeek != day)
			dt = dt.AddDays(1);
		for (; dt <= endDate; dt += TimeSpan.FromDays(7))
			{
			Programme.Add(new ProgrammeItem(dt));
			}
		}

	public bool Populate(DataView dataView)
		{
		try
			{
			foreach (DataRowView item in dataView)
				{
				var entry = Programme.SingleOrDefault(p => p.Date == item["Date"] as DateTime?);
				if (entry is not null)
					{
					entry.Speaker = item["Speaker"].ToString();
					entry.Ecclesia = item["Ecclesia"].ToString();
					if (day == DayOfWeek.Wednesday)
						entry.Subject = string.IsNullOrEmpty(item["Subject"].ToString()) ? item["Title"].ToString() : item["Subject"].ToString();
					entry.Email = item["Email"].ToString();
					}
				}
			}
		catch (Exception ex)
			{
			logger.Error("Error populating programme: {0}", ex.ToString());
			return false;
			}
		return true;
		}

	public async Task ExportToExcel(string fileName, string programmeName)
		{
		logger.Info("Exporting {0} programme to {1}", programmeName, fileName);
		try
			{
			var xldoc = new ExcelGenerator.CollectionDocument<ProgrammeItem>(fileName)
				{
				Title = $"{programmeName} Programme Export",
				FixedRowCount = 1,
				};
			string[] headers = day == DayOfWeek.Wednesday
				? [ "Date", "Speaker", "Ecclesia", "Subject", "email", ]
				: ["Date", "Speaker", "Ecclesia", "email",];
			xldoc.CreateWorkbook(Programme, "Programme", headers, ExportRowSupplier);
			}
		catch (Exception ex)
			{
			logger.Error($"Export to Excel failed: {ex.Message}");
			await messageService?.ShowAsync($"Excel export failed: {ex.Message}", RWS.UIClasses.AssemblyInfo.AssemblyTitle, MessageButton.OK, MessageImage.Error)!;
			}
		}

	object?[] ExportRowSupplier(ProgrammeItem pi)
		{
		object?[] result = new object?[5];

		int index = 0;
		result[index++] = pi.Date;
		result[index++] = pi.Speaker;
		result[index++] = pi.Ecclesia;
		if (day == DayOfWeek.Wednesday)
			result[index++] = pi.Subject;
		result[index++] = pi.Email;
		return result;
		}
	}
