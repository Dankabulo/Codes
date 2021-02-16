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
            _database.CreateTableAsync<FiliereModel>().Wait();
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
        public Task<List<EtudiantModel>> DeleteEtudiantCoteAsync(EtudiantModel etudiant)
        {
            return _database.QueryAsync<EtudiantModel>("UPDATE [EtudiantModel] SET [Cours] ='', [Epreuve] = '', [Cote_max] = '', [Date] = '' ,[Cote] = '' WHERE [Matricule] = '" + etudiant.Matricule + "'");
        }
        public Task<List<EtudiantModel>> DeleteListe()
        {
            // return _database.Table<EtudiantModel>().DeleteAsync();
            return _database.QueryAsync<EtudiantModel>("DELETE FROM [etudiantModel]");
        }
        public Task<List<EtudiantModel>> DeleteOne(string mat)
        {
            return _database.QueryAsync<EtudiantModel>("UPDATE [EtudiantModel] SET [Cours] ='', [Epreuve] = '', [Cote_max] = '', [Date] = '' ,[Cote] = '' WHERE [Matricule] = ?", mat);
        }
        public Task<List<EtudiantModel>> check_enregistrement(EtudiantModel e)
        {
            return _database.QueryAsync<EtudiantModel>("SELECT * FROM [etudiantModel] WHERE [matricule]= ?",e.Matricule);
        }
        public Task<List<FiliereModel>> DeleteFiliereExistante()
        {
            return _database.QueryAsync<FiliereModel>("DELETE FROM [FiliereModel]");
        }
        public Task<int> ChangerFiliere(FiliereModel filiere)
        {
            DeleteFiliereExistante();
            return _database.InsertAsync(filiere);
        }
        public Task<List<FiliereModel>> GetListeFiliere()
        {
            return _database.QueryAsync<FiliereModel>("SELECT * FROM [FiliereModel]");
        }
        public Task<List<EtudiantModel>> GetEnregistrementCible(string filiere)
        {
            return _database.QueryAsync<EtudiantModel>("SELECT * FROM [EtudiantModel] WHERE [filiere] = '"+filiere+"' ORDER BY [Matricule] ASC");
        }
        public Task<List<EtudiantModel>> GetEnregistrementCibleCote(string filiere)
        {
            return _database.QueryAsync<EtudiantModel>("SELECT * FROM [EtudiantModel] WHERE [filiere] = '" + filiere + "' AND [Cote] IS NOT NULL ORDER BY [Matricule] ASC");
        }
        public Task<int> RemplirListeFiliere(EtudiantModel etudiant)
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
        public Task<List<EtudiantModel>> DeleteEnregistrementFiliere(FiliereModel filiere)
        {
            return _database.QueryAsync<EtudiantModel>("DELETE * FROM [EtudiantModel] WHERE [Filiere] = '"+filiere.Filiere+"'");
        }
        public Task<List<EtudiantModel>> ReinitialiserListeCible(string filiere)
        {
            return _database.QueryAsync<EtudiantModel>("UPDATE [EtudiantModel] SET [Cours] ='', [Epreuve] = '', [Cote_max] = '', [Date] = '' ,[Cote] = '' WHERE [Filiere] = '"+filiere+"'");
        }
        public Task<List<EtudiantModel>> EnregistrerCoteEtudiant(string matricule, string cours, string epreuve, string cote_max, string cote, string date)
        {
            return _database.QueryAsync<EtudiantModel>("UPDATE [EtudiantModel] SET [Cours] ='"+cours+"', [Epreuve] = '"+epreuve+"', [Cote_max] = '"+cote_max+"', [Date] = '"+date+"' ,[Cote] = '"+cote+"' WHERE [Matricule] = '" + matricule + "'");
        }
        public Task<List<EtudiantModel>> CheckEnregistrementFiliereCible(string matricule, string filiere)
        {
            return _database.QueryAsync<EtudiantModel>("SELECT * FROM [etudiantModel] WHERE [matricule]= '"+matricule+"' AND [Filiere] = '"+filiere+"'");
        }
    }
}
