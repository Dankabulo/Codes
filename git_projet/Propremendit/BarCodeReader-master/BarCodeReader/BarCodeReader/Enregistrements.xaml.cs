using System;
using BarCodeReader.Interfaces;
using BarCodeReader.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarCodeReader.Data;
using BarCodeReader.Models;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Cell = DocumentFormat.OpenXml.Spreadsheet.Cell;

namespace BarCodeReader
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Enregistrements : ContentPage
    {
        public Enregistrements()
        {
            InitializeComponent();
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            listView.ItemsSource = await App.Database.GetListAsync();
        }

        public async void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            bool action = await DisplayAlert("Suppresion", "Voulez-vous supprimer cet enregistrement ?", "Oui", "Non");
            if (action == true)
            {
                var data = e.SelectedItem as EtudiantModel;
                await App.Database.DeleteEtudiantCoteAsync(data);
                Console.WriteLine(" Tache supprimee");
                await Navigation.PopAsync();
            }
            else
            {
                Console.WriteLine("suppression annullee");
                await Navigation.PopAsync();
            }
        }
        public void Export(object sender, EventArgs e)
        {
            ExportDataToExcelAsync();
        }

        public async Task ExportDataToExcelAsync()
        {
            List<EtudiantModel> Developers = await App.Database.GetListAsync();

            // Granted storage permission
            var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

            if (storageStatus != PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Storage });
                storageStatus = results[Permission.Storage];
            }

            if (Developers.Count() > 0)
            {
                try
                {
                    string date = DateTime.Now.ToShortDateString();
                    date = date.Replace("/", "_");

                    var path = DependencyService.Get<IExportFilesToLocation>().GetFolderLocation() + "Cotation" + date + ".xlsx";
                    FilePath = path;
                    using (SpreadsheetDocument document = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook))
                    {
                        WorkbookPart workbookPart = document.AddWorkbookPart();
                        workbookPart.Workbook = new Workbook();

                        WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                        worksheetPart.Worksheet = new Worksheet();

                        Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                        Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Xamarin Forms developers list" };
                        sheets.Append(sheet);

                        workbookPart.Workbook.Save();

                        SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                        // Constructing header
                        Row row = new Row();

                        row.Append(
                            ConstructCell("Identidiant", CellValues.String),
                            ConstructCell("Nom", CellValues.String),
                            ConstructCell("Postnom", CellValues.String),
                            ConstructCell("Prenom", CellValues.String),
                            ConstructCell("Filiere", CellValues.String),
                            ConstructCell("Cours", CellValues.String),
                            ConstructCell("Epreuve", CellValues.String),
                            ConstructCell("Cote_max", CellValues.String),
                            ConstructCell("Date epreuve", CellValues.String),
                            ConstructCell("Cote", CellValues.String)
                            );

                        // Insert the header row to the Sheet Data
                        sheetData.AppendChild(row);

                        // Add each product
                        foreach (var d in Developers)
                        {
                            row = new Row();
                            row.Append(
                                ConstructCell(d.Id.ToString(), CellValues.String),
                                ConstructCell(d.Nom.ToString(), CellValues.String),
                                ConstructCell(d.Postnom.ToString(), CellValues.String),
                                ConstructCell(d.Prenom.ToString(), CellValues.String),
                                ConstructCell(d.Filiere.ToString(), CellValues.String),
                                ConstructCell(d.Cours.ToString(), CellValues.String),
                                ConstructCell(d.Epreuve.ToString(), CellValues.String),
                                ConstructCell(d.Cote_max.ToString(), CellValues.String),
                                ConstructCell(d.Date.ToString(), CellValues.String),
                                ConstructCell(d.Cote.ToString(), CellValues.String));
                            sheetData.AppendChild(row);
                        }

                        worksheetPart.Worksheet.Save();
                        MessagingCenter.Send(this, "DataExportedSuccessfully");
                    }

                }
                catch (Exception e)
                {
                    Debug.WriteLine("ERROR: " + e.Message);
                }
            }
            else
            {
                MessagingCenter.Send(this, "NoDataToExport");
            }
        }


        /* To create cell in Excel */
        private Cell ConstructCell(string value, CellValues dataType)
        {
            return new Cell()
            {
                CellValue = new CellValue(value),
                DataType = new EnumValue<CellValues>(dataType)
            };
        }
        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set { SetProperty(ref _filePath, value); }
        }
    }
}