using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using WK.Libraries.BetterFolderBrowserNS;

namespace WpfApp1
{
	/// <summary>
	/// Logique d'interaction pour MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
    {
        public List<DisplayWindow> ListDisplays =  new List<DisplayWindow>();
        private readonly List<string> Files = new List<string>();
        private BitmapImage BitmapImageToOpen;
        private string LastFolder = "";
        private string FolderPath = "";
        private string FileName = "";
        private int LastID = 0;
        private int SelectedOpenedIndex;

        public MainWindow()
        {
            InitializeComponent();
        }

        private BitmapImage BitmapImageFromFullPath(string FullPath)
        {
            System.Drawing.Image image = System.Drawing.Image.FromFile(FullPath);
            BitmapImage bitmap = new BitmapImage();

            using (MemoryStream stream = new MemoryStream())
            {
                // Save to the stream
                image.Save(stream, ImageFormat.Jpeg);

                // Rewind the stream
                stream.Seek(0, SeekOrigin.Begin);

                // Tell the WPF BitmapImage to use this stream
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
            }

            return bitmap;
        }

        private void RefreshListOpenedImage()
        {
            ListOpenedImage.Items.Clear();
            foreach (DisplayWindow Display in ListDisplays)
                ListOpenedImage.Items.Add(Display.ImageName);

            if (!ListOpenedImage.HasItems)
                ManageButton(null);
        }

        private void RefreshListFiles()
        {
            ListFiles.Items.Clear();
            foreach (string name in Files)
                ListFiles.Items.Add(name);
        }

        private void ManageButton(object sender)
        {
            if (sender != null && sender.Equals(SelectFolder))
            {
                OpenSelected.IsEnabled = true;
                AddSelected.IsEnabled = true;
            }
            else if (sender != null && (sender.Equals(OpenSelected) || sender.Equals(AddSelected)))
            {
                ActivateSelected.IsEnabled = true;
                CloseSelected.IsEnabled = true;
                CloseAllImage.IsEnabled = true;
            }
            else if (sender == null || sender.Equals(CloseAllImage))
            {
                ActivateSelected.IsEnabled = false;
                CloseSelected.IsEnabled = false;
                CloseAllImage.IsEnabled = false;
            }
        }

        private void SetPreview(object sender)
        {
            if (ListFiles.HasItems && sender.Equals(ListFiles))
                PreviewImage.Source = BitmapImageFromFullPath(FolderPath + @"\" + ListFiles.SelectedValue.ToString());
            else if (ListOpenedImage.HasItems && sender.Equals(ListOpenedImage))
                PreviewImage.Source = BitmapImageFromFullPath(FolderPath + @"\" + ListOpenedImage.SelectedValue.ToString());
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            DisplayWindow Display = CreateDisplayWindow();

            AddDisplayToList(sender, Display);
        }

		private void Open_Click(object sender, RoutedEventArgs e)
        {
            DisplayWindow Display = CreateDisplayWindow();

            AddDisplayToList(sender, Display);

            ShowDisplay(Display);
        }

		private void AddDisplayToList(object sender, DisplayWindow Display)
        {
            if (!ListDisplays.Any(Item => Display.ImageName == Item.ImageName))
            {
                ListDisplays.Add(Display);
                LastID += 1;

                RefreshListOpenedImage();
                ManageButton(sender);
            }
        }
		private void ShowDisplay(DisplayWindow Display)
		{
            Display.Show();
            Display.Maximize(Display);
        }

        private DisplayWindow CreateDisplayWindow()
		{
            FileName = ListFiles.SelectedValue.ToString();
            BitmapImageToOpen = BitmapImageFromFullPath(FolderPath + @"\" +FileName);

            return new DisplayWindow(LastID, FileName, FolderPath, BitmapImageToOpen);
        }

        private void CloseSelected_Click(object sender, RoutedEventArgs e)
        {
            ListDisplays[SelectedOpenedIndex].Close();

            ListDisplays.RemoveAt(SelectedOpenedIndex);

            RefreshListOpenedImage();
        }

        private void CloseAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (DisplayWindow Display in ListDisplays)
            {
                Display.Close();
            }

            ListDisplays.Clear();
            LastID = 0;

            RefreshListOpenedImage();
            ManageButton(sender);
        }

        private void SelectFolder_Click(object sender, RoutedEventArgs e)
        {
            BetterFolderBrowser betterFolderBrowser = new BetterFolderBrowser();

            if (LastFolder == "")
                betterFolderBrowser.RootFolder = betterFolderBrowser.RootFolder.Substring(0, 2);
            else 
                betterFolderBrowser.RootFolder = LastFolder;

            DialogResult result = betterFolderBrowser.ShowDialog();

            if ("OK" == result.ToString())
            {
                FolderPath = betterFolderBrowser.SelectedPath;
                LastFolder = FolderPath;

                RetrieveAllFilesFromFolderPath(FolderPath);

                ManageButton(sender);

                ListFiles.SelectedIndex = ListFiles.Items.IndexOf(FileName);
            }
            else if ("Cancel" == result.ToString())
            {
                
            }
            else
            {
                System.Windows.MessageBox.Show("Error");
            }
        }

        private void RetrieveAllFilesFromFolderPath(string FolderPath)
        {
            Files.Clear();
            foreach (string file in Directory.EnumerateFiles(FolderPath, "*.*"))
            {
                string filename = file.Replace(FolderPath, "").Replace(@"\", "");

                if (FormatValide(filename))
                    Files.Add(filename);
            }

            Files.Sort();

            RefreshListFiles();
        }

		private bool FormatValide(string filename)
		{
            int lastPoint = filename.LastIndexOf(".");
			if (lastPoint != 0)
			{
                string format = filename.Substring(lastPoint);
                if (format.Equals(".jpg")
                    || format.Equals(".jpeg")
                    || format.Equals(".png")
                    || format.Equals(".bmp")
                    || format.Equals(".tiff")
                    || format.Equals(".gif"))
			    {
                    return true;
				}
            }
            return false;
		}

		private void Activate_Click(object sender, RoutedEventArgs e)
        {
            if (!ListDisplays[SelectedOpenedIndex].Activate())
			{
                ShowDisplay(ListDisplays[SelectedOpenedIndex]);
			}
        }

        private void ListFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetPreview(sender);
        }

        private void ListOpenedImage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListOpenedImage.HasItems)
            {
                SelectedOpenedIndex = int.Parse(ListOpenedImage.SelectedIndex.ToString());
                SetPreviewHistorique(ListDisplays[SelectedOpenedIndex].Chemin.ToString(), sender);
            }
        }

        private void SetPreviewHistorique(string chemmin, object sender)
        {
            if (ListFiles.HasItems && sender.Equals(ListFiles))
                PreviewImage.Source = BitmapImageFromFullPath(chemmin + @"\" + ListFiles.SelectedValue.ToString());
            else if (ListOpenedImage.HasItems && sender.Equals(ListOpenedImage))
                PreviewImage.Source = BitmapImageFromFullPath(chemmin + @"\" + ListOpenedImage.SelectedValue.ToString());
        }
    }
}