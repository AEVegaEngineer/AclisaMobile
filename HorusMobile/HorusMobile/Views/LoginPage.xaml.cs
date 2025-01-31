﻿using HorusMobile.Models;
using HorusMobile.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
/*
using System.ComponentModel;
using HorusMobile.Views;
using HorusMobile.ViewModels;
*/
using System.Net.Http;
using System.Net.Http.Headers;
/*
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
*/

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HorusMobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        ILoginManager iml = null;
        public LoginPage(ILoginManager ilm)
        {
            InitializeComponent();
            iml = ilm;
        }
        private bool _pbIndicator;
        public bool PBIndicator
        {
            get { return _pbIndicator; }
            set
            {
                _pbIndicator = value;
                OnPropertyChanged();
            }
        }
        /*
        void UpdateUiState()
        {
            Debug.WriteLine("\n\nUPDATEUISTATE\n\n");
            lblStatus.Text = statusSesion ? "Iniciando Sesión..." : "";
            IndicadorActividad.IsRunning = statusSesion;
            IndicadorActividad.IsVisible = statusSesion;
            IndicadorActividad.IsEnabled = statusSesion;
        }
        */

        protected override void OnAppearing()
        {
            base.OnAppearing();
            PBIndicator = true;
        }
        async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            IndicadorActividad.IsRunning = true;
            IndicadorActividad.IsEnabled = true;
            IndicadorActividad.IsVisible = true;
            btnLogin.IsEnabled = false;
            btnLogin.BackgroundColor = Color.DarkGray;

            //Muestra el activity indicator para el login
            PBIndicator = !PBIndicator;
            IsBusy = true;

            var user = username.Text;
            var pass = password.Text;

            if (string.IsNullOrWhiteSpace(user) && string.IsNullOrWhiteSpace(pass))
            {
                await DisplayAlert("Login", "Debe escribir un usuario y una contraseña", "OK");
                Debug.WriteLine("Debe escribir un usuario y una contraseña");
                return;
            }



            //RestClient client = new RestClient();
            HttpClient client = new HttpClient();
            //var getUserLogin = await client.Get<getUserLogin>("http://192.168.50.98/intermedio/api/usuarios/login.php");
            Users usuario = new Users();
            //Usuario usuario = new Usuario(user,pass, App.Current.getCurrentDeviceId());
            usuario.password = pass;
            usuario.username = user;
            //usuario.deviceId = App.Current.getCurrentDeviceId();
            Application.Current.Properties["_user_login"] = user;
            Application.Current.Properties["_user_pass"] = pass;
            Application.Current.Properties["_device_id"] = App.Current.getCurrentDeviceId();

            //serializo el objeto a json
            var myContent = JsonConvert.SerializeObject(usuario);
            //construyo un objeto contenido para mandar la data, uso un objeto ByteArrayContent
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);

            //establezco el tipo de contenido a JSON para que la api la reconozca
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //http://192.168.50.98/intermedio
            
            var result = (HttpResponseMessage)null;
            try
            {
                //envío el request por POST
                Debug.WriteLine("*************** antes de postasync ****************");
                result = await client.PostAsync("http://66.97.39.24:8044/login", byteContent);
                Debug.WriteLine("*************** postasync ****************");
            }
            catch (Exception exception)
            {
                Debug.WriteLine("EXCEPCIÓN ATRAPADA:");
                Debug.WriteLine(exception);
            }
            if (result != null)
            {
                var contents = await result.Content.ReadAsStringAsync();
                string token = result.Headers.Contains("Authorization") ? result.Headers.GetValues("Authorization").First() : null;
                //var token = result.Headers.GetValues("Authorization").FirstOrDefault();
                //reviso si se ha hecho una conexión correcta con el servidor
                validoRespuestaServer(contents, token);

                // ha retornado un json, no necesariamente correcto                
                token tk = JsonConvert.DeserializeObject<token>(contents);
                
                if (tk.message == "Unauthorized")
                {
                    await DisplayAlert("Login", "Usuario o pass incorrecto", "OK");
                }
                else
                {
                    try
                    {
                        //finalmente todo esta OK, voy a registrar el deviceid
                        Debug.WriteLine("Se registra el token:");
                        Debug.WriteLine(token);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        string deviceid = "{\"deviceId\": \"" + App.Current.getCurrentDeviceId() + "\", \"deviceName\" : \" \"}";
                        Debug.WriteLine("Se registra el deviceid:");
                        Debug.WriteLine(deviceid);
                        
                        var bufferdeviceid = System.Text.Encoding.UTF8.GetBytes(deviceid);
                        var byteContentdeviceid = new ByteArrayContent(bufferdeviceid);
                        //establezco el tipo de contenido a JSON para que la api la reconozca
                        byteContentdeviceid.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        result = await client.PostAsync("http://66.97.39.24:8044/mensajes/device/insert/mine", byteContentdeviceid);
                        var autenticado = false;
                        if (result != null)
                        {
                            var contenido = await result.Content.ReadAsStringAsync();
                            Debug.WriteLine("Se obtiene la respuesta del registro de dispositivo:");
                            Debug.WriteLine(contenido);
                            if (contenido.Contains("\"resultado\":true"))
                                autenticado = true;
                            if(contenido.Contains("La inserción produjo un error"))
                                ErrorInterno(159);
                        }
                        else
                        {
                            ErrorEnConexion(157);
                        }

                        //seteo el id del usuario en la app, para postearla al appcenter
                        //var usr_id = tk.nombreUsuario;
                        //seteo el token de la  app para persistirlo
                        if(autenticado)
                        {
                            Application.Current.Properties["_json_token"] = token;
                            Debug.WriteLine("_json_token = " + token);
                            //Muestro la página principal
                            iml.ShowMainPage();

                            //await Navigation.PushModalAsync(new MainPage());
                            //await Navigation.PopAsync();
                        }


                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        await DisplayAlert("Error en Login", ex.ToString(), "OK");
                    }
                }
                
            }
            else
            {
                ErrorEnConexion(183);
            }

            IndicadorActividad.IsRunning = false;
            IndicadorActividad.IsEnabled = false;
            IndicadorActividad.IsVisible = false;
            btnLogin.IsEnabled = true;
            btnLogin.BackgroundColor = Color.Cyan;
        }
        public static bool IsValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private async void ErrorEnConexion(int line)
        {
            Debug.WriteLine("\n\nRESULT NULL ERROR IN LINE " + line + "\n\n");
            await DisplayAlert("Error de red", "No se ha logrado hacer conexión con los servidores de Aclisa, revise su conexión o hable con el administrador de la red.", "OK");
        }
        private async void ErrorInterno(int line)
        {
            Debug.WriteLine("\n\nINTERNAL SERVER ERROR IN LINE " + line + "\n\n");
            await DisplayAlert("Error", "Ha ocurrido un error, por favor intente de nuevo, si el problema continúa contacte a soporte.", "OK");
        }
        private async void validoRespuestaServer(string contents, string token)
        {
            if (!IsValidJson(contents))
            {
                ErrorEnConexion(222);
                IndicadorActividad.IsRunning = false;
                IndicadorActividad.IsEnabled = false;
                IndicadorActividad.IsVisible = false;
                btnLogin.IsEnabled = true;
                btnLogin.BackgroundColor = Color.Cyan;
            }
            if (token == null)
            {
                await DisplayAlert("Error", "No autorizado", "OK");
                IndicadorActividad.IsRunning = false;
                IndicadorActividad.IsEnabled = false;
                IndicadorActividad.IsVisible = false;
                btnLogin.IsEnabled = true;
                btnLogin.BackgroundColor = Color.Cyan;
            }
            return;
        }
    }
}