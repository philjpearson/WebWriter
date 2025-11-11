//
//	Last mod:	04 February 2025 16:38:57
//
namespace WebWriter.ViewModels
	{
	using Catel.Data;
	using Catel.MVVM;
	using FileHelpers;
	using Models;
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Windows;
	public class TitlesViewModel : ViewModelBase
		{
		public TitlesViewModel()
			{
			Titles = [];
			}

		public override string Title { get { return "Bible Hour Titles"; } }

		const string filePath = @"C:\Users\Phil\OneDrive\My Documents\Ecclesia\Web site\titles.csv";

		public ObservableCollection<BibleHourTitle> Titles { get; set; }

		protected override async Task InitializeAsync()
			{
			await base.InitializeAsync();

			try
				{
				var engine = new FileHelperEngine<BibleHourTitleRecord>();
				var records = engine.ReadFile(filePath);
				foreach (var record in records)
					{
					DateTime dt = DateTime.Parse(record.Date!);
					var bht = new BibleHourTitle() { Date = dt, Title = record.Title ?? "" };
					Titles.Add(bht);
					}
				}
			catch (Exception ex)
				{
				MessageBox.Show($"Failed to load from titles.csv file: {ex.Message}", "WebWriter", MessageBoxButton.OK, MessageBoxImage.Error);
				await CloseAsync();
				}
			}

		protected override Task<bool> SaveAsync()
			{
			var titles = new List<BibleHourTitleRecord>();
			foreach (var t in Titles.OrderBy(t=>t.Date))
				{
				titles.Add(new BibleHourTitleRecord() { Date = t.Date.ToString("d MMMM yyyy"), Title = t.Title });
				}
			var engine = new FileHelperEngine<BibleHourTitleRecord>(Encoding.Unicode);
			engine.WriteFile(filePath, titles);
			Uploader.Upload(filePath, "titles.csv");
			return base.SaveAsync();
			}

		[DelimitedRecord(",")]
		public class BibleHourTitleRecord
			{
			[FieldQuoted('"', QuoteMode.AlwaysQuoted, MultilineMode.NotAllow)]
			public string? Date;
			[FieldQuoted('"', QuoteMode.AlwaysQuoted, MultilineMode.NotAllow)]
			public string? Title;
			}

		public class BibleHourTitle : ModelBase
			{
			public DateTime Date { get; set; }
			public string Title { get; set; } = string.Empty;
			}
		}
	}
