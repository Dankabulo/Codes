using BarCodeReader;
using BarCodeReader.ViewModels.Base;
using BarCodeReader.Views;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace BarCodeReader
{
    public partial class App : Application
    {
        static EtudiantData database;

        public static EtudiantData Database;
        {
            get
            {
                if (database == null)
                {
                    database = new EtudiantData(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EtudiantsCotes.db3"));
                }
                return database; 
            }
        }
        public App()
        {
            InitializeComponent();

            InitApp();

            if (Device.RuntimePlatform == Device.UWP)
            {
                InitNavigation();
            }
            MainPage = new CustomNavigationView(new MainMenuView());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
