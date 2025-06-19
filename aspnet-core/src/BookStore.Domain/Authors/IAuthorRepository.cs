using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace BookStore.Authors
{
    /// <summary>
    /// 作者仓储
    /// </summary>
    public interface IAuthorRepository : IRepository<Author, Guid>
    {
        /// <summary>
        /// 通过作者名称查询作者
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<Author> FindByNameAsync(string name);
        /// <summary>
        /// 获取作者列表
        /// </summary>
        /// <param name="skipCount"></param>
        /// <param name="maxResultCount"></param>
        /// <param name="sorting"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<List<Author>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting,
            string filter = null
        );

    }
}
