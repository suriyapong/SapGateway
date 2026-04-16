using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapGateway.Models
{
    public class SAPPurchaseRequestByDocEntryModel
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string RequesterEmail { get; set; }
    }
}