using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Diagnostics.Contracts;
using System.IO;
using MessagePack;
using System.Threading.Tasks;

namespace MCTProcon29Protocol
{
    public class IPCManager
    {
        static IPCManager()
        {
            MessagePack.Resolvers.CompositeResolver.RegisterAndSetAsDefault(
                new MessagePack.Formatters.IMessagePackFormatter[] { new ColoredBoardFormatter() }, new[] { MessagePack.Resolvers.StandardResolver.Instance });
        }

        TcpListener listener;
        TcpClient client;
        NetworkStream stream;

        IIPCClientReader clientReader;
        IIPCServerReader serverReader;

        Queue<byte[]> writeQueue = new Queue<byte[]>();

        CancellationTokenSource Canceller;

        int _port = 0;
        bool isClient;
        bool isStopRequired = false;

        public event Action<Exception> OnExceptionThrown;

        public async Task StartAsync(int port)
        {
            var ipAddress = Dns.GetHostEntry("localhost").AddressList[0];

            if (isClient)
                _port = port;
            else
            {
                listener = new TcpListener(ipAddress, port);
                listener.Start();
            }
            Canceller = new CancellationTokenSource();
            await Task.Run(() =>
            {
                try
                {
                    client = isClient ? new TcpClient("localhost", _port) : listener.AcceptTcpClient();
                }
                catch (SocketException ex)
                {
                    if (ex.ErrorCode != 10004)
                        System.Diagnostics.Debugger.Break();
                    return;
                }
                stream = client.GetStream();
            });
            await ServerMainAction();
        }

        public void Start(int port)
        {
            var ipAddress = Dns.GetHostEntry("localhost").AddressList[0];

            if (isClient)
                _port = port;
            else
            {
                listener = new TcpListener(ipAddress, port);
                listener.Start();
            }
            Canceller = new CancellationTokenSource();
            try
            {
                client = isClient ? new TcpClient("localhost", _port) : listener.AcceptTcpClient();
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode != 10004)
                    System.Diagnostics.Debugger.Break();
                return;
            }
            stream = client.GetStream();
            var IPCThread = ServerMainAction();
            IPCThread.Wait();
        }


    public IPCManager(IIPCClientReader client)
        {
            Contract.Requires(client != null);
            isClient = true;
            this.clientReader = client;
        }

        public IPCManager(IIPCServerReader server)
        {
            Contract.Requires(server != null);
            isClient = false;
            this.serverReader = server;
        }

        public async Task ServerMainAction()
        {
            client.ReceiveTimeout = Timeout.Infinite;

            int bufferSize = 0;
            int messageSize = 0;
            Methods.DataKind currentKind = 0;
            int current = 0;
            byte[] headBuffer = new byte[4];
            byte[] messageBuffer = new byte[1024];

            CancellationToken cancelToken = Canceller.Token;

            while (true)
            {
                try
                {
                    while (true)
                    {
                        bufferSize = await stream.ReadAsync(headBuffer, 0, 4, cancelToken);
                        if (isStopRequired)
                            return;
                        if (bufferSize == 4)
                        {
                            messageSize = BitConverter.ToInt32(headBuffer, 0); // little endian
                            current = 0;
                            goto data_kind_read_start;
                        }
                    }
                    data_kind_read_start:
                    while (true)
                    {
                        bufferSize = await stream.ReadAsync(headBuffer, 0, 4, cancelToken);
                        if (isStopRequired)
                            return;
                        if (bufferSize == 4)
                        {
                            currentKind = (Methods.DataKind)BitConverter.ToInt32(headBuffer, 0); // little endian
                            goto message_read_start;
                        }
                    }
                    message_read_start:

                    byte[] currentBuffer = messageBuffer.Length < messageSize ? new byte[messageSize] : messageBuffer;
                    while (current < messageSize)
                    {
                        bufferSize = await stream.ReadAsync(currentBuffer, current, messageSize, cancelToken);
                        current += bufferSize;
                        if (isStopRequired)
                            return;
                    }
                    if (isClient)
                    {
                        switch (currentKind)
                        {
                            case Methods.DataKind.GameInit:
                                clientReader.OnGameInit(MessagePackSerializer.Deserialize<Methods.GameInit>(currentBuffer));
                                break;
                            case Methods.DataKind.TurnStart:
                                clientReader.OnTurnStart(MessagePackSerializer.Deserialize<Methods.TurnStart>(currentBuffer));
                                break;
                            case Methods.DataKind.TurnEnd:
                                clientReader.OnTurnEnd(MessagePackSerializer.Deserialize<Methods.TurnEnd>(currentBuffer));
                                break;
                            case Methods.DataKind.GameEnd:
                                clientReader.OnGameEnd(MessagePackSerializer.Deserialize<Methods.GameEnd>(currentBuffer));
                                break;
                            case Methods.DataKind.Pause:
                                clientReader.OnPause(MessagePackSerializer.Deserialize<Methods.Pause>(currentBuffer));
                                break;
                            case Methods.DataKind.Interrupt:
                                clientReader.OnInterrupt(MessagePackSerializer.Deserialize<Methods.Interrupt>(currentBuffer));
                                break;
                            case Methods.DataKind.RebaseByUser:
                                clientReader.OnRebaseByUser(MessagePackSerializer.Deserialize<Methods.RebaseByUser>(currentBuffer));
                                break;
                            default:
                                throw new FormatException();
                        }
                    }
                    else
                    {
                        switch(currentKind)
                        {
                            case Methods.DataKind.Connect:
                                serverReader.OnConnect(MessagePackSerializer.Deserialize<Methods.Connect>(currentBuffer));
                                break;
                            case Methods.DataKind.Decided:
                                serverReader.OnDecided(MessagePackSerializer.Deserialize<Methods.Decided>(currentBuffer));
                                break;
                            case Methods.DataKind.Interrupt:
                                serverReader.OnInterrupt(MessagePackSerializer.Deserialize<Methods.Interrupt>(currentBuffer));
                                break;
                            default:
                                throw new FormatException();
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex is TimeoutException) continue;
                    if (ex is ObjectDisposedException) return;
                    if (ex.InnerException is SocketException && ((SocketException)(ex.InnerException)).ErrorCode == 10060) continue;
                    OnExceptionThrown?.Invoke(ex);
                    if (ex is IOException) return;
                }
            }
        }

        public void Write<T>(Methods.DataKind datakind, T data)
        {
            byte[] message = MessagePackSerializer.Serialize(data);
            byte[] cache = BitConverter.GetBytes(message.Length);
            stream.Write(cache, 0, cache.Length);
            cache = BitConverter.GetBytes((int)datakind);
            stream.Write(cache, 0, cache.Length);
            stream.Write(message, 0, message.Length);
        }

        public void Shutdown()
        {
            isStopRequired = true;
            Canceller?.Cancel();
            Thread.Sleep(800);
            stream?.Close();
            client?.Close();
            listener?.Stop();
            Canceller.Dispose();
        }
    }
}
