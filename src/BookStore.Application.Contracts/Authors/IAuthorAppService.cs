using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BookStore.Authors
{
    public interface IAuthorAppService: IApplicationService
    {
        Task<ResultDto<AuthorDto>> GetAsync(Guid id);

        Task<PagedResultDto<AuthorDto>> GetListAsync(GetAuthorListDto input);

        Task<ResultDto<AuthorDto>> CreateAsync(CreateAuthorDto input);

        Task<ResultDto<AuthorDto>> UpdateAsync(Guid id, UpdateAuthorDto input);

        Task<ResultDto<AuthorDto>> DeleteAsync(Guid id);
    }
}
