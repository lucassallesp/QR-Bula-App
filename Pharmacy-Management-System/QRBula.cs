using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using QRCoder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;

namespace Pharmacy_Management_System
{
    public partial class QRBula : Form
    {
        public QRBula()
        {
            InitializeComponent();
        }

        readonly Farmacia usuario = new Farmacia();

        private void QRBula_Load(object sender, EventArgs e)
        {
            SqlDataReader temp = this.usuario.BuscarAtivos();
            medicineList.CurrentCell = null;
            ConfigurarGrade(temp);
        }

        private void GerarQRCode(string link)
        {
            if(link != null)
            {
                BulaForm bula = new BulaForm(usuario.Path);
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(link, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);

                qrCodeBox.Image = qrCode.GetGraphic(4);
                bula.QrCode = qrCodeBox.Image;
                bula.ShowDialog();
            }
            else
            {
                qrCodeBox.Image = null;
            }
            
        }

        private void ConfigurarGrade(SqlDataReader temp)
        {
            DataTable dt = new DataTable();
            dt.Load(temp);
            medicineList.DataSource = dt;

            medicineList.CurrentCell = null;
            medicineList.DefaultCellStyle.Font = new Font("Arial", 14);
            medicineList.AlternatingRowsDefaultCellStyle.Font = new Font("Arial", 14);
            medicineList.RowHeadersWidth = 25;
            medicineList.DefaultCellStyle.BackColor = Color.MediumAquamarine;
            medicineList.DefaultCellStyle.SelectionBackColor = Color.PaleTurquoise;
            medicineList.DefaultCellStyle.ForeColor = Color.Black;

            medicineList.Columns["nome_medicamento"].Width = 484;
            medicineList.Columns["nome_medicamento"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            medicineList.Columns["nome_medicamento"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            
            medicineList.ColumnHeadersVisible = false;
            medicineList.Columns["classe_terapeutica"].Visible = false;
            medicineList.Columns["Id"].Visible = false;
            medicineList.Columns["bula"].Visible = false;
            medicineList.Columns["path"].Visible = false;

            medicineList.Sort(medicineList.Columns["nome_medicamento"], ListSortDirection.Ascending);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            searchBox.Clear();
            SqlDataReader temp = this.usuario.BuscarAtivos();
            qrCodeBox.Image = null;
            medicineBox.Text = "";
            classeBox.Text = "";
            LimparCampos(usuario);
            ConfigurarGrade(temp);
        }

        private void LimparCampos(Farmacia user)
        {
            user.Path = null;
            user.Bula = null;
            user.Nome_medicamento = null;
            user.Classe_terapeutica = null;
            user.Id = -1; // -1 -> default para null
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string filtro = searchBox.Text;
            SqlDataReader temp = this.usuario.BuscarAtivos(filtro);
            qrCodeBox.Image = null;
            medicineBox.Text = "";
            classeBox.Text = "";
            LimparCampos(usuario);
            ConfigurarGrade(temp);
        }

        private void medicineList_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridViewRow linha = medicineList.Rows[e.RowIndex];

            usuario.Id = Convert.ToInt32(linha.Cells[0].Value.ToString());
            medicineBox.Text = linha.Cells[1].Value.ToString();
            usuario.Nome_medicamento = medicineBox.Text;
            classeBox.Text = linha.Cells[2].Value.ToString();
            usuario.Classe_terapeutica = classeBox.Text;
            usuario.Bula = linha.Cells[3].Value.ToString();
            usuario.Path = linha.Cells[4].Value.ToString();
            qrCodeBox.Image = null;
        }

        private void btnGenerate_Click_1(object sender, EventArgs e)
        {
            GerarQRCode(usuario.Bula);
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDashForm_Click(object sender, EventArgs e)
        {
            //Dashboard dashboard = new Dashboard();
            //dashboard.ShowDialog();
            
        }
    }
}
