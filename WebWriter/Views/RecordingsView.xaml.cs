//
//	Last mod:	04 February 2025 12:07:58
//
namespace WebWriter.Views
	{
	using Catel.Windows;
	using ViewModels;

	public partial class RecordingsView
		{
		public RecordingsView()
				: this(null)
			{ }

		public RecordingsView(RecordingsViewModel? viewModel)
				: base(viewModel)
			{
			InitializeComponent();
			}
		}
	}
