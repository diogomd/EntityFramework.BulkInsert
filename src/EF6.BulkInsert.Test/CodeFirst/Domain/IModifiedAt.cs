using System;

namespace EF6.BulkInsert.Test.Domain
{
    public interface IModifiedAt
    {
        DateTime? ModifiedAt { get; set; }
    }
}