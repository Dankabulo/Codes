using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace BarCodeReader.Models
{
    public class EtudiantModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Postnom { get; set; }
        public string Prenom { get; set; }
        public string Filiere { get; set; }
        public string Cours { get; set; }
        public string Epreuve { get; set; }
        public string Cote_max { get; set; }
        public string Date { get; set; }
        public string Cote { get; set; }
    }
}
