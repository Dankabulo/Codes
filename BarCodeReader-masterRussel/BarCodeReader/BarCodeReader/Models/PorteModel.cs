using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace BarCodeReader.Models
{
    public class PorteModel
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
