using System;
using System.Security.Authentication;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SuperSocket.ClientEngine;
using WebSocket4Net;

namespace FtxApi
{
    public class FtxWebSocketApi
    {
        protected string _url;
        protected Action<FtxTicker> _tickerHandler;
        protected Action<FtxOrderEvt> _orderHandler;
        protected Action<string> _defaultHandler;

        protected WebSocket _webSocketClient;

        public Action OnWebSocketConnect;

        public FtxWebSocketApi(string url = "wss://ftx.com/ws/")
        {
            _url = url;
        }

        public async Task Connect(Action<FtxTicker> tickerHandler = null,
            Action<FtxOrderEvt> orderHandler = null,
            Action<string> defaultHandler = null)
        {
            _tickerHandler = tickerHandler;
            _orderHandler = orderHandler;
            _defaultHandler = defaultHandler;
            _webSocketClient = await CreateWebSocket(_url);
            OnWebSocketConnect?.Invoke();
        }

        protected async Task<WebSocket> CreateWebSocket(string url)
        {
            var webSocket = new WebSocket(url);

            webSocket.Security.EnabledSslProtocols = SslProtocols.Tls12;
            webSocket.EnableAutoSendPing = false;
            webSocket.Opened += WebsocketOnOpen;
            webSocket.Error += WebSocketOnError;
            webSocket.Closed += WebsocketOnClosed;
            webSocket.MessageReceived += WebsocketOnMessageReceive;
            webSocket.DataReceived += OnDataRecieved;
            await OpenConnection(webSocket);
            return webSocket;
        }

        int _retries = 0;
        protected async Task OpenConnection(WebSocket webSocket)
        {
            webSocket.Open();

            _retries = 0;
            while (webSocket.State != WebSocketState.Open)
            {
                await Task.Delay(100);
                _retries += 1;
                if (_retries >100)  // 10 seconds timeout
                {
                    throw new Exception("OpenConnection timeout and failed");
                }
            }
        }

        public async Task Stop()
        {
            _tickerHandler = null;
            _orderHandler = null;
            _defaultHandler = null;

            _webSocketClient.Opened -= WebsocketOnOpen;
            _webSocketClient.Error -= WebSocketOnError;
            _webSocketClient.Closed -= WebsocketOnClosed;
            _webSocketClient.MessageReceived -= WebsocketOnMessageReceive;
            _webSocketClient.DataReceived -= OnDataRecieved;
            _webSocketClient?.Close();
            _webSocketClient?.Dispose();

            await Task.Delay(1000);
            // while (_webSocketClient.State != WebSocketState.Closed) await Task.Delay(25);
        }

        protected void WebsocketOnOpen(object sender, EventArgs e)
        {
            Console.WriteLine($"OnOpen: {e}");
        }

        protected void WebSocketOnError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine($"OnError: {e.Exception}");
        }

        protected void WebsocketOnClosed(object o, EventArgs e)
        {
            Console.WriteLine($"OnClosed: {e}");
        }

        protected void WebsocketOnMessageReceive(object o, MessageReceivedEventArgs msg)
        {
            var evt = JsonConvert.DeserializeObject<dynamic>(msg.Message);
            string chn = evt.channel?.ToString() ?? "";
            string type = evt.type?.ToString() ?? "";

            if (type == "pong")
            {
                if (_tickerHandler != null)
                {
                    _tickerHandler(null);
                }
            }
            else if (type == "update" && chn == "trades" && evt.data != null)
            {
                var data = evt.data.Last;

                FtxTicker ticker = new FtxTicker();
                ticker.Instrument = evt.market;
                ticker.LastPrice = data.price;
                ticker.Time = data.time;
                if (_tickerHandler != null)
                {
                    _tickerHandler(ticker);
                }
                else
                {
                    Console.WriteLine($"[ticker] {ticker.Instrument} {ticker.LastPrice} {ticker.Time}");
                }
            }
            else if (type == "update" && chn == "orders" && evt.data != null)
            {
                if (_orderHandler != null)
                {
                    var evt2 = JsonConvert.DeserializeObject<FtxOrderPkg>(msg.Message);
                    _orderHandler(evt2.data);
                }
            }
            else if (_defaultHandler != null)
            {
                _defaultHandler(msg.Message);
            }
        }

        private void OnDataRecieved(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data.Length);
        }

        public void SendCommand(string command)
        {
            _webSocketClient?.Send(command);
        }
    }
}