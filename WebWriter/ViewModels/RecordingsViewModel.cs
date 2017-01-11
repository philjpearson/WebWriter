//
//	Last mod:	02 January 2017 20:02:15
//
namespace WebWriter.ViewModels
	{
	using Catel.Data;
	using Catel.MVVM;
	using Microsoft.Win32;
	using Models;
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.IO;
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
			public string Text { get; set; }
			public string Speaker { get; set; }
			public string Ecclesia { get; set; }
			}

		public RecordingsViewModel()
			{
			}

		public override string Title { get { return "Recordings"; } }

		const string filePath = @"D:\philj\Documents\OneDrive\My Documents\Ecclesia\Web site\recordings.xml";

		List<string> addedFiles = new List<string>();

		// TODO: Register models with the vmpropmodel codesnippet
		// TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets

		public ObservableCollection<Recording> Recordings { get; set; }

		public string NewRecording { get; set; }

		// TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets

		/// <summary>
		/// Gets the BrowseCommand command.
		/// </summary>
		public Command<object> BrowseCommand
			{
			get
				{
				if (_BrowseCommand == null)
					_BrowseCommand = new Command<object>(BrowseCommand_Execute);
				return _BrowseCommand;
				}
			}

		private Command<object> _BrowseCommand;

		/// <summary>
		/// Method to invoke when the BrowseCommand command is executed.
		/// </summary>
		/// <param name="parameter">The parameter of the command.</param>
		private void BrowseCommand_Execute(object parameter)
			{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "mp3 files (*.mp3)|*.mp3|All filkes (*.*)|*.*";
			if (ofd.ShowDialog() == true)
				{
				NewRecording = ofd.FileName;
				}
			}

		/// <summary>
		/// Gets the AddCommand command.
		/// </summary>
		public Command<object, object> AddCommand
			{
			get
				{
				if (_AddCommand == null)
					_AddCommand = new Command<object, object>(AddCommand_Execute, AddCommand_CanExecute);
				return _AddCommand;
				}
			}

		private Command<object, object> _AddCommand;

		/// <summary>
		/// Method to check whether the AddCommand command can be executed.
		/// </summary>
		/// <param name="parameter">The parameter of the command.</param>
		private bool AddCommand_CanExecute(object parameter)
			{
			return !string.IsNullOrWhiteSpace(NewRecording);
			}

		/// <summary>
		/// Method to invoke when the AddCommand command is executed.
		/// </summary>
		/// <param name="parameter">The parameter of the command.</param>
		private void AddCommand_Execute(object parameter)
			{
			if (!Recordings.Any(r => r.File == NewRecording))
				{
				Recordings.Add(new Recording() { Date = DateTime.Now, File = "lib/" + Path.GetFileName(NewRecording) });
				addedFiles.Add(NewRecording);
				}
			}

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
														Ecclesia = (string)r.Element("Ecclesia"),
														Text = (string)r.Element("Text")
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
																		new XElement("root", Recordings.OrderBy(r => r.Date)
																		 .Select(r => new XElement("recording", new XElement("Date", r.Date.ToString("d MMMM yyyy")),
																																						new XElement("Type", r.Type),
																																						new XElement("File", r.File),
																																						new XElement("Text", r.Text),
																																						new XElement("Speaker", r.Speaker),
																																						new XElement("Ecclesia", r.Ecclesia)
																		))));
			string bakFile = filePath + ".bak";
			File.Delete(bakFile);
			File.Move(filePath, bakFile);
			doc.Save(filePath);

			// upload all the added files
			foreach (var file in addedFiles)
				{
				string remotePath = "private/lib/" + Path.GetFileName(file);
				Uploader.Upload(file, remotePath, true);
				}

			Uploader.Upload(filePath, "private/recordings.xml");
			return base.Save();
			}

		protected override async Task Close()
			{
			// TODO: unsubscribe from events here

			await base.Close();
			}
		}
	}
