using System;
using System.IO;
using JobQueueCore;

namespace FileRepository
{
    public class FileQueueRepository<T> : IQueueRepository<T> where T: IQueueItem 
    {
        private const string FileExtension = ".queue-item.xml";
        private const string BadFileExtension = ".bad";
        private const string ItemIdFile = "_itemid";
        private readonly string _queueFolder;
        private string[] _files;

        public FileQueueRepository(string queueFolder)
        {
            if (!queueFolder.EndsWith("\\"))
                queueFolder += "\\";

            _queueFolder = queueFolder;
            Directory.CreateDirectory(queueFolder);
        }

        private void RefreshFileList()
        {
            _files = Directory.GetFiles(_queueFolder, "*" + FileExtension);
            Array.Sort(_files);
        }

        public T Peek()
        {
            RefreshFileList();
            var fileName = _files[0];
            return FindItemByFileName(fileName);
        }

        public T FindItemById(string itemId)
        {
            RefreshFileList();
            string fileName = _queueFolder + itemId + FileExtension;
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
                var itemAttributes = sr.ReadToEnd();

                var itemRaw = Activator.CreateInstance(assemblyName, className).Unwrap();
                var item = itemRaw as IQueueItem;
                if (item == null)
                    throw new ItemDeserializationException("Item deserialization failed.");

                item.ItemAttributes = itemAttributes;

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
            var fileName = _files[0];

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
                itemId = GetNewId().ToString("00000000000000000000");

            string fileName = _queueFolder + itemId + FileExtension;

            var ws = new StreamWriter(fileName);
            ws.WriteLine(item.GetType().Assembly);
            ws.WriteLine(item.GetType());
            ws.WriteLine(item.ItemAttributes);
            ws.Close();

            return itemId;
        }

        private long GetNewId()
        {
            long newId;

            var idFileInfo = new FileInfo(_queueFolder + ItemIdFile);
            if (!idFileInfo.Exists)
                newId = 1;
            else
            {
                var idReader = new StreamReader(_queueFolder + ItemIdFile);
                newId = int.Parse(idReader.ReadLine()) + 1;
                idReader.Close();
            }

            var idFile = new StreamWriter(_queueFolder + ItemIdFile, false);
            idFile.WriteLine(newId);
            idFile.Close();

            return newId;
        }

        public T Dequeue()
        {
            var item = Peek();

            var fileName = _files[0];
            new FileInfo(fileName).Delete();

            return item;
        }

        public void Clear()
        {
            RefreshFileList();
            foreach (var fileName in _files)
                new FileInfo(fileName).Delete();
        }
    }
}
