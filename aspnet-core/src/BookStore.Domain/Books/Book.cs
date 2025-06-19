using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace BookStore.Books
{
    public class Book : FullAuditedAggregateRoot<Guid>
    {
        //基类 AuditedAggregateRoot 有添加的审计字段
        //基类 FullAuditedAggregateRoot 有所有的审计字段
        //基类 Entity 只有一个主键泛型的Id
        public string Name { get; set; }

        public BookType Type { get; set; }

        public DateTime PublishDate { get; set; }

        public float Price { get; set; }
    }
}
