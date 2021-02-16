using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarCodeReader.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using BarCodeReader.Models;

namespace BarCodeReader.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchView : ContentPage
    {
        private MainMenuViewModel viewModel;
        public SearchView()
        {
            InitializeComponent();
            BindingContext = viewModel = new MainMenuViewModel();
        }
        public void Onsearch(object sender, TextChangedEventArgs e)
        {
            listView.ItemsSource = viewModel.ListeCible.Where(f => f.Nom.ToLowerInvariant().Contains(e.NewTextValue.ToString()));
        }
        public async void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            EtudiantModel d = e.SelectedItem as EtudiantModel;
            bool c = await DisplayAlert(d.Nom + " " + d.Postnom + " " + d.Prenom, d.Epreuve + " : " + d.Cote + "/" + d.Cote_max, "retirer de la liste", "OK");

            if (c == true)
            {
                bool x = await DisplayAlert("Suppression !", "Voulez-vous supprimer cet enregistrement de la liste?", "Supprimer", "Annuler");
                if (x == true)
                {
                    EtudiantModel t = e.SelectedItem as EtudiantModel;
                    await viewModel.DeleteEtudiant(t);
                    await App.Current.MainPage.Navigation.PopToRootAsync();
                    await viewModel.AfterDeleteOne();
                }
            }
        }
        public async void OnDelete(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            bool c = await DisplayAlert("Suppression", mi.CommandParameter.ToString() + " Voulez-vous supprimer cet enregistrement ?", "OUI", "NON");
            string mat = mi.CommandParameter.ToString();

            if (c == true)
            {
                // suppression de l'enregistrement selectionne
                await viewModel.SingletDelete(mi.CommandParameter.ToString());
            }
        }
    }
}