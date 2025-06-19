using BookStore.Permissions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BookStore.Books
{
    public interface IBookAppService : IApplicationService
    //ICrudAppService< //Defines CRUD methods
    //BookDto, //Used to show books
    //Guid, //Primary key of the book entity
    //PagedAndSortedResultRequestDto, //Used for paging/sorting
    //CreateUpdataBookDto> //Used to create/update a book
    {
        //添加方法
        Task<ResultDto<BookDto>> InsertAsync(CreateUpdataBookDto input);
        //定义查询方法
        Task<ResultDto<PagedResultDto<BookDto>>> GetListAsync(BookType? type);
        //定义查询方法 根据书籍类型查询
        Task<ResultDto<List<BookDto>>> GetListByTypeAsync(BookType type);
        //定义查询方法 根据书籍id查询
        Task<ResultDto<BookDto>> GetById(Guid id);
        Task<ResultDto<PageOutputDto<BookDto>>> GetPagesAsync(BookSearchDto booksearckDto);
        Task<ResultDto<BookDto>> UpdateBookAsync(Guid id, CreateUpdataBookDto input);
        Task<ResultDto<BookDto>> DeleteBookAsync(Guid id);


    }
}
