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
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace WinPDFMerge
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Pre-populate destination file path
            //string initialFilePath = Environment.GetEnvironmentVariable("USERPROFILE") + "\\Desktop\\NewPDF.pdf";
            //txtDestinationPath.Text = initialFilePath;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lstFiles.SelectedIndex > -1)
            {
                lstFiles.Items.RemoveAt(lstFiles.SelectedIndex);
            }

            lstFiles_AfterAddDelete();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog addDialog = new Microsoft.Win32.OpenFileDialog();

            addDialog.DefaultExt = ".pdf";
            addDialog.Filter = "PDF files (.pdf)|*.pdf";

            Nullable<bool> result = addDialog.ShowDialog();

            if (result == true)
            {
                ListBoxItem newItem = new ListBoxItem();
                newItem.Content = addDialog.FileName;
                lstFiles.Items.Add(newItem);
            }
        }

        // Movement algorithm shamelessly stolen from a StackOverflow answer. I'm a terrible person.
        private void btnMoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (lstFiles.SelectedIndex > 0)
            {
                ListBoxItem cloneItem = new ListBoxItem();
                ListBoxItem selectedItem = (ListBoxItem)lstFiles.SelectedItem;

                cloneItem.Content = selectedItem.Content;

                lstFiles.Items.Insert(lstFiles.SelectedIndex -1, cloneItem);
                lstFiles.SelectedIndex = lstFiles.SelectedIndex -2;
                lstFiles.Items.RemoveAt(lstFiles.Items.IndexOf(selectedItem));
            }
        }

        private void btnMoveDown_Click(object sender, RoutedEventArgs e)
        {

            if (lstFiles.SelectedIndex > -1 && lstFiles.SelectedIndex < lstFiles.Items.Count - 1)
            {
                ListBoxItem cloneItem = new ListBoxItem();
                ListBoxItem selectedItem = (ListBoxItem) lstFiles.SelectedItem;

                cloneItem.Content = selectedItem.Content;

                lstFiles.Items.Insert(lstFiles.SelectedIndex + 2, cloneItem);
                lstFiles.SelectedIndex = lstFiles.SelectedIndex + 2;
                lstFiles.Items.RemoveAt(lstFiles.Items.IndexOf(selectedItem));
            }
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog();

            saveDialog.DefaultExt = ".pdf";
            saveDialog.Filter = "PDF files (.pdf)|*.pdf";
            saveDialog.InitialDirectory = Environment.GetEnvironmentVariable("USERPROFILE") + "\\Desktop";

            Nullable<bool> result = saveDialog.ShowDialog();

            if (result == true)
            {
                txtDestinationPath.Text = saveDialog.FileName;
            }
        }

        private void btnMerge_Click(object sender, RoutedEventArgs e)
        {
            PdfDocument outputFile = new PdfDocument();

            // Loop over file paths in lstFiles
            for (int documentNumber = 0; documentNumber < lstFiles.Items.Count; documentNumber++)
            {
                ListBoxItem currentItem = (ListBoxItem)lstFiles.Items.GetItemAt(documentNumber);
                string currentPath = currentItem.Content.ToString();

                PdfDocument currentDocument = PdfReader.Open(currentPath, PdfDocumentOpenMode.Import);
                
                MessageBox.Show(currentDocument.FullPath);

                // Loop over pages in file, adding them to output
                for (int pageNumber = 0; pageNumber < currentDocument.PageCount; pageNumber++)
                {
                    outputFile.AddPage(currentDocument.Pages[pageNumber]);
                }
                
            }

            // Write file
            outputFile.Save(txtDestinationPath.Text);
        }

        // Helper function to toggle controls on/off based on whether 
        private void lstFiles_AfterAddDelete()
        {
            if (lstFiles.Items.Count > 0)
            {
                if (lstFiles.SelectedIndex > -1)
                {
                    btnDelete.IsEnabled = true;
                }                
                
                if (lstFiles.Items.Count > 1)
                {
                    btnMoveDown.IsEnabled = true;
                    btnMoveUp.IsEnabled = true;
                }

                btnMerge_toggle();
            }
            else
            {
                btnDelete.IsEnabled = false;
                btnMoveDown.IsEnabled = false;
                btnMoveUp.IsEnabled = false;
                btnMerge.IsEnabled = false;
            }
        }

        private void lstFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstFiles.SelectedIndex > -1)
            {
                btnDelete.IsEnabled = true;

                if (lstFiles.Items.Count > 1)
                {
                    btnMoveUp.IsEnabled = true;
                    btnMoveDown.IsEnabled = true;
                }
            }
            else
            {
                btnMoveUp.IsEnabled = false;
                btnMoveDown.IsEnabled = false;
                btnDelete.IsEnabled = false;
            }
        }

        private void btnMerge_toggle()
        {
            if (txtDestinationPath.Text.Length > 0 && lstFiles.Items.Count > 0) // The merge button should only be enabled if the path is filled in
            {
                btnMerge.IsEnabled = true;
            }
        }

        private void txtDestinationPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnMerge_toggle();
        }
    }
}
