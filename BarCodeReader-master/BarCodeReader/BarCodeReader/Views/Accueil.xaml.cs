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
using BarCodeReader.ViewModels;
using Android.Widget;
using Plugin.Toast;

namespace BarCodeReader
{
    public partial class Accueil : ContentPage
    {
        private MainMenuViewModel viewModel;

        public Accueil()
        {
            InitializeComponent();
            BindingContext = viewModel = new MainMenuViewModel();
            RegisterMessages();

            ToolbarItems.Add(new ToolbarItem
            {
                Text = "Gérer les listes",
                Order = ToolbarItemOrder.Secondary,
                Command = viewModel.ListeConfigurationCommand
            });
        }

        public void RegisterMessages()
        {
            MessagingCenter.Subscribe<MainMenuViewModel>(this, "Done", async (m) =>
            {
                if (m != null)
                {
                    string point = await DisplayPromptAsync("Encodage ", "Saisir une cote.", 
                                         maxLength: 4, keyboard: Keyboard.Numeric);

                    //string point = await DisplayPromptAsync("Encodage !", "Saisir une cote pour l'etudiant :" + m.ScanResult[2] + " .",
                    //                     maxLength: 4, keyboard: Keyboard.Numeric);

                    if (string.IsNullOrEmpty(point))
                    {
                        await DisplayAlert("Erreur !", "Vous devez saisir une cote pour enregistrer.", "OK");
                    }
                    else
                    {
                        viewModel.SaveCote(m.ScanResult, point);
                        //await DisplayAlert("Sucess !","Vos informations ont ete enregistrees.","OK");
                    }
                }
            });
            MessagingCenter.Subscribe<MainMenuViewModel>(this, "CoteEnregistree", (m) =>
            {
                if (m != null)
                {
                    //await DisplayAlert("Sucess !", "Vos informations ont ete enregistrees.", "OK");
                    CrossToastPopUp.Current.ShowToastMessage("Cote enrgistrée !");
                }
            });
            MessagingCenter.Subscribe<MainMenuViewModel>(this, "None", (m) =>
            {
                if (m != null)
                {
                    //await DisplayAlert("Erreur", "Ce code QR n'est pas reconnu veuillez scanner un autre", "OK");
                    CrossToastPopUp.Current.ShowToastMessage("Ce code QR n'est pas reconnu veuillez scanner un autre.");
                }
            }); 
            MessagingCenter.Subscribe<MainMenuViewModel>(this, "EtudiantNonSurListe", async(m) =>
            {
                if (m != null)
                {
                    await DisplayAlert("Erreur", "Ce matricule ne fait pas parti de la promotion cible, veuillez changer de promotion ou scanner un autre code", "OK");
                }
            });
            MessagingCenter.Subscribe<MainMenuViewModel>(this, "Nodata", async(m) =>
            {
                if (m != null)
                {
                    await DisplayAlert("Alerte !", "Veuillez préciser une liste ou remplir la liste cible.", "OK");
                }
            });
        }
        public void OnParametreClicked(object sender, EventArgs e)
        { 
            
        }
    }
}