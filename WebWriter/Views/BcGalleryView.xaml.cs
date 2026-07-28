//
//	Last mod:	27 July 2026 19:48:33
//
namespace WebWriter.Views
	{
	using Catel.Windows;
	using ViewModels;

	public partial class BcGalleryView
		{
		public BcGalleryView()
			: this(null) { }

		public BcGalleryView(BcGalleryViewModel? viewModel)
			: base(viewModel, DataWindowMode.Custom)
			{
			InitializeComponent();
			SizeToContent = System.Windows.SizeToContent.Manual;
			}
		}
	}
