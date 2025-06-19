using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace BookStore.Authors
{
    /// <summary>
    /// 作者实体表
    /// </summary>
    public class Author:FullAuditedAggregateRoot<Guid>
    {
        //private set对于该属性，限制为从此类之外设置此属性
        public string Name { get; private set; } 
        public DateTime BirthDate { get; set; }
        public string ShortBio { get; set; }
        
        private Author()
        {
            /* This constructor is for deserialization / ORM purpose */
        }

        internal Author(
            Guid id,
            string name,
            DateTime birthDate,
            string? shortBio = null)
            : base(id)
        {
            SetName(name);
            BirthDate = birthDate;
            ShortBio = shortBio;
        }

        internal Author ChangeName(string name)
        {
            SetName(name);
            return this;
        }
        
        private void SetName(string name)
        {
            Name = Check.NotNullOrWhiteSpace(
                name,
                nameof(name),
                maxLength: AuthorConsts.MaxNameLength
            );
        }
    }
}
