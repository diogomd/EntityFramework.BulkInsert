using System.Data.Entity.Spatial;

namespace EF6.BulkInsert.Test.Domain
{
    public class PinPoint : Entity
    {
        public string Name { get; set; }

        public DbGeography Coordinates { get; set; }
    }
}