//
//	Last mod:	30 December 2016 19:07:23
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
	using System.Threading.Tasks;
	using System.Windows;
	public class TitlesViewModel : ViewModelBase
		{
		public TitlesViewModel()
			{
			}

		public override string Title { get { return "Bible Hour Titles"; } }

		const string filePath = @"D:\philj\Documents\OneDrive\My Documents\Ecclesia\Web site\titles.csv";

		// TODO: Register models with the vmpropmodel codesnippet
		// TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
		/// <summary>
		/// Gets or sets the Titles property value
		/// </summary>
		public ObservableCollection<BibleHourTitle> Titles
			{
			get { return GetValue<ObservableCollection<BibleHourTitle>>(TitlesProperty); }
			set { SetValue(TitlesProperty, value); }
			}

		/// <summary>
		/// Register the Titles property so it is known in the class.
		/// </summary>
		public static readonly PropertyData TitlesProperty = RegisterProperty("Titles", typeof(ObservableCollection<BibleHourTitle>));
		// TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets

		protected override async Task Initialize()
			{
			await base.Initialize();

			Titles = new ObservableCollection<BibleHourTitle>();
			try
				{
				var engine = new FileHelperEngine<BibleHourTitleRecord>();
				var records = engine.ReadFile(filePath);
				foreach (var record in records)
					{
					DateTime dt = DateTime.Parse(record.Date);
					var bht = new BibleHourTitle() { Date = dt, Title = record.Title };
					Titles.Add(bht);
					}
				}
			catch (Exception ex)
				{
				MessageBox.Show($"Failed to load from titles.csv file: {ex.Message}", "WebWriter", MessageBoxButton.OK, MessageBoxImage.Error);
				await Close();
				}
			}

		protected override Task<bool> Save()
			{
			var titles = new List<BibleHourTitleRecord>();
			foreach (var t in Titles.OrderBy(t=>t.Date))
				{
				titles.Add(new BibleHourTitleRecord() { Date = t.Date.ToString("d MMMM yyyy"), Title = t.Title });
				}
			var engine = new FileHelperEngine<BibleHourTitleRecord>();
			engine.WriteFile(filePath, titles);
			Uploader.Upload(filePath, "titles.csv");
			return base.Save();
			}

		protected override async Task Close()
			{
			// TODO: unsubscribe from events here

			await base.Close();
			}

		[DelimitedRecord(",")]
		public class BibleHourTitleRecord
			{
			[FieldQuoted('"', QuoteMode.AlwaysQuoted, MultilineMode.NotAllow)]
			public string Date;
			[FieldQuoted('"', QuoteMode.AlwaysQuoted, MultilineMode.NotAllow)]
			public string Title;

			public BibleHourTitleRecord Tidied()
				{
				if (Date.StartsWith("\""))
					Date = Date.Substring(1);
				if (Date.EndsWith("\""))
					Date = Date.Substring(0, Date.Length - 1);
				if (Title.StartsWith("\""))
					Title = Title.Substring(1);
				if (Title.EndsWith("\""))
					Title = Title.Substring(0, Title.Length - 1);
				return this;
				}
			}

		public class BibleHourTitle : ModelBase
			{
			public DateTime Date { get; set; }
			public string Title { get; set; }
			}
		}
	}
