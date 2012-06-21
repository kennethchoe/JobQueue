using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using Dapper;
using DapperExtensions;
using DapperExtensions.Mapper;
using JobQueueCore;

namespace SqlRepository
{
    public class SqlQueueRepository<T> : IQueueRepository<T> where T : IQueueItem
    {
        public SqlConnection Connection;

        public SqlQueueRepository(SqlConnection connection)
        {
            Connection = connection;
            DapperExtensions.DapperExtensions.DefaultMapper = typeof(PluralizedAutoClassMapper<>);
        }

        public T Peek()
        {
            var itemRecord = GetFirstItemRecord();
            return BuildItemFromItemRecord(itemRecord);
        }

        public void MarkPeekItemAsBad()
        {
            var itemRecord = GetFirstItemRecord();

            Connection.Open();
            Connection.Execute("update ItemRecords set IsBad = 1 where id = @id", new { id = itemRecord.Id });
            Connection.Close();
        }

        public T FindItemById(string itemId)
        {
            var itemRecord = GetItemRecordById(int.Parse(itemId));
            return BuildItemFromItemRecord(itemRecord);
        }

        private T BuildItemFromItemRecord(ItemRecord itemRecord)
        {
            if (itemRecord == null)
                return default(T);

            try
            {
                var itemRaw = Activator.CreateInstance(itemRecord.AssemblyName, itemRecord.ClassName).Unwrap();
                var item = itemRaw as IQueueItem;
                if (item == null)
                    throw new ItemDeserializationException("Item deserialization failed.");

                item.ItemContent = itemRecord.ItemContent;
                item.ItemId = itemRecord.Id.ToString(CultureInfo.InvariantCulture);

                return (T) item;
            }
            catch (Exception e)
            {
                throw new ItemDeserializationException(e.Message);
            }
        }

        private ItemRecord GetFirstItemRecord()
        {
            Connection.Open();
            var itemRecords = Connection.Query<ItemRecord>("select top 1 * from ItemRecords where IsBad = 0 order by Id");
            Connection.Close();

            var itemRecord = itemRecords.FirstOrDefault();
            return itemRecord;
        }

        private ItemRecord GetItemRecordById(int id)
        {
            Connection.Open();
            var itemRecords = Connection.Query<ItemRecord>("select * from ItemRecords where Id = @Id", new { Id = id });
            Connection.Close();

            var itemRecord = itemRecords.FirstOrDefault();
            return itemRecord;
        }

        public int Count()
        {
            Connection.Open();
            var counts = Connection.Query<int>("select itemCount = count(*) from ItemRecords where IsBad = 0");
            Connection.Close();

            return counts.First();
        }

        public string Enqueue(T item)
        {
            var assemblyName = item.GetType().Assembly.ToString();
            var className = item.GetType().ToString();
            var itemContent = item.ItemContent;

            Connection.Open();
            var newItem = new ItemRecord { ItemName = item.ItemDescription, AssemblyName = assemblyName, ClassName = className, ItemContent = itemContent };
            Connection.Insert(newItem);
            Connection.Close();
            return newItem.Id.ToString(CultureInfo.InvariantCulture);
        }

        public T Dequeue()
        {
            var item = Peek();

            Connection.Open();
            Connection.Execute("delete from ItemRecords where id in (select min(id) from ItemRecords where IsBad = 0)");
            Connection.Close();

            return item;
        }

        public void Clear()
        {
            Connection.Open();
            Connection.Execute("delete from ItemRecords");
            Connection.Close();
        }

    }
}
