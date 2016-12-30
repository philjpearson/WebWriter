//
//	Last mod:	30 December 2016 20:58:17
//
namespace WebWriter.ViewModels
	{
	using Catel.Data;
	using Catel.MVVM;
	using System;
	using System.Collections.ObjectModel;
	using System.Linq;
	using System.Runtime.Serialization;
	using System.Threading.Tasks;
	using System.Windows;
	using System.Xml.Linq;

	public class RecordingsViewModel : ViewModelBase
		{
		public class Recording : ModelBase
			{
			public DateTime Date { get; set; }
			public string Type { get; set; }
			public string File { get; set; }
			public string Speaker { get; set; }
			public string Ecclesia { get; set; }
			}

		public RecordingsViewModel()
			{
			}

		public override string Title { get { return "Recordings"; } }

		const string filePath = @"D:\philj\Documents\OneDrive\My Documents\Ecclesia\Web site\recordings.xml";

		// TODO: Register models with the vmpropmodel codesnippet
		// TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets

		public ObservableCollection<Recording> Recordings { get; set; }

		// TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets

		protected override async Task Initialize()
			{
			await base.Initialize();

			Recordings = new ObservableCollection<Recording>();

			try
				{
				XDocument xd = XDocument.Load(filePath);
				var recordings = (from r in xd.Root.Elements("recording")
													select new Recording
														{
														Date = (DateTime)r.Element("Date"),
														Type = (string)r.Element("Type"),
														File = (string)r.Element("File"),
														Speaker = (string)r.Element("Speaker"),
														Ecclesia = (string)r.Element("Ecclesia")
														}).ToList();
				Recordings = new ObservableCollection<Recording>(recordings);
				}
			catch (Exception ex)
				{
				MessageBox.Show($"Failed to load xml file: {ex.Message}", "WebWriter", MessageBoxButton.OK, MessageBoxImage.Error);
				await Close();
				}
			// TODO: subscribe to events here
			}

		protected override Task<bool> Save()
			{
			XDocument doc = new XDocument(new XDeclaration("1.0", null, null),
																		new XElement("root", Recordings.Select(r => new XElement("recording", new XElement("Date", r.Date),
																																																					new XElement("Type", r.Type),
																																																					new XElement("File", r.File),
																																																					new XElement("Speaker", r.Speaker),
																																																					new XElement("Ecclesia", r.Ecclesia)
																		))));
			string xxx = doc.ToString();
			return base.Save();
			}

		protected override async Task Close()
			{
			// TODO: unsubscribe from events here

			await base.Close();
			}
		}
	}
