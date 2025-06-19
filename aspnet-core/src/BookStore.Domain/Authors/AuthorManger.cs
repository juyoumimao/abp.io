using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace BookStore.Authors
{
    //AuthorManager强制以受控方式创建作者并更改作者的姓名。应用层将使用这些方法
    public class AuthorManger: DomainService
    {
        private readonly IAuthorRepository _authorRepository;

        public AuthorManger(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }
        //添加检查
        public async Task<Author> CreateAsync(
            string name,
            DateTime birthDate,
            string? shortBio = null)
        {
            //检查是否存在
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var existingAuthor = await _authorRepository.FindByNameAsync(name);
            //如果存在作者，则抛出异常
            if (existingAuthor != null)
            {
                throw new AuthorAlreadyExistsException(name);
            }
            //返回新的作者
            return new Author(
                GuidGenerator.Create(),
                name,
                birthDate,
                shortBio
            );
        }
        //更改检查
        public async Task ChangeNameAsync(
            Author author,
            string newName)
        {
            
            Check.NotNull(author, nameof(author));//检查对象是否为null
            //检查字符串是否为null、空字符串或仅包含空白字符
            Check.NotNullOrWhiteSpace(newName, nameof(newName));
            //通过名称查询数据信息
            var existingAuthor = await _authorRepository.FindByNameAsync(newName);
            //如果存在作者，并且id不相等，则抛出异常
            if (existingAuthor != null && existingAuthor.Id != author.Id)
            {
                throw new AuthorAlreadyExistsException(newName);
            }

            author.ChangeName(newName);//调用ChangeName方法
        }
    }
}
