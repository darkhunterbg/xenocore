using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XenoCore.Engine.Console;
using XenoCore.Engine.Logging;
using XenoCore.Engine.Utilities;

namespace XenoCore.Engine.Threading
{

    public class JobService
    {
        private static JobService instance;

        private SemaphoreSlim semaphore;
        private Stopwatch watch = new Stopwatch();
        private ThreadData[] workerThreads;
        private ThreadData mainThread;

        private bool profiling = false;

        private bool kill = false;
        private Object mutex = new object();
        private int runningWorkerThreads = 0;
        private int[] cores;

        private IThreadProvider threadProvider;
        private ThreadData[] threads;

        private ListArray<JobTask> tasks = new ListArray<JobTask>(1024);
        private ConcurrentQueue<Job> jobQueue = new ConcurrentQueue<Job>();
        private ThreadLocal<ThreadData> currentThread = new ThreadLocal<ThreadData>();

        public static ThreadData CurrentThread
        {
            get { return instance.currentThread.Value; }
        }
        public static ThreadData[] Threads { get { return instance.threads; } }
        public static IThreadProvider ThreadProvider { get { return instance.threadProvider; } }

        public static bool IsProfiling { get { return instance.profiling; } }

        private JobService()
        {

        }

        public static void Initialize(IThreadProvider threadProvider)
        {
            Debug.Assert(instance == null, "JobService is already initialized!");

            instance = new JobService()
            {
                threadProvider = threadProvider,
                semaphore = new SemaphoreSlim(0),
                cores = threadProvider.GetAvailableCores(true)
            };

            int maxThreads =  instance.cores.Length;
            instance.workerThreads = new ThreadData[maxThreads - 1];

            for (int i = 0; i < instance.workerThreads.Length; ++i)
                instance.workerThreads[i] = new ThreadData();

            instance.currentThread.Value = instance.mainThread = new ThreadData();
            var threads = new List<ThreadData>() { instance.mainThread };
            threads.AddRange(instance.workerThreads);
            for (int i = 0; i < threads.Count; ++i)
                threads[i].ThreadIndex = i;

            instance.threads = threads.ToArray();

            ThreadProvider.LockCurrentThreadToCore(instance.mainThread.Core);

            instance.mainThread.Core = instance.cores[0];
            instance.mainThread.Name = $"Main Thread";
            instance.mainThread.IsRunning = true;
            instance.mainThread.IsSleeping = false;
            instance.mainThread.ThreadID = ThreadProvider.GetCurrentThreadID();

            foreach (var thread in instance.workerThreads)
                ThreadProvider.StartThread(instance.ThreadWork);

            //Wait for all worker threads to start
            while (instance.runningWorkerThreads < instance.workerThreads.Length) ;
        }
        public static void Uninitialize()
        {
            Debug.Assert(instance != null, "JobService is not initialized!");

            instance.kill = true;
            if (instance.workerThreads.Length > 0)
                instance.semaphore.Release(instance.workerThreads.Length);

            while (instance.runningWorkerThreads > 0) ;

            instance.semaphore.Dispose();

            ThreadProvider.UnlockCurrentThread();

            instance = null;
        }

