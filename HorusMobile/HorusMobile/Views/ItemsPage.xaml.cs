using HorusMobile.Models;
using HorusMobile.ViewModels;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace HorusMobile.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    //[Preserve(AllMembers = true)]
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel viewModel;

        public ItemsPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new ItemsViewModel();
            /*
            Resources["ListNotifTextStyle"] = Resources["labelTextBlack"];
            Resources["ListNotifDetailTextStyle"] = Resources["labelDescriptionBlack"];
            */
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as Item;
            if (item == null)
                return;

            //Debug.WriteLine("ITEM CUERPO: " + item.id_cuerpo);
            await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item)));

            // Deselecciona el item manualmente.
            ItemsListView.SelectedItem = null;

            // Marcando como leída la notificación
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", App.Current.Properties["_json_token"].ToString());
            var buffer = System.Text.Encoding.UTF8.GetBytes("");
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = client.PutAsync("http://66.97.39.24:8044/mensajes/msjCuerpo/changeEstado/"+ item.id_cuerpo + "/true", byteContent).Result;
        }

        async void AddItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new NewItemPage()));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.LoadItemsCommand.Execute(null);
        }
    }
}