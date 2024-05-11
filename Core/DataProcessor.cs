using System.Collections.Concurrent;
using System.Threading;

namespace Helpers;

public class DataProcessor
{
    private readonly ConcurrentQueue<string> _buffer = new();
    private CancellationTokenSource _cancellationTokenSource;
    private readonly AutoResetEvent _waitHandle = new(false);
    public event EventHandler<string> DataReceived;

    public DataProcessor()
    {
    }

    public void Start()
    {
        // 启动数据处理线程
        _cancellationTokenSource = new CancellationTokenSource();
        Task.Run(() => ProcessData(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
    }

    private void ProcessData(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (_buffer.TryDequeue(out string data))
            {
                // 处理数据
                DataReceived?.Invoke(this, data);
            }
            else
            {
                // 缓冲区为空，等待新的数据
                _waitHandle.WaitOne();
            }
        }
    }

    public void EnqueueData(string data)
    {
        // 将数据放入缓冲区
        _buffer.Enqueue(data);
        _waitHandle.Set();
    }

    public void Stop()
    {
        _cancellationTokenSource?.Cancel();
    }

}