        public static JobTask RunJobs(Job[] jobs, int categoryid = -1)
        {
            JobTask task = instance.tasks.NewInterlocked();
            task.RemainingCounter = jobs.Length;
            task.CategoryID = categoryid;

            if (CurrentThread.StackCounter > 0)
            {
                for (int i = 0; i < jobs.Length; ++i)
                {
                    jobs[i].Task = task;
                    CurrentThread.JobStack.Push(jobs[i]);
                }
            }
            else
            {
                for (int i = 0; i < jobs.Length; ++i)
                {
                    jobs[i].Task = task;
                    instance.jobQueue.Enqueue(jobs[i]);
                }

                int awake = Math.Min(task.RemainingCounter, instance.workerThreads.Length);

                if (awake > 0)
                    instance.semaphore.Release(awake);
            }


            return task;
        }
        public static void WaitForJobs(JobTask task)
        {
            do
            {
                instance.ProcessJobQueue();
            }
            while (task.RemainingCounter > 0 && !instance.kill);

        }
        public static void Reset()
        {
            foreach (var thread in Threads)
            {
                thread.JobStatsCount = 0;
            }
            instance.tasks.ClearInterlocked();
        }
        public static void StartDataCollection()
        {
            Debug.Assert(!instance.profiling, "Profiling already started!");
            foreach (var thread in Threads)
            {
                for (int i = 0; i < thread.JobStatsCount; ++i)
                {
                    thread.JobStats[i].StartTime = thread.JobStats[i].EndTime = -1;
                    thread.JobStats[i].Stack = -1;
                }

                thread.JobStatsCount = 0;
            }
            instance.watch.Restart();
            instance.profiling = true;
        }
        public static void EndDataCollection()
        {
            Debug.Assert(instance.profiling, "Profiling not started!");
            instance.watch.Stop();
            foreach (var thread in Threads)
            {
                for (int i = 0; i < thread.JobStatsCount; ++i)
                    Debug.Assert(thread.JobStats[i].EndTime >= 0, "Thread job stats failure!");
            }
            instance.profiling = false;
        }

        private bool ProcessJobQueue()
        {
            Job job;

            if (CurrentThread.JobStack.Count > 0)
            {
                job = CurrentThread.JobStack.Pop();
                ProcessJob(job);
                return true;
            }
            if (jobQueue.TryDequeue(out job))
            {
                ProcessJob(job);
                return true;
            }

            return false;
        }
        private void ProcessJob(Job job)
        {
            ConfirmCoreLocked();

            int i = currentThread.Value.JobStatsCount++;

#if DEV
            if (profiling)
                StartProfileJob(job, i);
#endif

            job.Action(job.Param);

#if DEV
            if (profiling)
                EndProfileJob(i);
#endif

            Interlocked.Decrement(ref job.Task.RemainingCounter);
        }
        private void ThreadWork()
        {
            int workerThreadIndex = Interlocked.Increment(ref runningWorkerThreads);

            var thread = workerThreads[workerThreadIndex - 1];
            thread.Core = cores[workerThreadIndex];
            thread.Name = $"Worker Thread { workerThreadIndex }";
            thread.ThreadID = ThreadProvider.GetCurrentThreadID();
            currentThread.Value = thread;

            ConsoleService.Msg($"Staring {thread.Name}");

            thread.IsRunning = true;

            ThreadProvider.LockCurrentThreadToCore(thread.Core);
            while (!kill)
            {
                thread.IsSleeping = true;
                //ConsoleService.Msg( $"{thread.Name} sleeping");

                semaphore.Wait();

                thread.IsSleeping = false;
                //ConsoleService.Msg( $"{thread.Name} awakened");

                while (!kill && ProcessJobQueue()) ;

            }
            ThreadProvider.UnlockCurrentThread();

            thread.IsRunning = false;

            Interlocked.Decrement(ref runningWorkerThreads);

            ConsoleService.Msg($"Shutting down {thread.Name}");
        }

        [Conditional("DEBUG")]
        private void ConfirmCoreLocked()
        {
            int coreid = ThreadProvider.GetCurrentCore();
            int threadid = ThreadProvider.GetCurrentThreadID();

            Debug.Assert(CurrentThread.Core == coreid, "Thread switched core!");

            Debug.Assert(CurrentThread.ThreadID == threadid, "Thread switched native thread!");
        }

        private void StartProfileJob(Job job, int i)
        {
            ++CurrentThread.StackCounter;

            CurrentThread.JobStats[i].Stack = CurrentThread.StackCounter - 1;
            CurrentThread.JobStats[i].Job = job;

            lock (mutex)
                CurrentThread.JobStats[i].StartTime = watch.ElapsedTicks;

        }
        private void EndProfileJob(int i)
        {
            lock (mutex)
                CurrentThread.JobStats[i].EndTime = watch.ElapsedTicks;

            --CurrentThread.StackCounter;
        }

        public override string ToString()
        {
            return "JobService";
        }
    }
}
