using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Internals;

namespace HorusMobile.Models
{
    [Preserve(AllMembers = true)]
    public class Notificaciones
    {
        public string id { get; set; }
        public string cabecera { get; set; }
        public string userDestino { get; set; }
        public string asunto { get; set; }
        public bool estado { get; set; }
        public string mensaje { get; set; }
        public string link { get; set; }
        public string message { get; set; }
        public string error { get; set; }
        public string fechaUltimoEnvio { get; set; }
    }
}
