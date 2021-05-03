//
//	Last mod:	07 November 2020 13:21:52
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

		public DodgyStuffView(DodgyStuffViewModel viewModel)
				: base(viewModel, DataWindowMode.Close)
			{
			InitializeComponent();
			}
		}
	}
