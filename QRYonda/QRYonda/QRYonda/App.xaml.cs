using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace QRYonda
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var nav = new NavigationPage(new Views.YondaList());
            nav.BarBackgroundColor = Color.FromHex("#920000");
            nav.BarTextColor = Color.White;
            MainPage = nav;
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
