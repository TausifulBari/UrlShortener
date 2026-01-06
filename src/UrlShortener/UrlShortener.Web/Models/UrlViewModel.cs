namespace UrlShortener.Web.Models
{
    public class UrlViewModel
    {
        public Guid? Id { get; set; }
        public string? LongUrl { get; set; }
        public string? ShortUrl { get; set; }
        public string? Code { get; set; }
        public DateTime? CreatedTime { get; set; }
    }
}
