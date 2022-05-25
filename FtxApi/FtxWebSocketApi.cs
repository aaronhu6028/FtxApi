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
        protected Action<FtxTicker> _handler;

        protected WebSocket _webSocketClient;

        public Action OnWebSocketConnect;

        public FtxWebSocketApi(string url = "wss://ftx.com/ws/")
        {
            _url = url;
        }

        public async Task Connect(Action<FtxTicker> handler = null)
        {
            _handler = handler;
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

        protected async Task OpenConnection(WebSocket webSocket)
        {
            webSocket.Open();

            while (webSocket.State != WebSocketState.Open)
            {
                await Task.Delay(25);
            }
        }

        public async Task Stop()
        {
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
            string chn = evt.channel.ToString();
            string type = evt.type.ToString();
            if (type == "update" && chn == "trades" && evt.data != null)
            {
                var data = evt.data.Last;

                FtxTicker ticker = new FtxTicker();
                ticker.Instrument = evt.market;
                ticker.LastPrice = data.price;
                ticker.Time = data.time;
                if (_handler != null)
                {
                    _handler(ticker);
                }
                else
                {
                    Console.WriteLine($"[ticker] {ticker.Instrument} {ticker.LastPrice} {ticker.Time}");
                }
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