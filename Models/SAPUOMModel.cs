namespace SapGateway.Models
{
    public class SAPUOMModel
    {
        public List<ValueModel>? Value { get; set; }

        public class ValueModel
        {
            public string? Code { get; set; }
            public string? Name { get; set; }
        }
    }
}
