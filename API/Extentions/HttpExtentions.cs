using API.Helpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Extentions
{
    public static class HttpExtentions
    {
        public static void AddPaginationHeader(this HttpResponse response ,int currentPage , int itemsPerPage,
            int totalItems, int totalPages)
        {
            var paginationHeader = new PaginationHeader(currentPage, totalPages, itemsPerPage, totalItems);
            response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }


    }
}
