

using Microsoft.EntityFrameworkCore;

namespace API.Helpers
{
    public class PagedList<T> : List<T>
    {
        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            // eg 10 items in db, pageSize of 4. 10/4=2.5, take the ceiling so get 3 page
            TotalPages = (int) Math.Ceiling(count / (double) pageSize);
            PageSize = pageSize; // how many user do we want to display in one page
            TotalCount = count;

            // return it with a list of items
            AddRange(items);
        }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize) {
            // return count of the items from the query
            var count = await source.CountAsync();
            /*
            * skip(): - how many do we want to skip?
                      - pageNumber - 1 (if we on first page, dont want to skip anything)
            */
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}