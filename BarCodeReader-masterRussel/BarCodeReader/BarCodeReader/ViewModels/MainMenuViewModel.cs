using DocumentFormat.OpenXml;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using BarCodeReader.Interfaces;
using BarCodeReader.Models;
using BarCodeReader.Services;
using BarCodeReader.Views;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Cell = DocumentFormat.OpenXml.Spreadsheet.Cell;
using System.Collections.Generic;

namespace BarCodeReader.ViewModels
{
    public class MainMenuViewModel : BaseViewModel
    {
        
        public MainMenuViewModel()
        {
            Title = "Access Scan";
            AccueilCommand = new Command(PageAccueil);
            PageConfigurationcommand = new Command(async () => await PageConfiguration());
            ScanCommand = new Command(async () => await Scannning());
            ConfigurationCommand = new Command(configurations);
            DeleteCommand = new Command(DemandeReinitialisation);
            ImportationCommand = new Command(FormulaireImportation);
            ChangerPorteCommand = new Command(FormulaireChangerPorte);
            //HistoriqueViewCommand = new Command(HistoriqueView);
            //SearchViewCommand = new Command(SearchView);
            //ExporterCommad = new Command(async () => await ExportDataToExcelAsync());
            //DeleteListeCommad = new Command(ReinitialiserListe);
            //SendCommand = new Command(async () => await ShareResult());
            LoadData();
        }
        // private async void HistoriqueView()
        // {
        //     await App.Current.MainPage.Navigation.PushAsync(new Enregistrements()
        //     {
        //         Title = "Historique"
        //     });
        // }
        // public async Task AfterDeleteOne()
        // {
        //     //await App.Current.MainPage.Navigation.PopAsync();
        //     await App.Current.MainPage.Navigation.PushAsync(new Enregistrements()
        //     {
        //         Title = "Historique"
        //     });
        // }
        // private async void SearchView()
        // {
        //     await App.Current.MainPage.Navigation.PushAsync(new SearchView()
        //     {
        //         Title = "Recherche"
        //     });
        // }
        // private async Task ShareText(string text)
        // {
        //     await Share.RequestAsync(new ShareTextRequest
        //     {
        //         Text = text,
        //         Title = "Share"
        //     });
        // }
        // //private async Task ShareResult()
        // //{
        // //    if (Liste.Count() <= 0)
        // //    {
        // //        MessagingCenter.Send(this, "NoScanResultToShare");
        // //    }
        // //    else
        // //    {
        // //        await this.ExportDataToExcelAsync();
        // //        string date = DateTime.Now.ToShortDateString();
        // //        date = date.Replace("/", "_");

        // //        var path = DependencyService.Get<IExportFilesToLocation>().GetFolderLocation() + "Export_cotation" + date + ".xlsx";
        // //        var file = path;

