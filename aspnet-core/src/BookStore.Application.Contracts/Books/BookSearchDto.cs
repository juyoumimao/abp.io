using BookStore.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Books
{
    public class BookSearchDto:PageInputDto
    {
        public string? Keywords { get; set; }
    }
}
