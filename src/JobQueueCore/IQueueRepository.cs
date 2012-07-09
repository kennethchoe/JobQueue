using System;

namespace JobQueueCore
{
    public interface IQueueRepository<T> where T: IQueueItem
    {
        T Peek();
        void MarkPeekItemAsBad();
        int Count();
        string Enqueue(T item);
        T Dequeue();
        void Clear();
        T FindItemById(string itemId);
    }

    public interface IQueueItem
    {
        string ItemId { get; set; }
        string ItemDescription { get; }
        string ItemAttributes { get; set; }
    }

    public class ItemDeserializationException : Exception
    {
        public ItemDeserializationException(string message): base(message)
        {
        }
    }
}