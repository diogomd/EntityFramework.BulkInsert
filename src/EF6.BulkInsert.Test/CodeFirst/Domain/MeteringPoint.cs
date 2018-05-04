using System;

namespace EF6.BulkInsert.Test.Domain
{
    public class MeteringPoint : Entity, ICreatedAt, IModifiedAt
    {
        public string EIC { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}