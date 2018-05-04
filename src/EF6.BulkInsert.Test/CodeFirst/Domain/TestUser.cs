using EF6.BulkInsert.Test.Domain.ComplexTypes;
using System;

namespace EF6.BulkInsert.Test.Domain
{
    public class TestUser : EntityWithTypedId<Guid>
    {
        public Contact Contact { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return string.Format("{0} {1}", FirstName, LastName); }}
        public DateTime CreatedAt { get; set; }
    }
}