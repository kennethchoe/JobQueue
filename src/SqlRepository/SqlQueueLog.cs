using System;

namespace SqlRepository
{
    public class SqlQueueLog
    {
        public long Id { get; set; }
        public string LogText { get; set; }
        public string ErrorDetails { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
