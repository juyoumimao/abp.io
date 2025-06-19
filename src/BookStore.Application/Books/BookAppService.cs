using AutoMapper;
using BookStore.Authors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace BookStore.Books
{
    [ApiExplorerSettings(GroupName = "v2")]
    public class BookAppService : ApplicationService,
    //CrudAppService<
    //     Book, //The Book entity 实体
    //     BookDto, //Used to show books 显示的书籍dto
    //     Guid, //Primary key of the book entity 主键类型
    //     PagedAndSortedResultRequestDto, //Used for paging/sorting 分页排序
    //     CreateUpdataBookDto>, //Used to create/update a book 创建更新 dto
    IBookAppService
    {


        //private readonly IRepository<Book, Guid> repository;

        //public BookAppService(IRepository<Book, Guid> repository)
        //    : base(repository)
        //{
        //    this.repository = repository;
        //}
        //public override async Task<BookDto> CreateAsync(CreateUpdataBookDto input)
        //{
        //    if (await repository.AnyAsync(x => x.Name == input.Name))
        //    {
        //        throw new Exception("重名");
        //    }
        //    return await base.CreateAsync(input);

        //}

        //两个重载
        private readonly IRepository<Book, Guid> repository;
        private readonly IAuthorRepository authorRepository;

        public BookAppService(IRepository<Book, Guid> repository, IAuthorRepository authorRepository)
        {
            this.repository = repository;
            this.authorRepository = authorRepository;
        }
        /// <summary>
        /// 根据书籍Id查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResultDto<BookDto>> GetById(Guid id)
        {
            //var res = repository.GetListAsync(x=>x.Id==id);
            var book = await repository.FindAsync(id);
            var dto=ObjectMapper.Map<Book,BookDto>(book);
            return ResultDto<BookDto>.Success(ResultCode.Ok, dto);
        }
        /// <summary>
        /// 查询所有书籍
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResultDto<PagedResultDto<List<BookDto>>>> GetListAsync(BookType? type)
        {
            //var list1=await repository.GetListAsync();
            //var list2=await repository.GetPagedListAsync(0,10,string.Empty);//(跳过多少条数据,最多返回多少条数据,排序)
            //var list3 = (await repository.GetQueryableAsync()).ToList();
            //var list4=await repository.ToListAsync();
            var query = await repository.GetQueryableAsync();//获取可查询对象
            if (type != null)
            {
                query = query.Where(x => x.Type == type);
            }
            var dto=ObjectMapper.Map<List<Book>,PagedResultDto<List<BookDto>>>(query.ToList());
            return ResultDto<PagedResultDto<List<BookDto>>>.Success(ResultCode.Ok, dto);
        }
        public async Task<ResultDto<PageOutputDto<BookDto>>> GetPagesAsync([FromQuery] BookSearchDto booksearckDto)
        {
            //var list = await repository.GetQueryableAsync();
            //list = list.WhereIf(!string.IsNullOrEmpty(booksearckDto.Keywords), x => x.Name.Contains(booksearckDto.Keywords));
            ////list = list.Where("", booksearckDto.);
            ////Page只返回当前页的数据
            ////var result1 =list.Page(booksearckDto.PageIndex,booksearckDto.PageSize).ToList();
            ////PageResult返回当前页的数据和总页数和总条数
            //var result = list.PageResult(booksearckDto.PageIndex, booksearckDto.PageSize);


            ////var res1 = list.Select(x => ObjectMapper.Map<Book, BookDto>(x)).ToList();
            //var res = ObjectMapper.Map<List<Book>, List<BookDto>>(result.Queryable.ToList());
            //int count =  res.Count();

            ////Get the IQueryable<Book> from the repository
            //var queryable = await repository.GetQueryableAsync();
            ////Prepare a query to join books and authors
            //var query = from book in queryable
            //            join author in await authorRepository.GetQueryableAsync() on book.Id equals author.Id
            //            select new { book, author };

            //query = query
            //.OrderBy("book.name")
            //.Skip(0)
            //.Take(10);

            ////Execute the query and get a list
            //var queryResult = await AsyncExecuter.ToListAsync(query);

            //var bookDtos = queryResult.Select(x =>
            //{
            //    var bookDto = ObjectMapper.Map<Book, BookDto>(x.book);
            //    bookDto.Name = x.author.Name;
            //    return bookDto;
            //}).ToList();

            ////Get the total count with another query
            //var totalCount = await repository.GetCountAsync();

            //PageOutputDto<BookDto> DTO = new PageOutputDto<BookDto>()
            //{
            //    TotalCount = totalCount,
            //    //PageCount=
            //    Data = bookDtos
            //};
            var booklist = await repository.GetQueryableAsync();
            var totalCount = booklist.Count();
            var totalPage=(int)Math.Ceiling((double)totalCount/ booksearckDto.PageSize);
            var pagesbookx=booklist.OrderBy(x=>x.Id).Skip((booksearckDto.PageIndex- 1)*booksearckDto.PageSize).Take(totalCount).ToList();
            return ResultDto<PageOutputDto<BookDto>>.Success(ResultCode.Ok, new PageOutputDto<BookDto>
            {
                Data=ObjectMapper.Map<List<Book>,List<BookDto>>(pagesbookx),
                TotalCount=totalCount,
                PageCount= totalPage
            });
          
        }
        public Task<ResultDto<List<BookDto>>> GetListByTypeAsync(BookType type)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="input">dto</param>
        /// <returns></returns>
        public async Task<ResultDto<BookDto>> InsertAsync(CreateUpdataBookDto input)
        {
            // 1. 将输入的DTO对象映射为领域实体Book
            // 这样可以将前端传递的数据转换为数据库实体对象，便于后续持久化
            var book = ObjectMapper.Map<CreateUpdataBookDto, Book>(input);

            // 2. 调用仓储的InsertAsync方法将Book实体插入数据库
            // InsertAsync会自动保存更改，并返回插入后的实体（包含主键等数据库生成的字段）
            book = await repository.InsertAsync(book);

            // 3. 将插入后的Book实体再次映射为BookDto用于返回
            // 这样可以保证返回给前端的数据是DTO格式，符合接口规范
            var bookDto = ObjectMapper.Map<Book, BookDto>(book);

            return ResultDto < BookDto >.Success(ResultCode.Ok, bookDto);
            
        }

        public async Task<ResultDto<BookDto>> UpdateBookAsync(Guid id, CreateUpdataBookDto input)
        {
            var book=await repository.FindAsync(id);
            //book.Name = input.Name;
            //book.Price = input.Price;
            //book.PublishDate = input.PublishDate;
            //book.Type = input.Type;

            var dto1=ObjectMapper.Map<CreateUpdataBookDto, Book>(input,book);
            var dto2 = ObjectMapper.Map(input, book);
            
     
            await repository.UpdateAsync(dto1);
            var res = ObjectMapper.Map<Book, BookDto>(dto1);
            return ResultDto < BookDto >.Success (ResultCode.Ok, res);
           
        }
        public async Task<ResultDto<BookDto>> DeleteBookAsync(Guid id)
        {
            var book = await repository.FindAsync(id);
            await repository.DeleteAsync(book);
            var res = ObjectMapper.Map<Book, BookDto>(book);
            return ResultDto<BookDto>.Success(ResultCode.Ok, res);
          
        }
    }
}
