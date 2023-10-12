//
//	Last mod:	11 October 2023 15:56:46
//
namespace WebWriter.Views
	{
	using Catel.Windows;
	using ViewModels;

	public partial class SundaysView
		{
		public SundaysView()
				: this(null)
			{ }

		public SundaysView(SundaysViewModel viewModel)
				: base(viewModel)
			{
			InitializeComponent();
			}
		}
	}
