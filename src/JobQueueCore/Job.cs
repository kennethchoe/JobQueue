using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace JobQueueCore
{
    public class Job: IQueueItem
    {
        public bool IsStopped;
        public NoDupCollection<JobTaskBase> JobTasks;
        public Stack<JobTaskBase> ExecutedJobTasks;
        public ILoggerDelegate LoggerDelegate;
        public IProgressDelegate ProgressDelegate;
        public Dictionary<string, object> Parameters;

        public Job()
        {
            JobTasks = new NoDupCollection<JobTaskBase>();
            Parameters = new Dictionary<string, object>();

            LoggerDelegate = new NullLoggerDelegate();
            ProgressDelegate = new NullProgressDelegate();
        }

        public void Execute()
        {
            IsStopped = false;

            LoggerDelegate.Log(LogActivity.JobStarted, ItemDescription);

            ExecutedJobTasks = new Stack<JobTaskBase>();

            foreach (var jobTask in JobTasks)
            {
                if (ProgressDelegate.ShouldStop())
                {
                    IsStopped = true;
                    LoggerDelegate.Log(LogActivity.Stopping, jobTask.JobTaskNameWithOrder());
                    Undo();
                    LoggerDelegate.Log(LogActivity.JobCancelled, ItemDescription);
                    return;
                }

                jobTask.Order = JobTasks.IndexOf(jobTask) + 1;
                ExecutedJobTasks.Push(jobTask);
                TryExecuteJobTask(jobTask);
                LoggerDelegate.Log(LogActivity.TaskFinished, jobTask.JobTaskNameWithOrder());
            }

            LoggerDelegate.Log(LogActivity.JobFinished, ItemDescription);
        }

        private void TryExecuteJobTask(JobTaskBase jobTask)
        {
            try
            {
                jobTask.LoggerDelegate = LoggerDelegate;
                EnrichJobTaskBeforeExecution(jobTask);
                jobTask.Execute();
            }
            catch (Exception e)
            {
                LoggerDelegate.LogError(jobTask.JobTaskNameWithOrder(), e);
                throw;
            }
        }

        protected virtual void EnrichJobTaskBeforeExecution(JobTaskBase jobTask)
        {
            
        }

        public void Undo()
        {
            while (ExecutedJobTasks.Count > 0)
            {
                var jobTask = ExecutedJobTasks.Pop();
                try
                {
                    jobTask.Undo();
                    LoggerDelegate.Log(LogActivity.TaskUndone, jobTask.JobTaskNameWithOrder());
                }
                catch (UndoTaskNotDefinedExcepton)
                {
                    LoggerDelegate.Log(LogActivity.SkippingUndoNotDefined, jobTask.JobTaskNameWithOrder());
                }
            }
        }

        #region Implementation of IQueueItem

        public string ItemId { get; set; }

        public string ItemDescription
        {
            get { return GetType() + " " + ItemId; }
        }

        public virtual string ItemAttributes
        {
            get { return GetItemAttributes(); }
            set { SetItemAttributes(value); }
        }

        private string GetItemAttributes()
        {
            return JsonConvert.SerializeObject(Parameters);
        }

        private void SetItemAttributes(string itemAttributes)
        {
            Parameters = JsonConvert.DeserializeObject<Dictionary<string, object>>(itemAttributes);
        }
        #endregion

        public void DeserializeParameters(string parameters)
        {
            SetItemAttributes(parameters);
        }
    }
}
