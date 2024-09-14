using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.DomainLayer.Models
{
    public class PayPalTokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }

    public class PayPalPaymentResponse
    {
        public List<PayPalLink> links { get; set; }
    }

    public class PayPalLink
    {
        public string href { get; set; }
        public string rel { get; set; }
        public string method { get; set; }
    }
}