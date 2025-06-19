using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore
{
    public class PageInputDto
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
    public class PageOutputDto<TData>
    {
        public long TotalCount { get; set; }
        public int PageCount { get; set; }
        public List<TData> Data { get; set; }

    }
}
