using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FotoStorio.Server.Helpers
{
    public class PaginationResponseHeader
    {
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}
