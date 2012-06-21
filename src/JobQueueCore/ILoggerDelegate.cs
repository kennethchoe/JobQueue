﻿
using System;

namespace JobQueueCore
{
    public interface ILoggerDelegate
    {
        void Log(LogActivity activity, string subject);
        void LogError(string subject, Exception e);
    }

    public enum LogActivity
    {
        ServiceStarted,
        ServiceStopped,
        CommandStarted,
        CommandFinished,
        CommandUndone,
        JobStarted,
        JobFinished,
        ErrorOccurred,
        Stopping,
        JobCancelled,
        Enqueued,
        Dequeued,
        QueueCleared,
        WaitingForJob
    }
}