using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.DomainLayer.Models
{
    public class PayPalClient
    {
        public string Mode { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public string BaseUrl => Mode == "Live"
         ? "https://api-m.paypal.com" : "https://api-m.sandbox.paypal.com";

        public PayPalClient(string clientid, string clientsecret, string mode)
        {
            Mode = mode;
            ClientId = clientid;
            ClientSecret = clientsecret;
        }
    }
}