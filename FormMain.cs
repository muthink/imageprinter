using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace imageprinter
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            comboBoxPrinters.Items.AddRange(PrinterSettings.InstalledPrinters.Cast<string>().ToArray());
        }
        private int _currentFile;
        private string[] _fileList;
        private Rectangle _pageSize;
        private PrintDocument _pd;

        private void buttonPrint_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    _fileList = listBoxImages.Items.Cast<string>().ToArray();
                    _pd = new PrintDocument();
                    _pd.PrinterSettings.PrinterName = comboBoxPrinters.SelectedItem.ToString();
                    _pd.DefaultPageSettings.PaperSize = new PaperSize("custom", (int)(numericUpDownWidth.Value*100), (int)(numericUpDownHeight.Value*100));
                    _pageSize = _pd.DefaultPageSettings.Bounds;
                    _pd.PrintPage += new PrintPageEventHandler(this.pd_PrintPage);
                    _pd.Print();
                }
                finally
                {
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        // The PrintPage event is raised for each page to be printed. 
        private void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {
            float leftMargin = ev.MarginBounds.Left;
            float topMargin = ev.MarginBounds.Top;

            Image image = Image.FromFile(_fileList[_currentFile]);
            ev.Graphics.DrawImage(image, _pageSize);

            if (++_currentFile >= _fileList.Length)
                ev.HasMorePages = false;
            else
                ev.HasMorePages = true;
        }


        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "Get files";
            fdlg.InitialDirectory = @"c:\";
            fdlg.Filter = "All files (*.*)|*.*|All files (*.*)|*.*";
            fdlg.FilterIndex = 2;
            fdlg.Multiselect = true;
            fdlg.CheckFileExists = true;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                listBoxImages.Items.AddRange(fdlg.FileNames);
            }
        }

        private void fillImagesFromDirectory(string path)
        {
            string[] files = Directory.GetFiles(path);
            listBoxImages.Items.AddRange(files);
        }

        private void buttonCustomPageSize_Click(object sender, EventArgs e)
        {

        }

        private void comboBoxPrinters_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            // Save the selected employee's name, because we will remove 
            // the employee's name from the list. 
            PrinterSettings ps = new PrinterSettings();
            ps.PrinterName = (string)(comboBoxPrinters.SelectedItem);
            comboBoxPageSize.Items.AddRange(ps.PaperSizes.Cast<PaperSize>().ToArray());
        }

        private void comboBoxPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            var paperSize = (PaperSize)comboBoxPageSize.SelectedItem;
            numericUpDownWidth.Value = paperSize.Width/(Decimal)100.0;
            numericUpDownHeight.Value = paperSize.Height / (Decimal)100.0;
        }
    }
}
