About JobQueue
=============================================
An extensible job queue system written in C#.

Immediate goal was to facilitate SQL script for ETL (extract-transform-load) job so that ETL job can be triggered as soon as its previous step is finished. If there is ETL job running already, new request will be queued and then executed as soon as current ETL job is done.

For more, see [.\doc\JobQueue Project.pptx](https://github.com/kennethchoe/JobQueue/raw/master/doc/JobQueue%20Project.pptx).

Defining Jobs
=============================================
Create .Net class library (DLL).

Defining a command:

    class MyFirstCommand: CommandBase
    {
        public override string CommandName()
        {
            return "Command Name";
        }

        public override void Execute()
        {
            // C# code
        }

        public override void Undo()
        {
            // C# code. Undo override is optional.
        }
    }

Defining a job:

    class MyFirstJob: Job
    {
        public MyFirstJob()
        {
            Commands.Add(new MyFirstCommand());
            Commands.Add(new MySecondCommand());
        }
    }

Defining a job group:

    public class MyJobGroup: JobGroup
    {
        public override string[] EnqueueOnJobQueue(JobQueue jobQueue)
        {
            var jobIds = new List<string>()
                {
                    EnqueueJob<MyFirstJob>(jobQueue), 
                    EnqueueJob<MySecondJob>(jobQueue)
                };
            return jobIds.ToArray();
        }
    }

Putting a job or job group to a job queue:

    (new MyJobGroup()).EnqueueOnJobQueue(_jobQueue);


Controlling JobQueue and Job Execution Service 
=============================================
All activities are controllable through URL. 
This feature was needed to support controlling from legacy application that is not easy to embed .Net code but has a way to make web service call.
For more complete usage, check out JobQueueManager.csproj.

Putting a job or job group using web API:

    http://JobQueueManager/api/enqueuejob/?assemblyName=MyJobLibrary&className=MyFirstJob
    http://JobQueueManager/api/enqueuejobgroup/?assemblyName=MyJobLibrary&className=MyJobGroup
    
Starting and Stopping Job Execution Service:

    http://JobQueueManager/api/startservice
    http://JobQueueManager/api/stopservice


Extensibility
=============================================

- You can hook up the job queue to file or sql repository for the queue persistency.
- You can define parameters. See SampleSqlJobLibrary and SampleSqlJobLibraryTest for more details.
- I implemented SqlJobExtension to solve my original problem.

How to create SqlJob Library:

1. The Project should have reference to SqlJobExtention.dll and JobQueueCore.dll.
2. Create three folders on The JobLibrary project.
    * `JobGroups\` : home for job groups.
    * `Jobs\`      : home for jobs.
    * `JobSqls\`   : home for *.sql files.
2. Job Class should inherit from SqlJob instead of Job.
4. Under the job class name folder, put your SQL statements in one more more files.
    * e.g. `JobSqls\MySqlJob\01_GetSnapshot.sql`
5. If you need to define undo command for them, put SQL filename + `.undo` as extra.
    * e.g. `JobSqls\MySqlJob\01_GetSnapshot.sql.undo`


PreRequisites
=============================================
Visual Studio 2010 with SP1,
SQL Express 2008 R2


How to test-drive the dev environment
=============================================

1. Run `click_to_build.bat`
2. Run `\register_JobExecutionService.bat` As Administrator.
3. Open `\src\JobQueue.sln` in Visual Studio - Visual Studio should run as Administrator.
4. Run JobQueueManager web application. Enqueue some jobs.
5. On Windows Explorer, go to `\src\JobQueueManager\bin\queue`.
   You should see one *.queue-item.xml file is created. That is one job registered on the job queue.

6. Relaunch JobQueueManager web application and start the service.
7. On Windows Explorer, go to `\src\JobQueueManager\bin\log`.
   You should see log saying the item from #5-6 is executed and dequeued.
8. On Windows Explorer, go to `\src\JobQueueManager\bin\queue`.
   You should see the one *.queue-item.xml file is now gone.


