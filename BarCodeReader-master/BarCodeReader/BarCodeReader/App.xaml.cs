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
        static EtudiantData database;

        public static EtudiantData Database
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
            MainPage = new CustomNavigationView(new Accueil())
            {
                BarBackgroundColor = Color.FromHex("#19274d")
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
