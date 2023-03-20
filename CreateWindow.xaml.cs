using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;

namespace Lab2
{
    /// <summary>
    /// Interaction logic for CreateWindow.xaml
    /// </summary>
    public partial class CreateWindow : Window
    {
        private string path;
        private bool success;
        public CreateWindow(string path)
        {
            this.path = path;
            this.success = false;
            InitializeComponent();
        }

        private void CreateOk(object sender, RoutedEventArgs e)
        {
            if (fileRadio.IsChecked == true)
            {
                string patternFile = @"^[a-zA-Z0-9_~\-]{1,8}\.(txt|php|html)$";
                if (!Regex.IsMatch(textbox.Text, patternFile))
                {
                    MessageBox.Show("Invalid file name", "Name error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                string patternDirectory = @"^[a-zA-Z0-9_~\-]{1,8}";
                if (!Regex.IsMatch(textbox.Text, patternDirectory))
                {
                    MessageBox.Show("Invalid directory name", "Name error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            
            this.Close();
            path = path + "\\" + textbox.Text;

            if ((bool)fileRadio.IsChecked)
            {
                File.Create(path);
            }
            else
            {
                Directory.CreateDirectory(path);
            }


            FileAttributes attributes = FileAttributes.Normal;
            if ((bool)rCheck.IsChecked)
            {
                attributes |= FileAttributes.ReadOnly;
            }
            if ((bool)aCheck.IsChecked)
            {
                attributes |= FileAttributes.Archive;
            }
            if ((bool)hCheck.IsChecked)
            {
                attributes |= FileAttributes.Hidden;
            }
            if ((bool)sCheck.IsChecked)
            {
                attributes |= FileAttributes.System;
            }
            File.SetAttributes(path, attributes);
            success = true;
        }

        private void CreateCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public string GetPath()
        {
            return path;
        }
        public bool GetSuccess()
        {
            return success;
        }
    }
}
