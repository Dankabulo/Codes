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
using Plugin.Toast;

namespace BarCodeReader.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListeConfigurationView : ContentPage
    {
        private MainMenuViewModel viewModel;
        public ListeConfigurationView()
        {
            InitializeComponent();
            BindingContext = viewModel = new MainMenuViewModel();
            RegisterMesssages();
        }
        public void OnTapping(object sender, ItemTappedEventArgs e)
        {
            Options n = e.Item as Options;
            viewModel.afficherFormulairesConfiguration(n);

        }
        private void RegisterMesssages()
        {
            MessagingCenter.Subscribe<MainMenuViewModel>(this, "SpecifierPromotion",async (m) =>
            {
                if (m != null)
                {
                    string res = await DisplayActionSheet("Spécifiez la promotion","Annuler", null, "Préparatoire", "G1", "G1 A", "G1 B", "G2", "G2 AS", "G2 TLC", "G2 GST", "G2 SI", "G2 DESIGN", "G3", "G3 AS", "G3 TLC", "G3 GST", "G3 SI", "G3 DESIGN");

                    if(res != null || res != "Annuler")
                    {
                        FiliereModel filiere = new FiliereModel
                        {
                            Filiere = res
                        };
                        try
                        {
                            await viewModel.ChangerFiliere(filiere);
                            //await DisplayAlert("Changement de la promotion cible", "" + res + " , a été definit comme promotion cible", "OK");
                            CrossToastPopUp.Current.ShowToastMessage(res+ " a été definit comme promotion cible");
                        }
                        catch
                        {
                            //await DisplayAlert("Echec", "Cette promotion n'a pas été ciblee comme promotion principale", "Annuler");
                            CrossToastPopUp.Current.ShowToastMessage("Cette promotion n'a pas été ciblee comme promotion principale");
                        }
                    }
                }
            });
            MessagingCenter.Subscribe<MainMenuViewModel>(this, "ImportationTerminee", (m) =>
            {
                if (m != null)
                {
                    //DisplayAlert("Importation", "L'importation a été effectuée avec succès.", "OK");
                    CrossToastPopUp.Current.ShowToastMessage("L'importation a été effectuée avec succès.");
                }
            });
            MessagingCenter.Subscribe<MainMenuViewModel>(this, "NonreconnuEnregistrements", (m) =>
            {
                if (m != null)
                {
                    DisplayAlert("Erreur", "L'importation n'a pas ete effectuée, vérifiez vos informations d'importation.", "OK");
                }
            });
            MessagingCenter.Subscribe<MainMenuViewModel>(this, "SpecifierPromotionAEnregistrer", async (m) =>
            {
                if (m != null)
                {
                    string res = await DisplayActionSheet("Choisissez une promotion ", "Annuler", null, "Préparatoire", "G1", "G1 A", "G1 B", "G2", "G2 AS", "G2 TLC", "G2 GST", "G2 SI", "G2 DESIGN", "G3", "G3 AS", "G3 TLC", "G3 GST", "G3 SI", "G3 DESIGN");

                    if (string.IsNullOrEmpty(res))
                    {
                        await DisplayAlert("Echec", "Vous devez préciser la promotion de continuer.", "Annuler");
                    }
                    else
                    {
                        FiliereModel filiere = new FiliereModel
                        {
                            Filiere = res
                        };
                        try
                        {
                            await viewModel.ChangerFiliere(filiere);
                            // Chargement de l'explorateur des fichier
                            viewModel.RemplirListeFiliere(filiere.Filiere);

                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("Echec", "" + ex.ToString() + "", "Annuler");
                        }
                    }
                }
            });
            MessagingCenter.Subscribe<MainMenuViewModel>(this, "DemandereinitialisationAllListe", async (m) =>
            {
                if (m != null)
                {
                    bool x = await DisplayAlert("Suppression", "Voulez-vous supprimer toutes les listes initialisées ? Toutes les informations enregistrées seront perdues lors de ce processus.", "Supprimer", "Annuler");
                    if (x == true)
                    {
                        await viewModel.SupprimerListesInitialisiees();
                    }
                }
            });
            MessagingCenter.Subscribe<MainMenuViewModel>(this, "DatadeleteSucess", (m) =>
            {
                if (m != null)
                {
                    //DisplayAlert("Suppression", "Données supprimées avec succès", "OK");
                    CrossToastPopUp.Current.ShowToastMessage("Données supprimées avec succès.");
                }
            });
        }
        
    }
}