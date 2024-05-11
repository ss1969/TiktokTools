using System.Net.WebSockets;
using System.Text;

namespace Helpers;
public class WebSocketClient
{
    #region Lazy
    private static readonly Lazy<WebSocketClient> _instance = new(() => new());
    public static WebSocketClient Instance => _instance.Value;
    #endregion

    private Uri _server;
    private DataProcessor _dataProcessor;       // 接受数据处理函数
    private ClientWebSocket _webSocket;
    private CancellationTokenSource _cancellationTokenSource;
    private EventHandler<string> statusNotify;

    private WebSocketClient()
    {
        _webSocket = new ClientWebSocket();
        _dataProcessor = new DataProcessor();
    }

    #region Public Methods
    public void SetSeverAddress(Uri uri) => _server = uri;

    public void AddDataHandler(EventHandler<string> eventHandler) => _dataProcessor.DataReceived += eventHandler;

    public void AddStatusHandler(EventHandler<string> eventHandler) => statusNotify += eventHandler;

    public void Start()
    {
        if (_webSocket == null || _webSocket.State != WebSocketState.Open)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _dataProcessor.Start();

            Task.Run(() => ConnectAndReceiveDataAsync(_cancellationTokenSource.Token));
        }
    }

    public async Task StopAsync()
    {
        if (_webSocket != null)
        {
            _cancellationTokenSource.Cancel();
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            _webSocket.Dispose();
            _webSocket = null;
            _dataProcessor.Stop();
        }
    }
    #endregion

    #region Core Task
    private async Task ConnectAndReceiveDataAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                statusNotify?.Invoke(this, $"Connecting to {_server}");
                await _webSocket.ConnectAsync(_server, cancellationToken);
                statusNotify?.Invoke(this, $"Connected!");
                while (_webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
                {
                    byte[] buffer = new byte[4096];
                    WebSocketReceiveResult result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", cancellationToken);
                    }
                    else if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string data = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        _dataProcessor.EnqueueData(data);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                statusNotify?.Invoke(this, "Connection Canceled");
            }
            catch (WebSocketException)
            {
                statusNotify?.Invoke(this, "WebSocketException"); //可依情况添加一些重试逻辑
            }
            catch (Exception ex)
            {
                statusNotify?.Invoke(this, $"Exception {ex}");
            }
            finally
            {
                //添加重试机制
                statusNotify?.Invoke(this, $"Wait 5 Sec to retry");
                await Task.Delay(5000, cancellationToken);
            }
        }
    }
    #endregion
}
