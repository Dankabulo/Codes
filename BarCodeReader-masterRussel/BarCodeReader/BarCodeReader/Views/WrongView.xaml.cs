using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarCodeReader.ViewModels;
using BarCodeReader.Models;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BarCodeReader.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WrongView : ContentPage
    {
        public WrongView(BilletModel billet)
        {
            InitializeComponent();
            label.Text = "Veuillez vous rendre au  : " + billet.place+".";
        }
    }
}