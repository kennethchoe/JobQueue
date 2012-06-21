About JobQueue
=============================================
A extendable job queue system with 
- Windows service for job monitoring/execution,
- Web service for job queue control, and
- .Net class library (DLL) for defining Jobs.

Immediate goal is to facilitate SQL script for ETL (extract-transform-load) job so that ETL job can be triggered as soon as its previous step is finished. If there is ETL job running already, new request will be queued and then executed as soon as current ETL job is done.

For more, see [.\doc\JobQueue Project.pptx](https://github.com/kennethchoe/JobQueue/raw/master/doc/JobQueue%20Project.pptx).

![JobQueue Process Flow](https://github.com/kennethchoe/JobQueue/raw/master/doc/JobQueue.jpg)

=============================================
PreRequisites
=============================================
Visual Studio 2010 with SP1,
SQL Express 2008 R2


=============================================
How to test-drive the dev environment
=============================================

1. Run click_to_build.bat
2. Run \register_JobExecutionService.bat As Administrator.
3. Open \src\JobQueue.sln in Visual Studio - Visual Studio should run as Administrator.
4. Run JobQueueManager web application. Enqueue some jobs.
5. On Windows Explorer, go to \src\JobQueueManager\bin\queue.
   You should see one *.queue-item.xml file is created. That is one job registered on the job queue.

6. Relaunch JobQueueManager web application and start the service.
7. On Windows Explorer, go to \src\JobQueueManager\bin\log.
   You should see log saying the item from #5-6 is executed and dequeued.
8. On Windows Explorer, go to \src\JobQueueManager\bin\queue.
   You should see the one *.queue-item.xml file is now gone.


=============================================
How to create new SqlJob Library
=============================================

1. The Project should have reference to SqlJobExtention.dll and JobQueueCore.dll.
2. Job Class should inherit from SqlJob.
    e.g. public class SyncTransactions: SqlJob
3. The JobLibrary project should have Sql folder, and Job class name folder.
4. Under the job class name folder, put your SQL statements in one more more files.
    e.g. Sql\SyncTransactions\01_GetSnapshot.sql
5. If you need to define undo command for them, put SQL filename + ".undo" as extra.
    e.g. Sql\SyncTransactions\01_GetSnapshot.sql.undo
