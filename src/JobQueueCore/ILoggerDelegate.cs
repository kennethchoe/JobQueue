﻿
using System;

namespace JobQueueCore
{
    public interface ILoggerDelegate
    {
        void Log(LogActivity activity, string subject);
        void LogError(string subject, Exception e);
        void LogDebugInfo(string subject, string debugInfo);
    }

    public enum LogActivity
    {
        ServiceStarted,
        ServiceStopped,
        TaskFinished,
        TaskUndone,
        JobStarted,
        JobFinished,
        ErrorOccurred,
        Stopping,
        JobCancelled,
        EnqueueJobGroup,
        Enqueued,
        Dequeued,
        QueueCleared,
        WaitingForJob,
        SkippingUndoNotDefined
    }
}
