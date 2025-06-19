using BookStore.Permissions;
using Microsoft.AspNetCore.Authorization;
using Polly.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace BookStore.Authors
{
    public class AuthorAppService : BookStoreAppService, IAuthorAppService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly AuthorManger _authorManager;

        public AuthorAppService(IAuthorRepository authorRepository, AuthorManger authorManager)
        {
            _authorRepository = authorRepository;
            _authorManager = authorManager;
        }

        public async Task<ResultDto<AuthorDto>> CreateAsync(CreateAuthorDto input)
        {
            var author = await _authorManager.CreateAsync(
            input.Name,
            input.BirthDate,
            input.ShortBio);

            await _authorRepository.InsertAsync(author);
            return new ResultDto<AuthorDto>
            {
                code = StatusCode.Success,
                msg = "添加成功",
                Data = ObjectMapper.Map<Author, AuthorDto>(author)
            };
        }
        [Authorize(BookStorePermissions.Authors.Delete)]
        public async Task<ResultDto<AuthorDto>> DeleteAsync(Guid id)
        {
            var author = await _authorRepository.FindAsync(id);
            await _authorRepository.DeleteAsync(author);
            return new ResultDto<AuthorDto>
            {
                code = StatusCode.Success,
                msg = "删除成功",
                Data = ObjectMapper.Map<Author, AuthorDto>(author)
            };
        }

        public async Task<ResultDto<AuthorDto>> GetAsync(Guid id)
        {
            var author = await _authorRepository.GetAsync(id);
            return new ResultDto<AuthorDto>
            {
                code = StatusCode.Success,
                msg = "查询成功",
                Data = ObjectMapper.Map<Author, AuthorDto>(author)
            };
        }

        public async Task<ResultDto<PagedResultDto<AuthorDto>>> GetListAsync(GetAuthorListDto input)
        {
            //默认排序字段
            if (input.Sorting.IsNullOrWhiteSpace())
            {
                input.Sorting = nameof(Author.Name);
            }
            //查询数据
            var authors = await _authorRepository.GetListAsync(
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting,
                input.Filter
            );

            var totalCount = input.Filter == null
                ? await _authorRepository.CountAsync()
                : await _authorRepository.CountAsync(
                    author => author.Name.Contains(input.Filter));

            return new ResultDto<PagedResultDto<AuthorDto>>
            {
                code = StatusCode.Success,
                msg = "查询成功",
                Data = new PagedResultDto<AuthorDto>(
                    totalCount,
                    authors.Select(author => ObjectMapper.Map<Author, AuthorDto>(author)).ToList())
            };
        }
        
        [Authorize(BookStorePermissions.Authors.Edit)]//添加权限
        public async Task<ResultDto<AuthorDto>> UpdateAsync(Guid id, UpdateAuthorDto input)
        {
            //根据id查询信息
            var author = await _authorRepository.GetAsync(id);
            //如果名称不相等
            if (author.Name != input.Name)
            {
                await _authorManager.ChangeNameAsync(author, input.Name);//调用ChangeName方法
            }

            author.BirthDate = input.BirthDate;
            author.ShortBio = input.ShortBio;
            return new ResultDto<AuthorDto>
            {
                code = StatusCode.Success,
                msg = "修改成功",
                Data = ObjectMapper.Map<Author, AuthorDto>(author)
            };
        }
    }
}
