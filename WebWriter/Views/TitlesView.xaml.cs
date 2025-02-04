//
//	Last mod:	04 February 2025 12:07:59
//
namespace WebWriter.Views
	{
	using Catel.Windows;
	using ViewModels;

	public partial class TitlesView
		{
		public TitlesView()
				: this(null)
			{ }

		public TitlesView(TitlesViewModel? viewModel)
				: base(viewModel)
			{
			InitializeComponent();
			}
		}
	}
