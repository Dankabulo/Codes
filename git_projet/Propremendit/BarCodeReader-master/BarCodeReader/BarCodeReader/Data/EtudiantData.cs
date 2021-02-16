using System;
using System.Collections.Generic;
using System.Text;
using BarCodeReader.Models;
using System.Threading.Tasks;
using SQLite;

namespace BarCodeReader.Data
{
    public class EtudiantData
    {
        readonly SQLiteAsyncConnection _database;

        public EtudiantData(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<EtudiantModel>().Wait();
        }

        public Task<List<EtudiantModel>> GetListAsync()
        {
            return _database.Table<EtudiantModel>().ToListAsync();
        }

        public Task<EtudiantModel> GetEtudiantCoteAsync(int id)
        {
            return _database.Table<EtudiantModel>()
                .Where(i => i.Id == id)
                .FirstOrDefaultAsync();
        }

        public Task<int> SaveEtudiantCoteAsync(EtudiantModel etudiant)
        {
            if (etudiant.Id != 0)
            {
                return _database.UpdateAsync(etudiant);
            }
            else
            {
                return _database.InsertAsync(etudiant);
            }
        }

        public Task<int> DeleteEtudiantCoteAsync(EtudiantModel etudiant)
        {
            return _database.DeleteAsync(etudiant);
        }
    }
}
