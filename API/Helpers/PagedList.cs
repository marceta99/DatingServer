using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class PagedList<T> : List<T>
    {
        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            PageSize = pageSize;
            TotalCount = count;
            this.AddRange(items); //dodaje ove iteme na kraj liste , a posto je ova nasa klasa sama lista onda na kraju nje
        }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber , int pageSize)
        {

            //u ovoj metodi kreiramo i vracamo novu isntancu PageList klase 


            var count = await source.CountAsync(); //ovo CountAsync vraca broj elemenata iz baze na osnovu tog source-a.

            /***********************************/
            //bitno je zapamtiti da se ovi upiti (query), pokreću nad bazom tek kada pozovemo toListAsync() ili ToList() metodu
            /**********************************/

            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            //ovim upitom gore cemo preskociti npr ako smo uneli da hocemo da odemo na stranu 2 , onda cemo preskociti sve
            //elemente sa prve strane. A hocemo da prikazemo naredni pageSize elemenata tj koliko ima elemenata na toj 2. strani

            return new PagedList<T>(items, count, pageNumber, pageSize);

        
        
        }

    }
}
