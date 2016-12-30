namespace WebWriter.Views
	{
	using Catel.Windows;
	using ViewModels;

	public partial class RecordingsView
		{
		public RecordingsView()
				: this(null)
			{ }

		public RecordingsView(RecordingsViewModel viewModel)
				: base(viewModel)
			{
			InitializeComponent();
			}
		}
	}
