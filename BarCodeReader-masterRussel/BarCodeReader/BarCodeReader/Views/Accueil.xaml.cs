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
                Text = "Configurations",
                Order = ToolbarItemOrder.Secondary,
                Command = viewModel.ConfigurationCommand
            });
        }

        public void RegisterMessages()
        {
            MessagingCenter.Subscribe<MainMenuViewModel>(this, "Authentification", async (m) =>
            {
                if (m != null)
                {
                    string point = await DisplayPromptAsync("Authentification", "Veuillez entrer le mot de passe administrateur",
                                         maxLength: 6, keyboard: Keyboard.Numeric );

                    if (string.IsNullOrEmpty(point))
                    {
                        await DisplayAlert("Erreur !", "Echec lors de l'authentification.", "Annuler");
                    }
                    else
                    {
                        await viewModel.PageConfiguration();
                    }
                }
            });
        }
        //public void OnParametreClicked(object sender, EventArgs e)
        //{ 

        //}
    }
}