using System;
using System.Collections.Generic;
using System.Text;
using BarCodeReader.Models;
using System.Threading.Tasks;
using SQLite;

namespace BarCodeReader.Data
{
    public class BilletData
    {
        readonly SQLiteAsyncConnection _database;

        public BilletData(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<BilletModel>().Wait();
        }
        public Task<BilletModel> GetBillet(BilletModel b)
        {
            return _database.Table<BilletModel>()
                .Where(i => i.Matricule == b.Matricule)
                .FirstOrDefaultAsync();
        }
        public Task<List<BilletModel>> GetListAsync()
        {
            //return _database.Table<EtudiantModel>().ToListAsync();
            return _database.QueryAsync<BilletModel>("SELECT * FROM [BilletModel] ORDER BY [dateCreation] ASC");
        }
        public Task<List<BilletModel>> GetListeRecue()
        {
            return _database.QueryAsync<BilletModel>("SELECT * FROM [BilletModel] WHERE [etat] = 'true'");
        }
        public Task<List<BilletModel>> AccueillirInvite(string mat)
        {
            return _database.QueryAsync<BilletModel>("UPDATE [BilletModel] SET [etat] = 'true' WHERE [matricule] = '"+mat+"'");
        }
        public Task<int> SaveBillet(BilletModel e)
        {
            Task<List<BilletModel>> existence = check_enregistrement(e.Matricule);
            if (existence.Result.Count <= 0 || existence == null)

            {
                return _database.InsertAsync(e);
            }
            else
            {
                return _database.UpdateAsync(e);
            }
        }
        public Task<List<BilletModel>> DeleteListe()
        {
            return _database.QueryAsync<BilletModel>("DELETE FROM [BilletModel]");
        }
        public Task<List<BilletModel>> check_enregistrement(string matricule)
        {
            return _database.QueryAsync<BilletModel>("SELECT * FROM [BilletModel] WHERE [matricule]= ?", matricule);
        }
        public Task<List<BilletModel>> checkIfInvite(string mat)
        {
            return _database.QueryAsync<BilletModel>("SELECT * FROM [BilletModel] WHERE [matricule]= ? AND [etat] = 'FALSE'", mat);
        }
    }
}
