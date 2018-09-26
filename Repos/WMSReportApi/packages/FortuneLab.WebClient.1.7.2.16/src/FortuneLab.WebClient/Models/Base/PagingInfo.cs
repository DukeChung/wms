namespace FortuneLab.WebClient.Models
{
    public class PagingInfo
    {
        public int Index { get; set; }
        public int Total { get; set; }
        public int Size { get; set; }
        public bool HasNextPage { get; set; }
    }
}