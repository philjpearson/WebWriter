//
//	Last mod:	04 February 2025 12:07:57
//
namespace WebWriter.Views
	{
	using Catel.Windows;
	using ViewModels;

	public partial class GalleryView
		{
		public GalleryView()
			: this(null) { }

		public GalleryView(GalleryViewModel? viewModel)
			: base(viewModel, DataWindowMode.Custom)
			{
			InitializeComponent();
			SizeToContent = System.Windows.SizeToContent.Manual;
			}
		}
	}
