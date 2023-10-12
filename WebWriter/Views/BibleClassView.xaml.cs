//
//	Last mod:	12 October 2023 10:31:33
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

		public BibleClassView(BibleClassViewModel viewModel)
				: base(viewModel)
			{
			InitializeComponent();
			}
		}
	}
