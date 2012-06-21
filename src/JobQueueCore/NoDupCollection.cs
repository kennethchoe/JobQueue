using System;
using System.Collections.ObjectModel;

namespace JobQueueCore
{
    public class NoDupCollection<T> : Collection<T>
    {
        protected override void InsertItem(int index, T item)
        {
            if (Contains(item))
                throw new DuplicateItemException();

            base.InsertItem(index, item);
        }
    }

    public class DuplicateItemException : Exception
    {
        public override string Message
        {
            get
            {
                return "Cannot add one instance twice. Instantiate another copy."; 
            }
        }
    }
}
