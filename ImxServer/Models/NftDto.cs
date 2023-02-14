namespace ImxServer.Models
{
    public class NftDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Image_url { get { return Image; } }
        public int Id { get; set; }
    }
}
