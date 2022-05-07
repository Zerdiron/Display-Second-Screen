using System;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace WpfApp1
{
	/// <summary>
	/// Interaction logic for DisplayWindow.xaml
	/// </summary>
	public partial class DisplayWindow : Window
    {
        public int ImageID;
        public string Chemin;
        public string ImageName;
        public BitmapImage BitmapImageSource;

        public DisplayWindow(int ID, string Name, string chemin, BitmapImage BitmapImageToOpen)
        {
            InitializeComponent();

            ImageID = ID;
            Chemin = chemin;
            ImageName = Name;
            BitmapImageSource = BitmapImageToOpen;

            MaximizeToSecondaryMonitor(Display);
            SetImage();
        }

        private void SetImage()
        {
            ImageToDisplay.Source = BitmapImageSource;
        }

        public void MaximizeToSecondaryMonitor(Window window)
        {
            // Select Screen
            Screen secondaryScreen = Screen.AllScreens.Where(s => !s.Primary).FirstOrDefault();

            if (secondaryScreen != null)
            {
                if (!window.IsLoaded)
                    window.WindowStartupLocation = WindowStartupLocation.Manual;

				// Required to define the Startup Location
				System.Drawing.Rectangle workingArea = secondaryScreen.WorkingArea;
                window.Left = workingArea.Left;
                window.Top = workingArea.Top;
                window.Width = workingArea.Width;
                window.Height = workingArea.Height;
            }
        }

        public void Maximize(Window window)
        {
            // If window isn't loaded then maxmizing will result in the window displaying on the primary monitor
            if (window.IsLoaded)
            {
                window.WindowState = WindowState.Maximized;
            }
        }

        private void Display_Closing(object sender, EventArgs e)
        {
        }
    }
}
