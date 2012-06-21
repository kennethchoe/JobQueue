using System;
using System.Collections.Generic;
using System.Linq;

namespace JobQueueCore
{
    class InMemoryQueueRepository<T> : IQueueRepository<T> where T: IQueueItem
    {
        private readonly Queue<T> _queue;

        public InMemoryQueueRepository()
        {
            _queue = new Queue<T>();
        }

        public T Peek()
        {
            return _queue.Peek();
        }

        public void MarkPeekItemAsBad()
        {
            _queue.Dequeue();
        }

        public int Count()
        {
            return _queue.Count();
        }

        public string Enqueue(T item)
        {
            if (item.ItemId == "")
                item.ItemId = Guid.NewGuid().ToString();

            _queue.Enqueue(item);
            return item.ItemId;
        }

        public T Dequeue()
        {
            return _queue.Dequeue();
        }

        public void Clear()
        {
            _queue.Clear();
        }

        public T FindItemById(string itemId)
        {
            return _queue.SingleOrDefault(x => x.ItemId == itemId);
        }
    }
}
