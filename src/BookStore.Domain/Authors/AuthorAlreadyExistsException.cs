using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;

namespace BookStore.Authors
{
    //BusinessException是一种特殊的异常类型,由 ABP 自动处理
    public class AuthorAlreadyExistsException : BusinessException
    {
        //构造函数
        public AuthorAlreadyExistsException(string name)
            : base(BookStoreDomainErrorCodes.AuthorAlreadyExists)
        {
            WithData("name", name);
        }
    }
}
