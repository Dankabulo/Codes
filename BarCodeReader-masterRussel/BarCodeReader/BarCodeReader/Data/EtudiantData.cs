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
            //_database.CreateTableAsync<Compte>().Wait();
        }

        public Task<List<EtudiantModel>> GetListAsync()
        {
            //return _database.Table<EtudiantModel>().ToListAsync();
            return _database.QueryAsync<EtudiantModel>("SELECT * FROM [EtudiantModel] ORDER BY [Matricule] ASC");
        }
        public Task<EtudiantModel> GetEtudiantCoteAsync(EtudiantModel e)
        {
            return _database.Table<EtudiantModel>()
                .Where(i => i.Matricule == e.Matricule)
                .FirstOrDefaultAsync();
        }
        public Task<int> SaveEtudiantCoteAsync(EtudiantModel etudiant)
        {
            Task<List<EtudiantModel>> existence = check_enregistrement(etudiant);
            if (existence.Result.Count <= 0 || existence == null)
            {
                return _database.InsertAsync(etudiant);
            }
            else
            {
               return _database.UpdateAsync(etudiant);  
            }

        }
        public Task<int> DeleteEtudiantCoteAsync(EtudiantModel etudiant)
        {
            return _database.DeleteAsync(etudiant);
        }
        public Task<List<EtudiantModel>> DeleteListe()
        {
            // return _database.Table<EtudiantModel>().DeleteAsync();
            return _database.QueryAsync<EtudiantModel>("DELETE FROM [etudiantModel]");
        }
        public Task<List<EtudiantModel>> DeleteOne(string mat)
        {
            return _database.QueryAsync<EtudiantModel>("DELETE FROM [etudiantModel] WHERE [matricule]= ?", mat);
        }
        public Task<List<EtudiantModel>> check_enregistrement(EtudiantModel e)
        {
            return _database.QueryAsync<EtudiantModel>("SELECT * FROM [etudiantModel] WHERE [matricule]= ?",e.Matricule);
        }
    }
}
