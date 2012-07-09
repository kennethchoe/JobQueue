using System;

namespace SqlRepository
{
    public class ItemRecord
    {
        public long Id { get; set; }
        public string AssemblyName { get; set; }
        public string ClassName { get; set; }
        public string ItemName { get; set; }
        public string ItemAttributes { get; set; }
        public bool IsBad { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
