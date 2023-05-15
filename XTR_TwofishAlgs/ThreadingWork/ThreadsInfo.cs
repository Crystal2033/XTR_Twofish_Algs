using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace XTR_TWOFISH.ThreadingWork
{
    public static class ThreadsInfo
    {
        //static ThreadsInfo()
        //{
        //    int coreCount = 0;
        //    foreach (var item in new ManagementObjectSearcher("Select * from Win32_Processor").Get())
        //    {
        //        coreCount += int.Parse(item["NumberOfCores"].ToString());
        //    }
        //    VALUE_OF_THREAD = coreCount;
        //    Console.WriteLine("Количество ядер: {0}", coreCount);
        //    Console.WriteLine("Количество логических процессоров: {0}", Environment.ProcessorCount);
        //}
        public static readonly int VALUE_OF_THREAD = Environment.ProcessorCount; 
    }
}
