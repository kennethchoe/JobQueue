using System;
using JobQueueCore;

namespace IntegrationTest.RepositoryTest
{
    public class NotDeserializableJob: Job
    {
        public new string ItemAttributes
        {
            get { return ""; }
            set { throw new NotDeserializableJobException(); }
        }
    }

    public class NotDeserializableJobException: Exception
    {
        
    }
}