        // //        await Share.RequestAsync(new ShareFileRequest
        // //        {
        // //            Title = "Exportation",
        // //            File = new ShareFile(file)
        // //        });
        // //    }
        // //}
        private async void LoadData()
        {
            Liste = new ObservableCollection<BilletModel>(await App.Database.GetListAsync());
            Listerecue = new ObservableCollection<BilletModel>(await App.Database.GetListeRecue());
            Porte = new ObservableCollection<PorteModel>(await App.DatabasePorte.GetListAsync());
        }
        private async Task Scannning()
        {
            var scanPage = new ZXingScannerPage
            {
                Title = "Veuillez cibler le Qr code",
                BackgroundColor = System.Drawing.Color.FromArgb(43)
            };
            await App.Current.MainPage.Navigation.PushAsync(scanPage);
            scanPage.OnScanResult += (result) =>
            {
                // Stop scanning
                scanPage.IsScanning = false;

                // Pop to page and show the result
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await App.Current.MainPage.Navigation.PopAsync();

                    string text = result?.Text;
                    await Saving(text);
                });
            };
        }
        public async Task Operating(string resultat)
        {
            if (string.IsNullOrWhiteSpace(resultat))
            {
                await App.Current.MainPage.Navigation.PushAsync(new RefusView()
                {
                    Title = "Etat"
                });
            }
            else
            {
                await App.Current.MainPage.Navigation.PushAsync(new AccessView()
                {
                    Title = "Etat"
                });
            }
        }
        public async void PageAccueil()
        {
            await App.Current.MainPage.Navigation.PopAsync();
        }
        public void configurations()
        {
            MessagingCenter.Send(this, "Authentification");
        }
        public async Task PageConfiguration()
        {
            // titre
            Liste = new ObservableCollection<BilletModel>(await App.Database.GetListAsync());
            Listerecue = new ObservableCollection<BilletModel>(await App.Database.GetListeRecue());
            int nb = Liste.Count();
            int nbe = Listerecue.Count();

            
            // titre
            await App.Current.MainPage.Navigation.PushAsync(new ImportationView()
            {
                Title = "Billets reçus(" + nbe + "/" + nb + ")"
            }) ;
        }
        public void FormulaireImportation()
        {
            MessagingCenter.Send(this, "FormulaireImportation");
        }
        public async Task OnImportation(string enregistremnts)
        {
            if (enregistremnts.Contains(';') && enregistremnts.Contains('_'))
            {
                string enregistrementsSpaced = enregistremnts.Replace("\r\n", "");
                string[] data = enregistrementsSpaced.Split(new char[] { ';' });   // decoupe en tableau des enregistrements separes

                foreach (string i in data)                                   // pour chaque enregistrements du tableau
                {
                    string[] billet = i.Split(new char[] { '_' });          // decoupe l'enregistrements en proprietes

                    var d = new BilletModel
                    {
                        Matricule = billet[0],
                        team1 = billet[1],
                        team2 = billet[2],
                        jour = billet[3],
                        place = billet[4],
                        dateCreation = billet[5],
                        etat = "FALSE"
                    };
                    await App.Database.SaveBillet(d);
                }
                // titre
                Liste = new ObservableCollection<BilletModel>(await App.Database.GetListAsync());
                Listerecue = new ObservableCollection<BilletModel>(await App.Database.GetListeRecue());
                int nb = Liste.Count();
                int nbe = Listerecue.Count();

                Title = "Billets reçus(" + nbe + "/" + nb + ")";
                // titre
                MessagingCenter.Send(this, "ImportationTerminee");
                //await App.Current.MainPage.Navigation.PopAsync();
            }
            else
            {
                MessagingCenter.Send(this, "NonreconnuEnregistrements");
            }
        }
        public async Task Saving(string text)
        {
            List<BilletModel> e = await App.Database.check_enregistrement(text);
            List<BilletModel> f = await App.Database.checkIfInvite(text);
            List<PorteModel> porteIntiale = await App.DatabasePorte.GetListAsync();

            if (e.Count() > 0) // si l'invite figure dans nos enregistrements
            {
                Billet = e.ToArray();   ////  --------- je ne comprends pas pourquoi 
                BilletModel g = Billet[0];

                // proprietes de la porte
                TbPorte = porteIntiale.ToArray();
                PorteModel porte = TbPorte[0];

                if (f.Count() > 0)  // s'il ne pas encore note comme etant entree
                {
                    if (g.place == porte.Name) // si son entree correspond a l'entree initiale
                    {
                        await App.Database.AccueillirInvite(text);
                        //LoadData();
                        Listerecue = new ObservableCollection<BilletModel>(await App.Database.GetListeRecue());
                        int nb = Listerecue.Count();
                        await App.Current.MainPage.Navigation.PushAsync(new AccessView()
                        {
                            Title = "Billet numero : " + nb
                        });
                    }
                    else        // si son entree ne correspond pas a l'entree initiale du terminal present
                    {
                        await App.Current.MainPage.Navigation.PushAsync(new WrongView(g)
                        {
                            Title = "Terminal non permis"
                        });
                    }
                }
                else  // deja recue
                {
                    await App.Current.MainPage.Navigation.PushAsync(new DejaRecu()
                    {
                        Title = "Attention"
                    });
                }
            }
            else  // si l'invite n'existe pas dans nos enregistrements
            {
                await App.Current.MainPage.Navigation.PushAsync(new RefusView()
                {
                    Title = "Alerte"
                });
            }
        }
        public void DemandeReinitialisation()
        {
            MessagingCenter.Send(this, "AskReinitialisation");
        }
        public async Task Reinitialisation()
        {
            await App.Database.DeleteListe();

            // titre
            Liste = new ObservableCollection<BilletModel>(await App.Database.GetListAsync());
            Listerecue = new ObservableCollection<BilletModel>(await App.Database.GetListeRecue());
            int nb = Liste.Count();
            int nbe = Listerecue.Count();

            Title = "Billets reçus(" + nbe + "/" + nb + ")";
            // titre
            MessagingCenter.Send(this, "Finreinitialisation");
        }
        public void FormulaireChangerPorte()
        {
            MessagingCenter.Send(this, "FormulaireChangerPorte");
        }
        public async Task Terminal(string texte)
        {
            PorteModel porte = new PorteModel();
            if (texte == "Tribune d'honneur")
            {
                porte.Name = "Tribune d'honneur";
                await changerTerminal(porte);
            }
            else if (texte == "VIP")
            {
                porte.Name = "VIP";
                await changerTerminal(porte);
            }
            else if (texte == "Laterale A")
            {
                porte.Name = "Laterale A";
                await changerTerminal(porte);
            }
            else if (texte == "Laterale B")
            {
                porte.Name = "Laterale B";
                await changerTerminal(porte);
            }
            else if (texte == "Laterale C")
            {
                porte.Name = "Laterale C";
                await changerTerminal(porte);
            }
            else if (texte == "Laterale D")
            {
                porte.Name = "Laterale D";
                await changerTerminal(porte);
            }
        }
        public async Task changerTerminal(PorteModel porte)
        {
            await App.DatabasePorte.DeletePorte();
            await App.DatabasePorte.SavePorte(porte);
            MessagingCenter.Send(this, "PorteChangee");
        }
        // //public async void SaveCote(string[] data, string cote)
        // //{
        // //    var d = new EtudiantModel();
        // //    d.Matricule = data[1];
        // //    d.Nom = data[2];
        // //    d.Postnom = data[3];
        // //    d.Prenom = data[4];
        // //    d.Filiere = data[5];
        // //    d.Cours = data[6];
        // //    d.Epreuve = data[7];
        // //    d.Cote_max = data[8];
        // //    d.Date = data[9];
        // //    d.Cote = cote;
        // //    await App.Database.SaveEtudiantCoteAsync(d);
        // //}
        // //public void RefreshListCotation(ListView v)
        // //{
        // //    v.ItemsSource = Liste;
        // //}
        // //public void ReinitialiserListe()
        // //{
        // //    if (Liste.Count() > 0)
        // //    {
        // //        MessagingCenter.Send(this, "ResultatASupprimer");
        // //    }
        // //    else
        // //    {
        // //        MessagingCenter.Send(this, "NoresultatDelete");
        // //    }
        // //}
        // //public async Task DeleteEtudiant(EtudiantModel etudiant)
        // //{
        // //    await App.Database.DeleteEtudiantCoteAsync(etudiant);
        // //}
        // //public async Task SingletDelete(string matricule)
        // //{
        // //    await App.Database.DeleteOne(matricule);
        // //    Liste = new ObservableCollection<EtudiantModel>(await App.Database.GetListAsync());
        // //}
        // //public async Task DeleteListe()
        // //{
        // //    await App.Database.DeleteListe();
        // //    //
        // //    Liste = new ObservableCollection<EtudiantModel>(await App.Database.GetListAsync());
        // //    MessagingCenter.Send(this, "DatadeleteSucess");
        // //}
        // //public async System.Threading.Tasks.Task Export()
        // //{
        // //    // Granted storage permission
        // //    var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

        // //    if (storageStatus != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
        // //    {
        // //        var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Storage });
        // //        storageStatus = results[Permission.Storage];
        // //    }

        // //    try
        // //    {
        // //        string date = DateTime.Now.ToShortDateString();
        // //        date = date.Replace("/", "_");

        // //        var path = DependencyService.Get<IExportFilesToLocation>().GetFolderLocation() + "Export_cotation" + date + ".xlsx";
        // //        FilePath = path;
        // //        using (SpreadsheetDocument document = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook))
        // //        {
        // //            WorkbookPart workbookPart = document.AddWorkbookPart();
        // //            workbookPart.Workbook = new Workbook();

        // //            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
        // //            worksheetPart.Worksheet = new Worksheet();

        // //            Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
        // //            Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Liste des cotes" };
        // //            sheets.Append(sheet);

        // //            workbookPart.Workbook.Save();

        // //            SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

        // //            // Constructing header
        // //            Row row = new Row();

        // //            row.Append(
        // //                ConstructCell("Matricule", CellValues.String),
        // //                ConstructCell("Nom", CellValues.String),
        // //                ConstructCell("Post Nom", CellValues.String),
        // //                ConstructCell("Prenom", CellValues.String),
        // //                ConstructCell("Epreuve", CellValues.String),
        // //                ConstructCell("Cote", CellValues.String)
        // //                );

        // //            // Insert the header row to the Sheet Data
        // //            sheetData.AppendChild(row);

        // //            // Add each product
        // //            foreach (var d in Liste)
        // //            {
        // //                row = new Row();
        // //                row.Append(
        // //                    ConstructCell(d.Matricule.ToString(), CellValues.String),
        // //                    ConstructCell(d.Nom.ToString(), CellValues.String),
        // //                    ConstructCell(d.Postnom, CellValues.String),
        // //                    ConstructCell(d.Prenom, CellValues.String),
        // //                    ConstructCell(d.Epreuve, CellValues.String),
        // //                    ConstructCell(d.Cote, CellValues.String));
        // //                sheetData.AppendChild(row);
        // //            }

        // //            worksheetPart.Worksheet.Save();
        // //        }
        // //    }
        // //    catch (Exception e)
        // //    {
        // //        Debug.WriteLine("ERROR: " + e.Message);
        // //    }
        // //}
        // //public async System.Threading.Tasks.Task ExportDataToExcelAsync()
        // //{
        // //    if (Liste.Count() > 0)
        // //    {
        // //        await Export();
        // //        MessagingCenter.Send(this, "DataExportedSuccessfully");
        // //    }
        // //    else
        // //    {
        // //        MessagingCenter.Send(this, "NoDataToExport");
        // //    }
        // //}
        // private Cell ConstructCell(string value, CellValues dataType)
        // {
        //     return new Cell()
        //     {
        //         CellValue = new CellValue(value),
        //         DataType = new EnumValue<CellValues>(dataType)
        //     };
        // }
        public ICommand ScanCommand { get; private set; }
        // public ICommand HistoriqueViewCommand { get; private set; }
        // public ICommand ExporterCommad { get; set; }
        // public ICommand SearchViewCommand { get; set; }
        // public ICommand RefreshCommand { get; set; }
        //// public ICommand DeleteEtudiantCommad { get; set; }
        // public ICommand DeleteListeCommad { get; set; }
        // public ICommand SendCommand { get; set; }
        public ICommand PageConfigurationcommand { get; set; }
        public ICommand ImportationCommand { get; set; }
        public ICommand ConfigurationCommand { get; set; }
        public ICommand AccueilCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand ChangerPorteCommand { get; set; }

        private ObservableCollection<BilletModel> _liste;
        public ObservableCollection<BilletModel> Liste
        {
            get { return _liste; }
            set { SetProperty(ref _liste, value); }
        }
        private ObservableCollection<BilletModel> _listeRecue;
        public ObservableCollection<BilletModel> Listerecue
        {
            get { return _listeRecue; }
            set { SetProperty(ref _listeRecue, value); }
        }
        private ObservableCollection<PorteModel> _porte;
        public ObservableCollection<PorteModel> Porte
        {
            get { return _porte; }
            set { SetProperty(ref _porte, value); }
        }
        private string[] _scanResult;
        public string[] ScanResult
        {
            get { return _scanResult; }
            set { SetProperty(ref _scanResult, value); }
        }
        // private string _filePath;
        // private ListView _listView;
        // public ListView listView
        // {
        //     get { return _listView; }
        //     set { SetProperty(ref _listView, value); }
        // }

        // public string FilePath
        // {
        //     get { return _filePath; }
        //     set { SetProperty(ref _filePath, value); }
        // }
        private BilletModel[] _billet;
        public BilletModel[] Billet
        {
            get { return _billet; }
            set { SetProperty(ref _billet, value); }
        }
        private PorteModel[] _tbPorte;
        public PorteModel[] TbPorte
        {
            get { return _tbPorte; }
            set { SetProperty(ref _tbPorte, value); }
        }

    }
}
