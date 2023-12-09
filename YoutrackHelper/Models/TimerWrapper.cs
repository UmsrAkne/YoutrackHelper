using System;
using System.Timers;

namespace YoutrackHelper.Models
{
    public class TimerWrapper : IDisposable
    {
        private readonly Timer timer;
        private bool disposed;

        public TimerWrapper(ElapsedEventHandler elapsedEventHandler)
        {
            timer = new Timer();
            timer.Elapsed += elapsedEventHandler;
        }

        public double Interval { get => timer.Interval; set => timer.Interval = value; }

        private bool IsRunning { get; set; }

        public void Start()
        {
            if (IsRunning)
            {
                return;
            }

            timer.Start();
            IsRunning = true;
        }

        public void Stop()
        {
            if (!IsRunning)
            {
                return;
            }

            timer.Stop();
            IsRunning = false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                timer.Stop();
                timer.Dispose();
            }

            disposed = true;
        }
    }
}