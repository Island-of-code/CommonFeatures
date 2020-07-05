using System;
using System.Threading.Tasks.Dataflow;

namespace DataflowExtensions
{
    //FIFO pattern is used to process items
    public class ActionBlockPerformer<T> : IDisposable
    {
        private readonly ActionBlock<PerfWorkItem<T>> _actionBlock;

        private bool _disposed;

        public event EventHandler<PerfWorkItem<T>> ItemComplete;
        public event EventHandler<Exception> ItemFailed;

        private void RaiseCompleted(PerfWorkItem<T> item)
        {
            ItemComplete?.Invoke(this, item);
        }

        private void RaiseFailed(Exception error)
        {
            ItemFailed?.Invoke(this, error);
        }

        public ActionBlockPerformer(int maxDegreeOfParallelism = 4)
        {
            _actionBlock = new ActionBlock<PerfWorkItem<T>>(item =>
                {
                    if (item.IsCanceled)
                        return;

                    try
                    {
                        item.Action.Invoke(item.Argument);
                        item.RaiseCompleted();
                        RaiseCompleted(item);
                    }
                    catch (Exception e)
                    {
                        item.RaiseFailed(e);
                        RaiseFailed(e);
                    }
                },
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = maxDegreeOfParallelism, MaxMessagesPerTask = int.MaxValue,
                    EnsureOrdered = true
                });

            _actionBlock.Completion.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    ((IDataflowBlock) _actionBlock).Fault(t.Exception);
                    throw new Exception("Error while ActionBlock running", t.Exception);
                }
            });
        }

        public void Dispose()
        {
            _actionBlock.Complete();
            _disposed = true;
        }

        public void Wait(int milliseconds)
        {
            _actionBlock.Completion.Wait(milliseconds);
        }

        public void Send(PerfWorkItem<T> workItem)
        {
            if (_disposed)
                throw new Exception("Dispose method has been called");
            
            _actionBlock.Post(workItem);
        }
    }
}