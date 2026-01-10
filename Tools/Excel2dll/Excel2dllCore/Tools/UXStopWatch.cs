using System;
using System.Collections.Generic;
using System.Text;

namespace Excel2dllCore.Tools
{
    public class UXStopWatch
    {
        private DateTime startTime;
        private DateTime stopTime;

        public void Start()
        {
            startTime = DateTime.Now;
        }

        public void Stop()
        {
            stopTime = DateTime.Now;
        }

        public string Result
        {
            get
            {
                return (stopTime - startTime).ToString();
            }
        }
    }

    public class UXAutoStopWatch : IDisposable
    {
        private UXStopWatch stopWatch;

        public UXAutoStopWatch()
        {
            stopWatch = new UXStopWatch();
            stopWatch.Start();
        }

        public void Dispose()
        {
            stopWatch.Stop();
            Logger.Debug(string.Format("所用时间={0}", stopWatch.Result));
        }
    }

}