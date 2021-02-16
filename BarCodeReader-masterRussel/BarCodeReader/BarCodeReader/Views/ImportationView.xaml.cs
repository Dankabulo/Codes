using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarCodeReader.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BarCodeReader.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImportationView : ContentPage
    {
        private MainMenuViewModel viewModel;
        public ImportationView()
        {
            InitializeComponent();
            BindingContext = viewModel = new MainMenuViewModel();
            RegisterMessages();
        }
        public void RegisterMessages()
        {
            MessagingCenter.Subscribe<MainMenuViewModel>(this, "ImportationTerminee", async (m) =>
            {
                if (m != null)
                {
                    await DisplayAlert("Importation", "L'operation d'importation a ete effectuee avec succes.", "OK");
                }
            });
            MessagingCenter.Subscribe<MainMenuViewModel>(this, "NonreconnuEnregistrements", async (m) =>
            {
                if (m != null)
                {
                    await DisplayAlert("Erreur importation", "Echec lors de l'importation.", "Annuler");
                }
            });
            MessagingCenter.Subscribe<MainMenuViewModel>(this, "AskReinitialisation", async (m) =>
            {
                if (m != null)
                {
                    bool c = await DisplayAlert("Reinitilisation", "Voulez-vous reinitilaiser les enregistrements disponibles ?", "OUI", "NON");

                    if (c == true)
                    {
                        // on reinitialise la liste
                        await viewModel.Reinitialisation();
                    }
                }
            });
            MessagingCenter.Subscribe<MainMenuViewModel>(this, "Finreinitialisation", async (m) =>
            {
                if (m != null)
                {
                    await DisplayAlert("Reinitialisation", "Enregistrments supprimés.", "OK");
                }
            });
            MessagingCenter.Subscribe<MainMenuViewModel>(this, "FormulaireImportation", async (m) =>
            {
                if (m != null)
                {
                    string texte = await DisplayPromptAsync("Importation", "Coller le texte des enregistrements dans ce champ.");

                    if (string.IsNullOrEmpty(texte))
                    {
                        await DisplayAlert("Erreur !", "Echec lors de l'importation.", "Annuler");
                    }
                    else
                    {
                        await viewModel.OnImportation(texte);
                    }
                }
            });
            MessagingCenter.Subscribe<MainMenuViewModel>(this, "FormulaireChangerPorte", async (m) =>
            {
                if (m != null)
                {
                    string res = await DisplayActionSheet("Terminal", "Annuler", null, "Tribune d'honneur", "VIP", "Laterale A", "Laterale B", "Laterale C", "Laterale D");

                    await viewModel.Terminal(res);
                }
            });
            MessagingCenter.Subscribe<MainMenuViewModel>(this, "PorteChangee", async (m) =>
            {
                if (m != null)
                {
                    await DisplayAlert("Changement du terminal", "L'etat de terminal a été modifie avec succès", "OK");
                }
            });
        }
    }
}