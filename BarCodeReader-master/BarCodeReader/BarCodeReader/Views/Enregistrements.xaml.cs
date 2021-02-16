using System;
using BarCodeReader.ViewModels;
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
using Plugin.Toast;

namespace BarCodeReader
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Enregistrements : ContentPage
    {
        private MainMenuViewModel viewModel;

        
        public Enregistrements()
        {
            InitializeComponent();
            BindingContext = viewModel = new MainMenuViewModel();
            RegisterMesssages();
            ToolbarItems.Add(new ToolbarItem
            {
                Text = "Exporter",
                Order = ToolbarItemOrder.Secondary,
                Command = viewModel.ExporterCommad
            }) ;
            ToolbarItems.Add(new ToolbarItem
            {
                Text = "Envoyer vers",
                Order = ToolbarItemOrder.Secondary,
                Command = viewModel.SendCommand
            });
            ToolbarItems.Add(new ToolbarItem
            {
                Text = "Rechecher",
                Order = ToolbarItemOrder.Secondary,
                Command = viewModel.SearchViewCommand
            });
            ToolbarItems.Add(new ToolbarItem
            {
                Text = "Réinitialiser",
                Order = ToolbarItemOrder.Secondary,
                Command = viewModel.DeleteListeCommad
            });
        }
        private void RegisterMesssages()
        {
            MessagingCenter.Subscribe<MainMenuViewModel>(this, "DataExportedSuccessfully", (m) =>
            {
                if (m != null)
                {
                    DisplayAlert("Info", "Données exportées avec succès. Son emplacement est :" + m.FilePath, "OK");
                }
            });

            MessagingCenter.Subscribe<MainMenuViewModel>(this, "ResultatASupprimer", (m) =>
            {
                if (m != null)
                {
                    this.OnDeleteListe();
                }
            });

            MessagingCenter.Subscribe<MainMenuViewModel>(this, "NoresultatDelete", (m) =>
            {
                if (m != null)
                {
                    //DisplayAlert("Info", "La liste des enregistremnts est vide.", "OK");
                    CrossToastPopUp.Current.ShowToastMessage("Aucune donnée à supprimer.");
                }
            });

            MessagingCenter.Subscribe<MainMenuViewModel>(this, "NoScanResultToShare", (m) =>
            {
                if (m != null)
                {
                    //DisplayAlert("Attention", "Il n'existe pas des données à envoyer", "OK");
                    CrossToastPopUp.Current.ShowToastMessage("Aucune donée à envoyer.");
                }
            });

            MessagingCenter.Subscribe<MainMenuViewModel>(this, "NoDataToExport", (m) =>
            {
                if (m != null)
                {
                    //DisplayAlert("Attention !", "Aucune donée n'a été exportée.", "OK");
                    CrossToastPopUp.Current.ShowToastMessage("Aucune donée à exporter.");
                }
            });
            MessagingCenter.Subscribe<MainMenuViewModel>(this, "DatadeleteSucess", (m) =>
            {
                if (m != null)
                {
                    //DisplayAlert("Info", "Données supprimées avec succès", "OK");
                    CrossToastPopUp.Current.ShowToastMessage("Données supprimées avec succès.");
                }
            });
        }
        public async void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            EtudiantModel d = e.SelectedItem as EtudiantModel;
            bool c = await DisplayAlert(d.Nom+" "+d.Postnom + " " +d.Prenom,d.Epreuve + " : " +d.Cote + "/" +d.Cote_max, "retirer de la liste","OK");

            if (c == true)
            {
                bool x = await DisplayAlert("Suppression !", "Voulez-vous supprimer cet enregistrement de la liste?", "Supprimer", "Annuler");
                if( x == true)
                {
                    EtudiantModel t = e.SelectedItem as EtudiantModel;
                    await viewModel.DeleteEtudiant(t);
                }
            }
        }
        public async void OnDeleteListe()
        {
            bool c = await DisplayAlert("Réinitialisation !", "Voulez-vous réinitialiser la liste ? Toutes les informations enregistrées seront perdues", "Réinitialiser", "Annuler");

            if (c == true)
            {
                await viewModel.DeleteListe();
            }
            //String action = await DisplayActionSheet("Options",
            //        "Cancel", null, "Here", "There", "Everywhere");
        }
        public void OnRefreshing(object sender,EventArgs e)
        {
            this.listView.IsRefreshing = true;
            viewModel.RefreshListCotation(this.listView);
            this.listView.IsRefreshing = false;
        }
        public async void OnDelete(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            bool c = await DisplayAlert("Suppression", mi.CommandParameter.ToString() + " Voulez-vous supprimer cet enregistrement ?", "OUI", "NON") ;
            string mat = mi.CommandParameter.ToString();

            if (c == true)
            {
                // suppression de l'enregistrement selectionne
                await viewModel.SingletDelete(mi.CommandParameter.ToString());
            }
        }
    }
}