using System;

namespace DataflowExtensions
{
    public class PerfWorkItem<T>
    {
        public PerfWorkItem(Action<T> action)
        {
            Action = action;
        }

        public bool IsCanceled { get; set; }
        public Action<T> Action { get; set; }
        public Exception Error { get; set; }
        public T Argument { get; set; }
        public event EventHandler<T> Completed;
        public event EventHandler<Exception> Failed;

        public void RaiseCompleted()
        {
            Completed?.Invoke(this, Argument);
        }

        public void RaiseFailed(Exception error)
        {
            Error = error;
            Failed?.Invoke(this, error);
        }
    }
}