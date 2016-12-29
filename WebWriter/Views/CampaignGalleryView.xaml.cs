//
//	Last mod:	27 April 2016 18:06:04
//
namespace WebWriter.Views
	{
	using Catel.Windows;
	using ViewModels;

	public partial class CampaignGalleryView
		{
		public CampaignGalleryView()
				: this(null)
			{ }

		public CampaignGalleryView(CampaignGalleryViewModel viewModel)
				: base(viewModel)
			{
			InitializeComponent();
			SizeToContent = System.Windows.SizeToContent.Manual;
			}
		}
	}
