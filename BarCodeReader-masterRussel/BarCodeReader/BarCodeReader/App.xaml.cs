using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
using BarCodeReader.Data;
using BarCodeReader.Services.Navigation;
using BarCodeReader.ViewModels.Base;
using BarCodeReader.Views;
using System;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace BarCodeReader
{
    public partial class App : Application
    {
        //static EtudiantData database;
        static BilletData database;
        static PorteData databsePorte;
        public static BilletData Database
        {
            get
            {
                if (database == null)
                {
                    database = new BilletData(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Billets.db3"));
                }
                return database; 
            }
        }
        public static PorteData DatabasePorte
        {
            get 
            {
                if (databsePorte == null)
                {
                    databsePorte = new PorteData(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Portes.db3"));
                }
                return databsePorte;
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
            MainPage = new CustomNavigationView(new Accueil())
            {
                BarBackgroundColor = Color.FromHex("#449e8f")
            };
        }
        private Task InitNavigation()
        {
            var navigationService = ViewModelLocator.Resolve<INavigationService>();
            return navigationService.InitializeAsync();
        }
        private void InitApp()
        {
            ViewModelLocator.RegisterDependencies(false);
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
