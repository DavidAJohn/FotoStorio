namespace FotoStorio.Shared.Entities
{
    public class ProductParameters
    {
        const int maxPageSize = 50; 
        public int PageIndex { get; set; } = 1; 
        private int _pageSize = 10; 
        public int PageSize 
        { 
            get 
            { 
                return _pageSize; 
            } 
            set 
            { 
                _pageSize = (value > maxPageSize) ? maxPageSize : value; 
            }
        }

        public string Search { get; set; } = "";
        public int CategoryId { get; set; } = 0;
        public int BrandId { get; set; } = 0;
        public string SortBy { get; set; }
    }
}