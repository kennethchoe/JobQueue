using System;

namespace JobQueueCore
{
    public static class JobFactory
    {
        public static Job Build(string assemblyName, string className)
        {
            var obj = CreateInstance(assemblyName, className);

            var job = obj as Job;
            if (job == null)
            {
                throw new Exception("The class is not Job class.");
            }

            return job;
        }

        public static JobGroup BuildJobGroup(string assemblyName, string className)
        {
            var obj = CreateInstance(assemblyName, className);

            var jobGroup = obj as JobGroup;
            if (jobGroup == null)
            {
                throw new Exception("The class is not JobGroup class.");
            }

            return jobGroup;
        }

        private static object CreateInstance(string assemblyName, string className)
        {
            var obj = Activator.CreateInstance(assemblyName, className).Unwrap();
            if (obj == null)
            {
                throw new Exception("The class not found.");
            }
            return obj;
        }
    }
}
