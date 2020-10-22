using HorusMobile.Models;
using HorusMobile.ViewModels;
using System;
using System.ComponentModel;
using System.Diagnostics;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace HorusMobile.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ItemDetailPage : ContentPage
    {
        ItemDetailViewModel viewModel;

        public ItemDetailPage(ItemDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;
        }

        public ItemDetailPage()
        {
            InitializeComponent();

            var item = new Item
            {
                Text = "Título de notificación 1",
                Description = "Ejemplo de cuerpo de notificación.",
                FechaEnviado = "2020-09-15 10:18:00",
                TituloFormateado = "2020-09-15 10:18:00 Título de notificación 1",
                Link = "www.google.com"
            };

            viewModel = new ItemDetailViewModel(item);
            BindingContext = viewModel;
        }

        private async void VerMasInfoHorusTapped(object sender, EventArgs e)
        {
            string user = App.Current.Properties["_user_login"].ToString();
            string pass = App.Current.Properties["_user_pass"].ToString();
            string link = labelLink.Text;
            Debug.WriteLine("Linkurl: " + link);
            await Browser.OpenAsync(link, BrowserLaunchMode.SystemPreferred);
        }
    }
}