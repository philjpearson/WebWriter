//
//	Last mod:	07 March 2015 20:08:15
//
namespace WebWriter.Views
	{
	using Catel.Windows;
	using ViewModels;

	public partial class GalleryView
		{
		public GalleryView()
			: this(null) { }

		public GalleryView(GalleryViewModel viewModel)
			: base(viewModel, DataWindowMode.Custom)
			{
			InitializeComponent();
			SizeToContent = System.Windows.SizeToContent.Manual;
			}
		}
	}
