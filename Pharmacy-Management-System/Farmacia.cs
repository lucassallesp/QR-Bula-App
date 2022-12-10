using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pharmacy_Management_System
{
    class Farmacia
    {
        private string nome_medicamento;
        private string classe_terapeutica;
        private string bula;
        private int id;
        private string path;
        public Banco banco;


        public string Path { get => path; set => path = value; }
        public int Id { get => id; set => id = value; }
        public string Nome_medicamento { get => nome_medicamento; set => nome_medicamento = value; }
        public string Classe_terapeutica { get => classe_terapeutica; set => classe_terapeutica = value; }
        public string Bula { get => bula; set => bula = value; }

        public Farmacia()
        {
            this.banco = new Banco();
        }

        public SqlDataReader BuscarAtivos(string buscar = "")
        {
            SqlDataReader dr;
            string sql = "SELECT * FROM PharmacyQR.dbo.tbl_medicamentos";

            if(buscar != "")
            {
                sql += " WHERE nome_medicamento LIKE '" + buscar + "' OR classe_terapeutica LIKE '" + buscar + "'"; 
            }

            dr = this.banco.Query(sql);
            return dr;            
        }
    } 
}
