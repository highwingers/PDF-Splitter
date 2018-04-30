using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InnoWeb.Applications.Pdf.Splitter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void btnPdfSelector_Click(object sender, RoutedEventArgs e)
        {
            string fileName = GetFileName();
            txtFilePath.Text = fileName;

            if (!string.IsNullOrEmpty(fileName))
            {
                this.SplitPdf(fileName);
                MessageBox.Show("Done!");
            }   


        }

        private string GetFileName()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".pdf";
            dlg.Filter = "PDF Files|*.pdf";

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                // Open document 
                return dlg.FileName;

            }
            return null;
        }


        private void SplitPdf(string filename)
        {
            try
            {
                // Open the file
                PdfDocument inputDocument = PdfReader.Open(filename, PdfDocumentOpenMode.Import);

                string name = System.IO.Path.GetFileNameWithoutExtension(filename);

                var pageRanges = txtRange.Text.Split(';');

                foreach (var item in pageRanges)
                {
                    string fromPage = item.Split('-')[0];
                    string toPage = item.Split('-')[1];

                    CreatePdfDocument(inputDocument, fromPage, toPage);
                }



            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }


        }

        private void CreatePdfDocument(PdfDocument inputDocument, string _fromPg, string _toPg)
        {
            int fromPg = getPageNumber(_fromPg, 1) - 1;
            int toPg = getPageNumber(_toPg, inputDocument.PageCount);

            // Create new document
            PdfDocument outputDocument = new PdfDocument();

            for (int idx = fromPg; idx < toPg; idx++)
            {

                outputDocument.Version = inputDocument.Version;
                outputDocument.Info.Title =
                  String.Format("Page {0} of {1}", idx + 1, inputDocument.Info.Title);
                outputDocument.Info.Creator = inputDocument.Info.Creator;

                // Add the page and save it
                outputDocument.AddPage(inputDocument.Pages[idx]);

            }

            outputDocument.Save(String.Format("Page {0} - Page {1}.pdf", fromPg + 1, toPg));
        }

        private int getPageNumber(string pg, int defaultValue=0)
        {
            int result;
            int.TryParse(pg, out result);
            if (result > 0)
            {
                return result ;
            }

            return defaultValue;

        }

    }
}
