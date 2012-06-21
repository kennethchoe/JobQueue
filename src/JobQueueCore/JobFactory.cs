using System;

namespace JobQueueCore
{
    public static class JobFactory
    {
        public static Job Build(string assemblyName, string className, string parameters)
        {
            var obj = Activator.CreateInstance(assemblyName, className).Unwrap();
            var job = obj as Job;

            if (job == null)
            {
                throw new Exception("Job class not found.");
            }

            job.ItemContent = parameters;
            return job;
        }
    }
}
