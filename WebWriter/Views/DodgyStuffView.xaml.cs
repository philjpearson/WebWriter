//
//	Last mod:	04 February 2025 12:07:57
//
namespace WebWriter.Views
	{
	using Catel.Windows;
	using ViewModels;

	public partial class DodgyStuffView
		{
		public DodgyStuffView()
				: this(null)
			{ }

		public DodgyStuffView(DodgyStuffViewModel? viewModel)
				: base(viewModel, DataWindowMode.Close)
			{
			InitializeComponent();
			}
		}
	}
