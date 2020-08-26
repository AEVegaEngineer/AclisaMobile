using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;

using HorusMobile.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace HorusMobile.Views
{
    [DesignTimeVisible(false)]
    public partial class MainPage : MasterDetailPage
    {
        Dictionary<int, NavigationPage> MenuPages = new Dictionary<int, NavigationPage>();
        public MainPage()
        {

            InitializeComponent(); 

            MasterBehavior = MasterBehavior.Popover;

            //MenuPages.Add((int)MenuItemType.Browse, (NavigationPage)Detail);
            
            //var usr_id = Application.Current.Properties["_user_id"];
            
        }

        //public async Task NavigateFromMenu(int id)
        public async void NavigateFromMenu(int id)
        {
            if (!MenuPages.ContainsKey(id))
            {
                switch (id)
                {
                    /*
                    case (int)MenuItemType.Browse:
                        MenuPages.Add(id, new NavigationPage(new ItemsPage()));
                        break;
                    case (int)MenuItemType.About:
                        MenuPages.Add(id, new NavigationPage(new AboutPage()));
                        break;
                        */
                    case (int)MenuItemType.Logout:
                        HttpClient client = new HttpClient();
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", App.Current.Properties["_json_token"].ToString());
                        
                        //string deviceid = "{\"deviceId\": \"" +  + "\", \"deviceName\" : \" \"}";
                        var result = await client.DeleteAsync("http://66.97.39.24:8044/mensajes/device/delete/mine/"+ App.Current.getCurrentDeviceId().ToString());
                        App.Current.Logout();
                        break;
                }
            }
            /*
            var newPage = MenuPages[id];

            if (newPage != null && Detail != newPage)
            {
                Detail = newPage;

                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(100);

                IsPresented = false;
            }
            */
        }
    }
}