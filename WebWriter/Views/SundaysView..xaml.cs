//
//	Last mod:	04 February 2025 12:07:58
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

		public SundaysView(SundaysViewModel? viewModel)
				: base(viewModel)
			{
			InitializeComponent();
			}
		}
	}
