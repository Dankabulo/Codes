using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;
using BarCodeReader;
using System.IO;
using System;
using BarCodeReader.Models;
using BarCodeReader.Data;

namespace BarCodeReader
{
    public partial class Accueil : ContentPage
    {
        public Accueil()
        {
            InitializeComponent();
        }

        public async void OnScanning(object sender, EventArgs e)
        {
            string scanResult;
            var scanPage = new ZXingScannerPage
            {
                Title = "Scanning...",
                BackgroundColor = Color.FromHex("#212223")
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
                    //ScanResult = result?.Text; 
                    string text = result?.Text;
                    string[] elements = text.Split(new char[] { '_' });
                    scanResult = elements[2];

                    string cote = await DisplayPromptAsync(elements[7] + " /" + elements[8], elements[2] + " " + elements[3] + " " + elements[4], initialValue: "1", maxLength: 2, keyboard: Keyboard.Numeric);
                    if (cote == null)
                    {
                        await DisplayAlert("Erreur", "Aucune cote n a ete saisie pour cet etudiant", "Annuler");
                    }
                    else
                    {
                        OnSave(elements, cote);
                    }

                });
            };
        }

        public async void OnSave(string[] data, string point )
        {
            // enregistrement des infos
            var cote = new EtudiantModel();
            cote.Nom = data[2];
            cote.Postnom = data[3];
            cote.Prenom = data[4];
            cote.Epreuve = data[5];
            cote.Filiere = data[6];
            cote.Cours = data[7];
            cote.Cote_max = data[8];
            cote.Date = data[9];
            cote.Cote = point;
            await App.Database.SaveEtudiantCoteAsync(cote);
        }
        public async void OnExport(object sender,EventArgs e)
        {
            await Navigation.PushAsync(new Enregistrements
            {
                BindingContext = new EtudiantModel()
            });
        }
    }
}