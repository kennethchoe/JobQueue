using System;
using System.IO;
using JobQueueCore;

namespace FileRepository
{
    public class FileQueueRepository<T> : IQueueRepository<T> where T: IQueueItem 
    {
        private const string FileExtension = ".queue-item.xml";
        private const string BadFileExtension = ".bad";
        private readonly DirectoryInfo _queueDirectory;
        private FileInfo[] _files;

        public FileQueueRepository(string queueFolder)
        {
            if (!queueFolder.EndsWith("\\"))
                queueFolder += "\\";

            _queueDirectory = Directory.CreateDirectory(queueFolder);
        }

        private void RefreshFileList()
        {
            _files = _queueDirectory.GetFiles("*" + FileExtension);
            Array.Sort(_files, (fi1, fi2) => Math.Sign((fi1.CreationTime - fi2.CreationTime).Ticks));
        }

        public T Peek()
        {
            RefreshFileList();
            var fileName = _files[0].FullName;
            return FindItemByFileName(fileName);
        }

        public T FindItemById(string itemId)
        {
            RefreshFileList();
            string fileName = _queueDirectory.FullName + itemId + FileExtension;
            return FindItemByFileName(fileName);
        }

        private T FindItemByFileName(string fileName)
        {
            var file = new FileInfo(fileName);
            if (!file.Exists)
                return default(T);

            var sr = new StreamReader(fileName);

            try
            {
                var assemblyName = sr.ReadLine() + "";
                var className = sr.ReadLine() + "";
                var itemContent = sr.ReadToEnd();

                var itemRaw = Activator.CreateInstance(assemblyName, className).Unwrap();
                var item = itemRaw as IQueueItem;
                if (item == null)
                    throw new ItemDeserializationException("Item deserialization failed.");

                item.ItemContent = itemContent;

                item.ItemId = ExtractItemIdFromFileName(fileName);

                return (T) item;
            }
            catch (Exception e)
            {

                throw new ItemDeserializationException(e.Message);
            }
            finally
            {
                sr.Close();
            }
        }

        public void MarkPeekItemAsBad()
        {
            RefreshFileList();
            var fileName = _files[0].FullName;

            new FileInfo(fileName).MoveTo(fileName + BadFileExtension);
        }

        private string ExtractItemIdFromFileName(string fullFilePath)
        {
            var beginning = fullFilePath.LastIndexOf("\\", StringComparison.Ordinal);
            var fileNameOnly = fullFilePath.Substring(beginning + 1);
            return fileNameOnly.Substring(0, fileNameOnly.IndexOf(FileExtension, StringComparison.Ordinal));
        }

        public int Count()
        {
            RefreshFileList();
            return _files.Length;
        }

        public string Enqueue(T item)
        {
            string itemId = item.ItemId;
            
            if (item.ItemId == null)
                itemId = Guid.NewGuid().ToString();

            string fileName = _queueDirectory.FullName + itemId + FileExtension;

            var ws = new StreamWriter(fileName);
            ws.WriteLine(item.GetType().Assembly);
            ws.WriteLine(item.GetType());
            ws.WriteLine(item.ItemContent);
            ws.Close();

            return itemId;
        }

        public T Dequeue()
        {
            var item = Peek();

            var fileName = _files[0].FullName;
            new FileInfo(fileName).Delete();

            return item;
        }

        public void Clear()
        {
            RefreshFileList();
            foreach (var fileInfo in _files)
                new FileInfo(fileInfo.FullName).Delete();
        }
    }
}
