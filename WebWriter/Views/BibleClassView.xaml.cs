//
//	Last mod:	04 February 2025 12:07:58
//
namespace WebWriter.Views
	{
	using Catel.Windows;
	using ViewModels;

	public partial class BibleClassView
		{
		public BibleClassView()
				: this(null)
			{ }

		public BibleClassView(BibleClassViewModel? viewModel)
				: base(viewModel)
			{
			InitializeComponent();
			}
		}
	}
