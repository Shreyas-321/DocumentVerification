namespace webApitest.Models
{
    public class GeoTable
    {
        public int Id { get; set; }
        public int UploadId { get; set; }
        public string Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class NominatimResult
    {
        public string lat { get; set; }
        public string lon { get; set; }
        public string display_name { get; set; }
    }


}
