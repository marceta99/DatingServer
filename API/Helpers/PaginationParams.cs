using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class PaginationParams
    {
        private int _pageSize = 10; //default page size

        private const int maxPageSize = 50;
        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > maxPageSize ? maxPageSize : value; //ako je broj strana veci od 50, vrati 50
        }
    }
}
