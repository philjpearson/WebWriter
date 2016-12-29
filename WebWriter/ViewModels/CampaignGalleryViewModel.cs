//
//	Last mod:	27 April 2016 21:07:03
//
namespace WebWriter.ViewModels
	{
	using Catel.MVVM;
	using System.Threading.Tasks;
	using Catel.Data;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	public class CampaignGalleryViewModel : ViewModelBase
		{
		public CampaignGalleryViewModel()
			{
			Folder = @"F:\Campaign Gallery\Publish";
			}

		public override string Title { get { return "Campaign Photo Gallery"; } }

		// TODO: Register models with the vmpropmodel codesnippet

		// TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
		/// <summary>
		/// Gets or sets the Folder property value
		/// </summary>
		public string Folder
			{
			get { return GetValue<string>(FolderProperty); }
			set { SetValue(FolderProperty, value); }
			}

		/// <summary>
		/// Register the Folder property so it is known in the class.
		/// </summary>
		public static readonly PropertyData FolderProperty = RegisterProperty("Folder", typeof(string));

		/// <summary>
		/// Gets or sets the Files property value
		/// </summary>
		public List<string> Files
			{
			get { return GetValue<List<string>>(FilesProperty); }
			set { SetValue(FilesProperty, value); }
			}

		/// <summary>
		/// Register the Files property so it is known in the class.
		/// </summary>
		public static readonly PropertyData FilesProperty = RegisterProperty("Files", typeof(List<string>));

		/// <summary>
		/// Gets or sets the Output property value
		/// </summary>
		public string Output
			{
			get { return GetValue<string>(OutputProperty); }
			set { SetValue(OutputProperty, value); }
			}

		/// <summary>
		/// Register the Output property so it is known in the class.
		/// </summary>
		public static readonly PropertyData OutputProperty = RegisterProperty("Output", typeof(string));

		// TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets
		/// <summary>
		/// Gets the GoCommand command.
		/// </summary>
		public Command<object> GoCommand
			{
			get
				{
				if (_GoCommand == null)
					_GoCommand = new Command<object>(GoCommand_Execute);
				return _GoCommand;
				}
			}

		private Command<object> _GoCommand;

		/// <summary>
		/// Method to invoke when the GoCommand command is executed.
		/// </summary>
		/// <param name="parameter">The parameter of the command.</param>
		private void GoCommand_Execute(object parameter)
			{
			var files = Directory.GetFiles(Folder, "*.jpg", SearchOption.AllDirectories).ToList();
			Files = files.Where(f=>!f.Contains("thumbnails")).ToList();
			}

		/// <summary>
		/// Gets the CreateOutputCommand command.
		/// </summary>
		public Command<object> CreateOutputCommand
			{
			get
				{
				if (_CreateOutputCommand == null)
					_CreateOutputCommand = new Command<object>(CreateOutputCommand_Execute);
				return _CreateOutputCommand;
				}
			}

		private Command<object> _CreateOutputCommand;

		/// <summary>
		/// Method to invoke when the CreateOutputCommand command is executed.
		/// </summary>
		/// <param name="parameter">The parameter of the command.</param>
		private void CreateOutputCommand_Execute(object parameter)
			{
			//var data = [
			//  {
			//	thumb: 'thumb.jpg',
			//  image: 'image.jpg',
			//  title: 'My title',
			//  description: 'My description',
			//	},
			//  {
			//  thumb: 'thumb2.jpg',
			//  image: 'image2.jpg',
			//  title: 'My title 2',
			//  description: 'My description 2',
			//	}
			//];

			StringBuilder sb = new StringBuilder("var data = [");
			foreach (var item in Files)	
				{
				string fname = Path.GetFileName(item);
				string tname = fname.Replace("JPG", "jpg");
				string pname = Path.GetFileName(Path.GetDirectoryName(item));

				sb.AppendLine("  {");
				sb.AppendLine($"  thumb: 'img/gallery/thmb/{tname}',");
				sb.AppendLine($"  image: 'img/gallery/{fname}',");
				sb.AppendLine($"  title: '{fname}',");
				sb.AppendLine($"  description: 'Photo from {pname}.'");
				sb.AppendLine("  },");
				}
			sb = sb.Remove(sb.Length - 2, 1);
			sb.AppendLine("];");
			Output = sb.ToString();
			}

		protected override async Task Initialize()
			{
			await base.Initialize();

			// TODO: subscribe to events here
			}

		protected override async Task Close()
			{
			// TODO: unsubscribe from events here

			await base.Close();
			}
		}
	}
