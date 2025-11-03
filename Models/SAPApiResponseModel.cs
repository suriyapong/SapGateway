using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapGateway.Models
{
    public class SAPApiResponseWrapper
    {
        public List<SAPApiResponseModel> Value { get; set; }
    }

    public class SAPApiResponseModel
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
    }

}
