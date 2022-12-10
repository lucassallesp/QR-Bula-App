using PdfiumViewer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Printing;

namespace Pharmacy_Management_System
{
    public partial class BulaForm : Form
    {
        public Image QrCode { get; internal set; }

        PdfiumViewer.PdfViewer pdf;
        PictureBox qrcode = new PictureBox();
        public BulaForm(string path)
        {
            InitializeComponent();
            pdf = new PdfViewer();
            this.Controls.Add(pdf);
            OpenFile(path);
        }

        public void OpenFile(string file)
        {
            AppDomain root = AppDomain.CurrentDomain;
            AppDomainSetup setup = new AppDomainSetup();

            setup.ApplicationBase =
            root.SetupInformation.ApplicationBase + @"Bulas\";

            AppDomain domain = AppDomain.CreateDomain("MyDomain", null, setup);
            string filepath = setup.ApplicationBase + file;
            byte[] bytes = System.IO.File.ReadAllBytes(filepath);
            var stream = new System.IO.MemoryStream(bytes);
            PdfDocument pdfDocument = PdfDocument.Load(stream);
            pdf.Document = pdfDocument;
        }

        private void BulaForm_Load(object sender, EventArgs e)
        {
            pdf.Width = this.Width - 20;
            pdf.Height = this.Height - 40;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnQrBula_Click(object sender, EventArgs e)
        {
            if(printDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                printDocument1.PrinterSettings = printDialog1.PrinterSettings;
                printDocument1.Print();
            }
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawImage(QrCode, 100, 200, 231, 231);
        }
    }
}
