﻿using System;
using FileRepository;
using IntegrationTest.Properties;
using JobQueueCore;
using NUnit.Framework;

namespace IntegrationTest.RepositoryTest
{
    [TestFixture]
    class FileRepositoryBehavior: RepositoryBehavior
    {
        private Logger _logger;

        [SetUp]
        public void SetupQueue()
        {
            JobConfiguration.AppSettings = Settings.Default;

            Repository = new FileQueueRepository<Job>(Environment.CurrentDirectory + "\\queue");
            var fileErrorRep = new FileQueueRepository<Job>(Environment.CurrentDirectory + "\\queue-error");
            _logger = new Logger(Environment.CurrentDirectory + "\\log");
            
            JobQueue = new JobQueue { Repository = Repository, ErroredJobs = fileErrorRep, LoggerDelegate = _logger};
            JobQueue.Clear();
            JobQueue.ErroredJobs.Clear();
        }

    }
}