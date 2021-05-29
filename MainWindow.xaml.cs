using System.Windows;
using System.Windows.Controls;
using System.Drawing;
using Image = System.Drawing.Image;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System;
using System.Windows.Media;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.IO;

namespace EXE_Icon_Extractor
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool IsLoad = false;
        public string exePath = null;
        public MainWindow()
        {
            InitializeComponent();
            PixelComboBox.SelectedIndex = 2;
            ReviewIconImage.Source = ChangeBitmapToImageSource(Properties.Resources.NoReview);
        }

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);
        /// <summary>
        /// 从bitmap转换成ImageSource
        /// </summary>
        /// <param name="icon"></param>
        /// <returns></returns>
        public static ImageSource ChangeBitmapToImageSource(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            if (!DeleteObject(hBitmap))
            {
                throw new System.ComponentModel.Win32Exception();
            }
            return wpfBitmap;
        }

        private void FileBrowseBtn_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "Executable file|*.exe"
            };
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                exePath = openFileDialog.FileName;
            }
            else
            {
                exePath = null;
            }
            if (exePath != null)
            {
                ExePathTextBox.Text = exePath;
                Bitmap icon = System.Drawing.Icon.ExtractAssociatedIcon(exePath).ToBitmap();
                ReviewIconImage.Source = ChangeBitmapToImageSource(icon);
                SaveImageSizeLabel.Content = "图标大小: " + icon.Height + "x" + icon.Width;
                IsLoad = true;
                SaveBrowseBtn.IsEnabled = true;
                SaveImageSizeLabel.IsEnabled = true;
                SaveImageTypeLabel.IsEnabled = true;
                SavePathLabel.IsEnabled = true;
                SavePathTextBox.IsEnabled = true;
            }
        }

        private void PixelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selection = PixelComboBox.SelectedValue.ToString().Replace("System.Windows.Controls.ComboBoxItem: ", "").Replace("x", "");
            if (IsLoad)
            {
                ReviewIconImage.Height = double.Parse(selection);
                ReviewIconImage.Width = double.Parse(selection);
            }
        }

        private void SaveBrowseBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog saveDg = new System.Windows.Forms.SaveFileDialog();
            saveDg.Filter = "Icon file (*.ico)|";
            saveDg.FileName = "Icon.ico";
            saveDg.AddExtension = true;
            saveDg.RestoreDirectory = true;
            if (saveDg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filePath = saveDg.FileName;
                SavePathTextBox.Text = filePath;
                SaveImageBtn.IsEnabled = true;
            }
        }

        private void SaveImageBtn_Click(object sender, RoutedEventArgs e)
        {
            Image icon = System.Drawing.Icon.ExtractAssociatedIcon(exePath).ToBitmap();
            icon.Save(SavePathTextBox.Text.ToString());
        }
    }
}
