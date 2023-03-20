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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Lab2
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Open(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderBrowserDialog() { Description = "Select directory to open" };
            var result = dlg.ShowDialog();
            DirectoryInfo directoryInfo = new DirectoryInfo(dlg.SelectedPath);
            var root = new TreeViewItem
            {
                Header = directoryInfo.Name,
                Tag = directoryInfo.FullName
            };
            root.ContextMenu = new System.Windows.Controls.ContextMenu();
            var menuItem1 = new MenuItem { Header = "Create" };
            var menuItem2 = new MenuItem { Header = "Delete" };
            menuItem1.Click += new RoutedEventHandler(CreateFile);
            menuItem2.Click += new RoutedEventHandler(DeleteFile);
            root.ContextMenu.Items.Add(menuItem1);
            root.ContextMenu.Items.Add(menuItem2);
            root.Selected += new RoutedEventHandler(StatusBarUpdate);
            root.IsExpanded = true;
            treeView.Items.Add(root);
            HandleDirectory(directoryInfo, root);
        }
        private void Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void HandleDirectory(DirectoryInfo root, TreeViewItem parent)
        {
            foreach (FileInfo file in root.GetFiles())
            {
                var item = new TreeViewItem
                {
                    Header = file.Name,
                    Tag = file.FullName
                };
                item.ContextMenu = new System.Windows.Controls.ContextMenu();
                var menuItem1 = new MenuItem { Header = "Open" };
                var menuItem2 = new MenuItem { Header = "Delete" };
                menuItem1.Click += new RoutedEventHandler(OpenFile);
                menuItem2.Click += new RoutedEventHandler(DeleteFile);
                item.ContextMenu.Items.Add(menuItem1);
                item.ContextMenu.Items.Add(menuItem2);
                item.Selected += new RoutedEventHandler(StatusBarUpdate);
                parent.Items.Add(item);
            }
            foreach (DirectoryInfo directory in root.GetDirectories())
            {
                var item = new TreeViewItem
                {
                    Header = directory.Name,
                    Tag = directory.FullName
                };
                item.ContextMenu = new System.Windows.Controls.ContextMenu();
                var menuItem1 = new MenuItem { Header = "Create" };
                var menuItem2 = new MenuItem { Header = "Delete" };
                menuItem1.Click += new RoutedEventHandler(CreateFile);
                menuItem2.Click += new RoutedEventHandler(DeleteFile);
                item.ContextMenu.Items.Add(menuItem1);
                item.ContextMenu.Items.Add(menuItem2);
                item.Selected += new RoutedEventHandler(StatusBarUpdate);
                item.IsExpanded = true;
                parent.Items.Add(item);
                HandleDirectory(directory, item);
            }
        }

        private void DeleteFile(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)treeView.SelectedItem;
            string path = (string)item.Tag;
            FileAttributes attributes = File.GetAttributes(path);
            File.SetAttributes(path, attributes & ~FileAttributes.ReadOnly);
            if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                DeleteDirectory(path);
            }
            else
            {
                File.Delete(path);
            }
            if ((TreeViewItem)treeView.Items[0] != item)
            {
                TreeViewItem parent = (TreeViewItem)item.Parent;
                parent.Items.Remove(item);
            }
            else
            {
                treeView.Items.Clear();
            }
        }
        private void DeleteDirectory(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            foreach (var subdir in dir.GetDirectories())
            {
                DeleteDirectory(subdir.FullName);
            }
            foreach (var file in dir.GetFiles())
            {
                File.SetAttributes(file.FullName, FileAttributes.Normal);
                File.Delete(file.FullName);
            }
            Directory.Delete(path);
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)treeView.SelectedItem;
            string content = File.ReadAllText((string)item.Tag);
            scrollViewer.Content = new TextBlock() { Text = content };
        }

        private void CreateFile(object sender, RoutedEventArgs e)
        {
            TreeViewItem parent = (TreeViewItem)treeView.SelectedItem;
            CreateWindow createWindow = new CreateWindow(parent.Tag.ToString());
            createWindow.ShowDialog();
            if (createWindow.GetSuccess())
            {
                if (File.Exists(createWindow.GetPath()))
                {
                    FileInfo file = new FileInfo(createWindow.GetPath());
                    var item = new TreeViewItem
                    {
                        Header = file.Name,
                        Tag = file.FullName
                    };
                    item.ContextMenu = new System.Windows.Controls.ContextMenu();
                    var menuItem1 = new MenuItem { Header = "Open" };
                    var menuItem2 = new MenuItem { Header = "Delete" };
                    menuItem1.Click += new RoutedEventHandler(OpenFile);
                    menuItem2.Click += new RoutedEventHandler(DeleteFile);
                    item.ContextMenu.Items.Add(menuItem1);
                    item.ContextMenu.Items.Add(menuItem2);
                    item.Selected += new RoutedEventHandler(StatusBarUpdate);
                    parent.Items.Add(item);
                }
                else if (Directory.Exists(createWindow.GetPath()))
                {
                    DirectoryInfo directory = new DirectoryInfo(createWindow.GetPath());
                    var item = new TreeViewItem
                    {
                        Header = directory.Name,
                        Tag = directory.FullName
                    };
                    item.ContextMenu = new System.Windows.Controls.ContextMenu();
                    var menuItem1 = new MenuItem { Header = "Create" };
                    var menuItem2 = new MenuItem { Header = "Delete" };
                    menuItem1.Click += new RoutedEventHandler(CreateFile);
                    menuItem2.Click += new RoutedEventHandler(DeleteFile);
                    item.ContextMenu.Items.Add(menuItem1);
                    item.ContextMenu.Items.Add(menuItem2);
                    item.Selected += new RoutedEventHandler(StatusBarUpdate);
                    parent.Items.Add(item);
                }
            }
        }
        private void StatusBarUpdate(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)treeView.SelectedItem;
            FileAttributes attributes = File.GetAttributes((string)item.Tag);
            statusRASH.Text = "";
            if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                statusRASH.Text += 'r';
            }
            else
            {
                statusRASH.Text += '-';
            }
            if ((attributes & FileAttributes.Archive) == FileAttributes.Archive)
            {
                statusRASH.Text += 'a';
            }
            else
            {
                statusRASH.Text += '-';
            }
            if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
            {
                statusRASH.Text += 'h';
            }
            else
            {
                statusRASH.Text += '-';
            }
            if ((attributes & FileAttributes.System) == FileAttributes.System)
            {
                statusRASH.Text += 's';
            }
            else
            {
                statusRASH.Text += '-';
            }
        }

    }
}
