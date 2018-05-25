using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.PhoneBooks.Persons
{
    public class Person: FullAuditedEntity //Entity<Guid> ,
    {
        public string Name { get; set; }
    }
}
