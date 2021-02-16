using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BarCodeReader.Models;
using BarCodeReader.Data;
using BarCodeReader.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.FilePicker;
using DocumentFormat.OpenXml;
using System.Windows.Input;
using Xamarin.Essentials;
using ZXing.Net.Mobile.Forms;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using BarCodeReader.Interfaces;
using BarCodeReader.Services;
using BarCodeReader.Views;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Cell = DocumentFormat.OpenXml.Spreadsheet.Cell;
using Syncfusion.XlsIO;
using System.Reflection;
using Color = Syncfusion.Drawing.Color;
using System.IO;

namespace BarCodeReader.ViewModels
{
    public class MainMenuViewModel : BaseViewModel
    {
        
        public MainMenuViewModel()
        {
            Title = "Merles";
            ScanCommand = new Command(async () => await GetStatusListeBeginScanning());
            HistoriqueViewCommand = new Command(HistoriqueView);
            SearchViewCommand = new Command(SearchView);
            ExporterCommad = new Command(async () => await ExportDataToExcelAsync());
            DeleteListeCommad = new Command(ReinitialiserListe);
            SendCommand = new Command(async () => await ShareResult());
            ListeConfigurationCommand = new Command(GererListe);
            LoadData();
            AfficherOptions();
        }
        private async void HistoriqueView()
        {
            Liste = new ObservableCollection<EtudiantModel>(await App.Database.GetListAsync());
            Filiere = new ObservableCollection<FiliereModel>(await App.Database.GetListeFiliere());

            foreach (FiliereModel f in Filiere)
            {
                //await App.Current.MainPage.Navigation.PushAsync(new Enregistrements()
                //{
                //    Title = "Historique"
                //});
                string filiereCible = f.Filiere;
                ListeCible = new ObservableCollection<EtudiantModel>(await App.Database.GetEnregistrementCible(filiereCible));
            }
            await App.Current.MainPage.Navigation.PushAsync(new Enregistrements()
            {
                Title = "Historique"
            });
        }
        public async Task AfterDeleteOne()
        {
            //await App.Current.MainPage.Navigation.PopAsync();
            await App.Current.MainPage.Navigation.PushAsync(new Enregistrements()
            {
                Title = "Historique"
            });
        }
        private async void SearchView()
        {
            await App.Current.MainPage.Navigation.PushAsync(new SearchView()
            {
                Title = "Recherche"
            });
        }
        private async Task ShareText(string text)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = text,
                Title = "Share"
            });
        }
        private async Task ShareResult()
        {
            if (ListeCible.Count() <= 0)
            {
                MessagingCenter.Send(this, "NoScanResultToShare");
            }
            else
            {
                await this.ExportDataToExcelAsync();
                string date = DateTime.Now.ToShortDateString();
                date = date.Replace("/", "_");

                var path = DependencyService.Get<IExportFilesToLocation>().GetFolderLocation() + "Export_cotation" + date + ".xlsx";
                var file = path;

                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = "Exportation",
                    File = new ShareFile(file)
                });
            }
        }
        private async void LoadData()
        {
            Liste = new ObservableCollection<EtudiantModel>(await App.Database.GetListAsync());
            Filiere = new ObservableCollection<FiliereModel>(await App.Database.GetListeFiliere());

            foreach (FiliereModel f in Filiere)
            {
                string filiereCible = f.Filiere;
                ListeCible = new ObservableCollection<EtudiantModel>(await App.Database.GetEnregistrementCible(filiereCible));
            }
        }
        public async Task GetStatusListeBeginScanning()
        {
            try
            {
                LoadData();
                if (ListeCible.Count() > 0 && Filiere.Count() > 0)
                {
                    await Scannning();
                }
                else
                {
                    MessagingCenter.Send(this, "Nodata");
                }
            }
            catch
            {
                MessagingCenter.Send(this, "Nodata");
            }
        }
        private async Task Scannning()
        {
            var scanPage = new ZXingScannerPage
            {
                Title = "Scanner un code QR...",
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
                    if (text.Contains('_'))
                    {
                        string[] elements = text.Split(new char[] { '_' }); // formatage du resultat
                        ScanResult = text.Split(new char[] { '_' });

                        if (ScanResult.Contains(ScanResult[0]) && ScanResult.Contains(ScanResult[1]) && ScanResult.Contains(ScanResult[2]))           //string.IsNullOrEmpty(ScanResult[1])
                        {
                            MessagingCenter.Send(this, "Done");
                        }
                        else
                        {
                            MessagingCenter.Send(this, "None");
                        }
                    }
                    else
                    {
                        MessagingCenter.Send(this, "None");
                    }
                });
            };
        }
        public async void SaveCote(string[] data, string cote)
        {
            // verifier si le matricule fait parti des elements de la liste cible

            Filiere = new ObservableCollection<FiliereModel>(await App.Database.GetListeFiliere());

            foreach (FiliereModel filiere in Filiere)
            {
                string filiereCible = filiere.Filiere;

                List<EtudiantModel> liste = new List<EtudiantModel>();
                liste = await App.Database.CheckEnregistrementFiliereCible(data[1], filiereCible);

                if (liste.Count() > 0)          // s'il existe
                {
                    await App.Database.EnregistrerCoteEtudiant(data[1], data[6], data[7], data[8], cote, data[9]);
                    MessagingCenter.Send(this, "CoteEnregistree");
                }
                else
                {
                    MessagingCenter.Send(this, "EtudiantNonSurListe");
                }
            }
        }
        public void RemplirListeFiliere(string filiere)
        {
            PickFile_Clicked(filiere);
        }
        public void RefreshListCotation(ListView v)
        {
            v.ItemsSource = ListeCible;
        }
        public void ReinitialiserListe()
        {
            if (ListeCible.Count() > 0)
            {
                MessagingCenter.Send(this, "ResultatASupprimer");
            }
            else
            {
                MessagingCenter.Send(this, "NoresultatDelete");
            }
        }
        public async Task SupprimerListesInitialisiees()
        {
            await App.Database.DeleteListe();
            MessagingCenter.Send(this, "DatadeleteSucess");
        }
        public async Task DeleteEtudiant(EtudiantModel etudiant)
        {
            await App.Database.DeleteEtudiantCoteAsync(etudiant);
        }
        public async Task SingletDelete(string matricule)
        {

            Filiere = new ObservableCollection<FiliereModel>(await App.Database.GetListeFiliere());

            foreach (FiliereModel filiere in Filiere)
            {
                string filiereCible = filiere.Filiere;
                await App.Database.DeleteOne(matricule);
                ListeCible = new ObservableCollection<EtudiantModel>(await App.Database.GetEnregistrementCible(filiereCible));
            }
        }
        public async Task DeleteListe()
        {
            Filiere = new ObservableCollection<FiliereModel>(await App.Database.GetListeFiliere());

            foreach (FiliereModel filiere in Filiere)
            {
                string filiereCible = filiere.Filiere;
                await App.Database.ReinitialiserListeCible(filiereCible);

                ListeCible = new ObservableCollection<EtudiantModel>(await App.Database.GetEnregistrementCible(filiereCible));
            }
            MessagingCenter.Send(this, "DatadeleteSucess");
        }
        public async System.Threading.Tasks.Task Export()
        {
            // Granted storage permission
            var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

            if (storageStatus != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Storage });
                storageStatus = results[Permission.Storage];
            }

            try
            {
                string date = DateTime.Now.ToShortDateString();
                date = date.Replace("/", "_");

                var path = DependencyService.Get<IExportFilesToLocation>().GetFolderLocation() + "Export_cotation" + date + ".xlsx";
                FilePath = path;
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet();

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Liste des cotes" };
                    sheets.Append(sheet);

                    workbookPart.Workbook.Save();

                    SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    // Constructing header
                    Row row = new Row();

                    row.Append(
                        ConstructCell("Matricule", CellValues.String),
                        ConstructCell("Nom", CellValues.String),
                        ConstructCell("Post Nom", CellValues.String),
                        ConstructCell("Prenom", CellValues.String),
                        ConstructCell("Epreuve", CellValues.String),
                        ConstructCell("Cote", CellValues.String)
                        );

                    // Insert the header row to the Sheet Data
                    sheetData.AppendChild(row);

                    // Add each product
                    foreach (var d in ListeCible)
                    {
                        row = new Row();
                        row.Append(
                            ConstructCell(d.Matricule.ToString(), CellValues.String),
                            ConstructCell(d.Nom.ToString(), CellValues.String),
                            ConstructCell(d.Postnom, CellValues.String),
                            ConstructCell(d.Prenom, CellValues.String),
                            ConstructCell(d.Epreuve, CellValues.String),
                            ConstructCell(d.Cote, CellValues.String));
                        sheetData.AppendChild(row);
                    }

                    worksheetPart.Worksheet.Save();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("ERROR: " + e.Message);
            }
        }
        public async System.Threading.Tasks.Task ExportDataToExcelAsync()
        {
            //Filiere = new ObservableCollection<FiliereModel>(await App.Database.GetListeFiliere());

            //foreach (FiliereModel f in Filiere)
            //{
            //    string filiereCible = f.Filiere;
            //    ListeCible = new ObservableCollection<EtudiantModel>(await App.Database.GetEnregistrementCible(filiereCible));
            //}

            if (ListeCible.Count() > 0)
            {
                await Export();
                MessagingCenter.Send(this, "DataExportedSuccessfully");
            }
            else
            {
                MessagingCenter.Send(this, "NoDataToExport");
            }
        }
        private Cell ConstructCell(string value, CellValues dataType)
        {
            return new Cell()
            {
                CellValue = new CellValue(value),
                DataType = new EnumValue<CellValues>(dataType)
            };
        }
        private List<Options> Options()
        {
            Options o = new Options
            {
                Titre = "Définir la promotion cible",
                Description = "Choisissez une promotion pour laquel vous voudriez encoder les cotes."
            };

            Options x = new Options
            {
                Titre = "Charger le contenu des promotions",
                Description = "Charger les enregistrements de cette promotion à partir d'une liste d'étudiants."
            };

            Options P = new Options
            {
                Titre = "Effacer toutes les listes disponibles",
                Description = "Supprimer tous les enregistrements des listes des promotions."
            };

            List<Options> options = new List<Options>
            {
                o,
                x,
                P
            };

            return options;
        }
        private void AfficherOptions()
        {
            MesOptions  = new ObservableCollection<Options>(Options());
        }
        private async void GererListe()
        {
            await App.Current.MainPage.Navigation.PushAsync(new ListeConfigurationView()
            {
                Title = "Liste"
            });
        }
        public void afficherFormulairesConfiguration(Options op)
        {
            if (op.Titre == "Définir la promotion cible")
            {
                MessagingCenter.Send(this, "SpecifierPromotion");
            }
            else if (op.Titre == "Charger le contenu des promotions")
            {
                MessagingCenter.Send(this, "SpecifierPromotionAEnregistrer");
            }
            else if (op.Titre == "Effacer toutes les listes disponibles")
            {
                MessagingCenter.Send(this, "DemandereinitialisationAllListe");
            }
        }
        public async void PickFile_Clicked(string filiere)
        {
            await PickAndShowFile(null, filiere);
        }
        private async Task PickAndShowFile(string[] fileTypes, string filiere)
        {
            try
            {
                var pickedFile = await CrossFilePicker.Current.PickFile(fileTypes);

                if (pickedFile != null)
                {
                    string path = pickedFile.FilePath;
                    await LoadingData(path, filiere);
                }
            }
            catch
            {
                MessagingCenter.Send(this, "NonreconnuEnregistrements");
            }
        }
        public async Task LoadingData(string path, string filiere)
        {
            ExcelEngine excelEngine = new ExcelEngine();
            IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Excel2013;

            string resourcePath = path;
            //"App" is the class of Portable project.
            Assembly assembly = typeof(App).GetTypeInfo().Assembly;

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);

            //Opens the workbook 
            IWorkbook workbook = application.Workbooks.Open(fileStream);

            //Access first worksheet from the workbook.
            IWorksheet worksheet = workbook.Worksheets[0];

            // chargement des donnees 

            int i = 2;
            
            while (worksheet.Range["A" + i + ""].Text != null)
            {
                EtudiantModel etudiant = new EtudiantModel
                {
                    Matricule = worksheet.Range["A" + i + ""].Text,
                    Nom = worksheet.Range["B" + i + ""].Text,
                    Postnom = worksheet.Range["C" + i + ""].Text,
                    Prenom = worksheet.Range["D" + i + ""].Text,
                    Filiere = filiere
                };
                // enregistrement dans la BDD
                await App.Database.RemplirListeFiliere(etudiant);

                i += 1;
            }

            workbook.Close();
            excelEngine.Dispose();

            MessagingCenter.Send(this, "ImportationTerminee");
        }

        public async Task ChangerFiliere(FiliereModel filiere)
        {
            await App.Database.ChangerFiliere(filiere);

            Liste = new ObservableCollection<EtudiantModel>(await App.Database.GetListAsync());
            Filiere = new ObservableCollection<FiliereModel>(await App.Database.GetListeFiliere());

            foreach (FiliereModel f in Filiere)
            {
                string filiereCible = f.Filiere;
                ListeCible = new ObservableCollection<EtudiantModel>(await App.Database.GetEnregistrementCible(filiereCible));
            }
        }
        public ICommand ScanCommand { get; private set; }
        public ICommand HistoriqueViewCommand { get; private set; }
        public ICommand ExporterCommad { get; set; }
        public ICommand SearchViewCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand DeleteListeCommad { get; set; }
        public ICommand SendCommand { get; set; }
        public ICommand ListeConfigurationCommand { get; set; }

        private ObservableCollection<EtudiantModel> _liste;
        public ObservableCollection<EtudiantModel> Liste
        {
            get { return _liste; }
            set { SetProperty(ref _liste, value); }
        }
        private ObservableCollection<EtudiantModel> _listeCible;
        public ObservableCollection<EtudiantModel> ListeCible
        {
            get { return _listeCible; }
            set { SetProperty(ref _listeCible, value); }
        }
        private ObservableCollection<FiliereModel> _filiere;
        public ObservableCollection<FiliereModel> Filiere
        {
            get { return _filiere; }
            set { SetProperty(ref _filiere, value); }
        }
        private ObservableCollection<Options> _mesOptions;
        public ObservableCollection<Options> MesOptions
        {
            get { return _mesOptions; }
            set { SetProperty(ref _mesOptions, value); }
        }

        private string[] _scanResult;
        public string[] ScanResult
        {
            get { return _scanResult; }
            set { SetProperty(ref _scanResult, value); }
        }
        private string _filePath;
        private ListView _listView;
        public ListView listView
        {
            get { return _listView; }
            set { SetProperty(ref _listView, value); }
        }

        public string FilePath
        {
            get { return _filePath; }
            set { SetProperty(ref _filePath, value); }
        }
    }   
}
