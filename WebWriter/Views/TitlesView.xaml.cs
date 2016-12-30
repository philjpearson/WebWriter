namespace WebWriter.Views
	{
	using Catel.Windows;
	using ViewModels;

	public partial class TitlesView
		{
		public TitlesView()
				: this(null)
			{ }

		public TitlesView(TitlesViewModel viewModel)
				: base(viewModel)
			{
			InitializeComponent();
			}
		}
	}
