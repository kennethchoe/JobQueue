using System;
using System.Collections.Generic;

namespace JobQueueCore
{
    public class Job: IQueueItem
    {
        public bool IsStopped;
        public NoDupCollection<CommandBase> Commands;
        public Stack<CommandBase> ExecutedCommands;
        public ILoggerDelegate LoggerDelegate;
        public IProgressDelegate ProgressDelegate;
        public Dictionary<string, object> Parameters;

        public Job()
        {
            Commands = new NoDupCollection<CommandBase>();
            Parameters = new Dictionary<string, object>();

            LoggerDelegate = new NullLoggerDelegate();
            ProgressDelegate = new NullProgressDelegate();
        }

        public void Execute()
        {
            IsStopped = false;

            LoggerDelegate.Log(LogActivity.JobStarted, ItemDescription);

            ExecutedCommands = new Stack<CommandBase>();

            foreach (var command in Commands)
            {
                if (ProgressDelegate.ShouldStop())
                {
                    IsStopped = true;
                    LoggerDelegate.Log(LogActivity.Stopping, command.CommandNameWithOrder());
                    Undo();
                    LoggerDelegate.Log(LogActivity.JobCancelled, ItemDescription);
                    return;
                }

                command.Order = Commands.IndexOf(command) + 1;
                LoggerDelegate.Log(LogActivity.CommandStarted, command.CommandNameWithOrder());
                ExecutedCommands.Push(command);
                TryExecuteCommand(command);
                LoggerDelegate.Log(LogActivity.CommandFinished, command.CommandNameWithOrder());
            }

            LoggerDelegate.Log(LogActivity.JobFinished, ItemDescription);
        }

        private void TryExecuteCommand(CommandBase command)
        {
            try
            {
                command.LoggerDelegate = LoggerDelegate;
                EnrichCommandBeforeExecution(command);
                command.Execute();
            }
            catch (Exception e)
            {
                LoggerDelegate.LogError(command.CommandNameWithOrder(), e);
                throw;
            }
        }

        protected virtual void EnrichCommandBeforeExecution(CommandBase command)
        {
            
        }

        public void Undo()
        {
            while (ExecutedCommands.Count > 0)
            {
                var command = ExecutedCommands.Pop();
                command.Undo();
                LoggerDelegate.Log(LogActivity.CommandUndone, command.CommandNameWithOrder());
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
            string getParam = "";
            foreach (var parameter in Parameters)
            {
                getParam += "," + parameter.Value;
            }

            if (getParam != "")
                getParam = getParam.Substring(1);

            return getParam;
        }

        private void SetItemAttributes(string itemAttributes)
        {
            int i = 0;
            var newParams = new Dictionary<string, object>();

            if (itemAttributes != null)
            {
                var parameterArray = itemAttributes.Split(',');
                foreach (var parameter in Parameters)
                {
                    newParams.Add(parameter.Key, parameterArray[i]);
                    i++;
                }
            }
            
            Parameters = newParams;
        }
        #endregion

        public void DeserializeParameters(string parameters)
        {
            SetItemAttributes(parameters);
        }
    }
}
