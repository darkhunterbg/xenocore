using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Threading
{
   public class PerThreadData<T> 
    {
        private T[] threadData;
        public T Current
        {
            get
            {
                return threadData[JobService.CurrentThread.ThreadIndex];
            }
        }
        public IEnumerable<T> Data
        {
            get { return threadData; }
        }


        public PerThreadData()
        {
            threadData = new T[JobService.Threads.Length];
            for (int i = 0; i < threadData.Length; ++i)
                threadData[i] = Activator.CreateInstance<T>();
        }

     
        
    }
}
