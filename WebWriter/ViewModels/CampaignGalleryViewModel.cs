//
//	Last mod:	04 February 2025 16:38:57
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

		public string Folder { get; set; }

		public List<string> Files { get; set; } = [];

		public string Output { get; set; } = string.Empty;

		/// <summary>
		/// Gets the GoCommand command.
		/// </summary>
		public Command<object?> GoCommand
			{
			get
				{
				return goCommand ??= new Command<object?>(GoCommand_Execute);
				}
			}

		private Command<object?>? goCommand;

		/// <summary>
		/// Method to invoke when the GoCommand command is executed.
		/// </summary>
		/// <param name="parameter">The parameter of the command.</param>
		private void GoCommand_Execute(object? parameter)
			{
			var files = Directory.GetFiles(Folder, "*.jpg", SearchOption.AllDirectories).ToList();
			Files = files.Where(f=>!f.Contains("thumbnails")).ToList();
			}

		/// <summary>
		/// Gets the CreateOutputCommand command.
		/// </summary>
		public Command<object?> CreateOutputCommand
			{
			get
				{
				return createOutputCommand ??= new Command<object?>(CreateOutputCommand_Execute);
				}
			}

		private Command<object?>? createOutputCommand;

		/// <summary>
		/// Method to invoke when the CreateOutputCommand command is executed.
		/// </summary>
		/// <param name="parameter">The parameter of the command.</param>
		private void CreateOutputCommand_Execute(object? parameter)
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
				string pname = Path.GetFileName(Path.GetDirectoryName(item))!;

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
		}
	}
