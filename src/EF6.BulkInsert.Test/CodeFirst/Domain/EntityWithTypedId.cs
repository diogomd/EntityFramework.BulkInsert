namespace EF6.BulkInsert.Test.Domain
{
    public abstract class EntityWithTypedId<T>
    {
        public T Id { get; set; }
    }
}