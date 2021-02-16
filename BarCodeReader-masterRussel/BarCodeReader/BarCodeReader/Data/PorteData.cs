using System;
using System.Collections.Generic;
using System.Text;
using BarCodeReader.Models;
using System.Threading.Tasks;
using SQLite;

namespace BarCodeReader.Data
{
    public class PorteData
    {
        readonly SQLiteAsyncConnection _database;

        public PorteData(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<PorteModel>().Wait();
        }
        public Task<int> SavePorte(PorteModel porte)
        {
            if (porte.ID != 0)
            {
                return _database.UpdateAsync(porte);
            }
            else
            {
                return _database.InsertAsync(porte);
            }
        }
        public Task<List<PorteModel>> DeletePorte()
        {
            return _database.QueryAsync<PorteModel>("DELETE FROM [PorteModel]");
        }
        public Task<List<PorteModel>> GetListAsync()
        {
            return _database.QueryAsync<PorteModel>("SELECT * FROM [PorteModel]");
        }
    }
}
