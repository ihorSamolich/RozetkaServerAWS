namespace WebRozetka.Models
{
    public class QueryParameters
    {
        private const int maxPageCount = 50;
        public int Page { get; set; } = 1;

        private int _pageCount = maxPageCount;
        public int PageCount
        {
            get { return _pageCount; }
            set { _pageCount = (value > maxPageCount) ? maxPageCount : value; }
        }

        public string Query { get; set; } = "";
        public int CategoryId { get; set; } = 0;
        public int PriceMin { get; set; } = 0;
        public int PriceMax { get; set; } = 0;
        public int QuantityMin { get; set; } = 0;
        public int QuantityMax { get; set; } = 0;
    }
}
