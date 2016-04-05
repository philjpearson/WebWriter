//
//	Last mod:	12 January 2016 17:29:52
//
namespace WebWriter.Views
	{
	public partial class MainWindow : Catel.Windows.Window
		{
		public MainWindow()
			{
			InitializeComponent();
			CanCloseUsingEscape = false;
			}

		private void TitleBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
			{
			DragMove();
			}
		}
	}
