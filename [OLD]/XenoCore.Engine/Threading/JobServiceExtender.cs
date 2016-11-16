using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Entities;
using XenoCore.Engine.Utilities;

namespace XenoCore.Engine.Threading
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BatchedJobData
    {
        public int StartIndex;
        public int EndIndex;
    }

    public static class JobServiceExtender
    {
        public static JobTask RunJobs<T>(List<T> array, Action<Object> callback, int categoryid = -1) 
        {

            Job[] jobs = new Job[array.Count];

            for (int i = 0; i < array.Count; ++i)
            {
                jobs[i].Action = callback;
                jobs[i].Param = array[i];
            }

            return JobService.RunJobs(jobs);
        }

        public static JobTask RunJobs<T>(ComponentContainer<T> array, Action<Object> callback, int categoryid = -1) where T : Component
        {

            Job[] jobs = new Job[array.Count];

            for (int i = 0; i < array.Count; ++i)
            {
                jobs[i].Action = callback;
                jobs[i].Param = array[i];
            }

            return JobService.RunJobs(jobs);
        }
        public static JobTask RunJobs<T>(ListArray<T> array, Action<Object> callback, int categoryid = -1) where T : class
        {
            Job[] jobs = new Job[array.Count];

            for (int i = 0; i < array.Count; ++i)
            {
                jobs[i].Action = callback;
                jobs[i].Param = array[i];
            }

            return JobService.RunJobs(jobs);
        }

        public static JobTask RunJobsBatched(int count, int batchSize, Action<Object> callback, int categoryid = -1)
        {
            int batchesCount = count / batchSize + (count % batchSize > 0 ? 1 : 0);

            Job[] jobs = new Job[batchesCount];

            for (int i = 0; i < batchesCount; ++i)
            {
                BatchedJobData data = new BatchedJobData()
                {
                    StartIndex = i * batchSize,
                    EndIndex = Math.Min(count - i * batchSize, batchSize) + i * batchSize,
                };
                jobs[i].Action = callback;
                jobs[i].Param = data;
            }
            return JobService.RunJobs(jobs);
        }

    }
}
