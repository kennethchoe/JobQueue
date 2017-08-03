using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using Dapper;
using JobQueueCore;

namespace SqlRepository
{
    public class SqlQueueRepository<T> : IQueueRepository<T> where T : IQueueItem
    {
        public SqlConnection Connection;
        private readonly string _queueTableName;

        public SqlQueueRepository(SqlConnection connection, string tableName)
        {
            Connection = connection;
            _queueTableName = tableName;
            //DapperExtensions.DapperExtensions.DefaultMapper = typeof(PluralizedAutoClassMapper<>);
        }

        public T Peek()
        {
            var itemRecord = GetFirstItemRecord();
            return BuildItemFromItemRecord(itemRecord);
        }

        public void MarkPeekItemAsBad()
        {
            var itemRecord = GetFirstItemRecord();

            EnsureConnectionIsOpen();
            Connection.Execute("update " + _queueTableName + " set IsBad = 1 where id = @id", new { id = itemRecord.Id });
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

                item.ItemAttributes = itemRecord.ItemAttributes;
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
            EnsureConnectionIsOpen();
            var itemRecords = Connection.Query<ItemRecord>("select top 1 * from " + _queueTableName + " where IsBad = 0 order by Id");
            Connection.Close();

            var itemRecord = itemRecords.FirstOrDefault();
            return itemRecord;
        }

        private ItemRecord GetItemRecordById(int id)
        {
            EnsureConnectionIsOpen();
            var itemRecords = Connection.Query<ItemRecord>("select * from " + _queueTableName + " where Id = @Id", new { Id = id });
            Connection.Close();

            var itemRecord = itemRecords.FirstOrDefault();
            return itemRecord;
        }

        public int Count()
        {
            EnsureConnectionIsOpen();
            var counts = Connection.Query<int>("select itemCount = count(*) from " + _queueTableName + " where IsBad = 0");
            Connection.Close();

            return counts.First();
        }

        public string Enqueue(T item)
        {
            var assemblyName = item.GetType().Assembly.ToString();
            var className = item.GetType().ToString();
            var itemAttributes = item.ItemAttributes;

            EnsureConnectionIsOpen();
            if (item.ItemId == null)
            {
                var id = Connection.Query<decimal>(@"
                        insert into " + _queueTableName + @"(ItemName, AssemblyName, ClassName, ItemAttributes) values(@a, @b, @c, @d)
                        select scope_identity()",
                    new {a = item.ItemDescription, b = assemblyName, c = className, d = itemAttributes}).Single();
                item.ItemId = id.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                Connection.Execute("SET IDENTITY_INSERT " + _queueTableName + " ON");

                Connection.Execute(@"
                        insert into " + _queueTableName + @"(Id, ItemName, AssemblyName, ClassName, ItemAttributes) values(@i, @a, @b, @c, @d)",
                    new { i = item.ItemId, a = item.ItemDescription, b = assemblyName, c = className, d = itemAttributes });

                Connection.Execute("SET IDENTITY_INSERT " + _queueTableName + " OFF");
              
            }

            Connection.Close();
            return item.ItemId;
        }

        public T Dequeue()
        {
            var item = Peek();

            EnsureConnectionIsOpen();
            Connection.Execute("delete from " + _queueTableName + " where id in (select min(id) from " + _queueTableName + " where IsBad = 0)");
            Connection.Close();

            return item;
        }

        public void Clear()
        {
            EnsureConnectionIsOpen();
            Connection.Execute("delete from " + _queueTableName);
            Connection.Close();
        }

        private void EnsureConnectionIsOpen()
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
        }
    }
}
