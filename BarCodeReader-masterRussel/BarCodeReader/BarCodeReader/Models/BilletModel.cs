using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
namespace BarCodeReader.Models
{
    public class BilletModel
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string Matricule { get; set; }
        public string team1 { get; set; }
        public string team2 { get; set; }
        public string jour { get; set; }
        public string place { get; set; }
        public string stade { get; set; }
        public string dateCreation { get; set; }
        public string etat { get; set; }
    }
}
