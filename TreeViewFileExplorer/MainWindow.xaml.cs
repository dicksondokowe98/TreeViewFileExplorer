using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TreeViewFileExplorer.ShellClasses;

namespace TreeViewFileExplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeFileSystemObjects();
        }
        public List<string> filesImported = new List<string>();
        public string batchName;

        private void InitializeFileSystemObjects()
        {
            var drives = DriveInfo.GetDrives();
            DriveInfo.GetDrives().ToList().ForEach(drive =>
            {
                var fileSystemObject = new FileSystemObjectInfo(drive);

                fileSystemObject.BeforeExplore += FileSystemObject_BeforeExplore;
                fileSystemObject.AfterExplore += FileSystemObject_AfterExplore;
                treeView.Items.Add(fileSystemObject);      
                
            });
        }

        private void FileSystemObject_AfterExplore(object sender, System.EventArgs e)
        {
            Cursor = Cursors.Arrow;
        }

        private void FileSystemObject_BeforeExplore(object sender, System.EventArgs e)
        {
            Cursor = Cursors.Wait;
        }

        private void SelectionChanged(object sender, RoutedPropertyChangedEventArgs<Object> e)
        {
            fileList.ItemsSource = "";
            List<Object> sampleFileList = new List<Object>();

            try
            {
                DirectoryInfo drive = (DirectoryInfo)((FileSystemObjectInfo)treeView.SelectedItem).FileSystemInfo;
                foreach(FileInfo fi in drive.GetFiles())
                {
                    if (fi.Extension == ".sample"/* || fi.Extension == ".collect"*/)
                        sampleFileList.Add(fi);
                }
                fileList.ItemsSource = sampleFileList;  
                

            } 
            catch (Exception ) {; }

        }


        private void FileListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FileNameComboBox.Text = "";
            foreach (FileInfo file in fileList.SelectedItems)
            {
                FileNameComboBox.Text += "\"" + file.Name + "\"" + " ";
            }
            FileNameComboBox.ItemsSource = fileList.SelectedItems;

            if (fileList.SelectedItems.Count < 2)
            {
                BatchName.IsEditable = false;
                BatchName.IsEnabled = false;
                BatchName.IsReadOnly = true;

            }
            else
            {
                BatchName.IsEditable = true;
                BatchName.IsEnabled = true;
                BatchName.IsReadOnly = false;

            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            batchName = BatchName.Text;
            foreach(FileInfo fi in fileList.SelectedItems)
            {
                filesImported.Add(fi.FullName);
            }

            if (filesImported.Count > 1)
            {
                if (string.IsNullOrWhiteSpace(batchName))                
                    batchName = DateTime.Now.ToString(@"yyyy-MM-dd");


                this.Close();
            }
            else if (filesImported.Count == 1)
            {
                batchName = "SingleSample"; //single samples aren't in a batch
                this.Close();
            }
            else {; }

            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
