namespace SapGateway.Endpoints
{
    public class DocumentModel
    {
        public int DocumentId { get; set; }
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string DocumentType { get; set; }
        public string CompanyName { get; set; }
        public bool IsOpened { get; set; }
    }
}