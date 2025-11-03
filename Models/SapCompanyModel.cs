using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapGateway.Models
{
    public class SapCompanyModel
    {
        public int id { get; set; }
        public string CompanyName { get; set; }
        public string CompanyDB { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
