using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace BookStore.Authors
{
    
    public class GetAuthorListDto : PagedAndSortedResultRequestDto
    {
        //PagedAndSortedResultRequestDto 用于分页和排序
        public string? Filter { get; set; }// 搜索
    }
}
