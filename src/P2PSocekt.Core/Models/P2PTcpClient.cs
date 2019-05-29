using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;

namespace P2PSocket.Core.Models
{
    public class P2PTcpClient : TcpClient
    {
        public P2PTcpClient() : base() { }
        public P2PTcpClient(AddressFamily family) : base(family) { }
        public P2PTcpClient(IPEndPoint localEP) : base(localEP) { }
        public P2PTcpClient(TcpClient tcpClient) : base()
        {
            CopyTcpClientSocket(tcpClient);
        }

        public P2PTcpClient(Socket socket) : base()
        {
            Client = socket;
            Active = Client.Connected;
            m_remoteEndPoint = socket.RemoteEndPoint.ToString();
            m_localEndPoint = socket.LocalEndPoint.ToString();
        }
        public P2PTcpClient(string hostname, int port) : base()
        {
            if (Proxy != null && !string.IsNullOrEmpty(Proxy.IP) && Proxy.Address.Contains(hostname))
            {
                CopyTcpClientSocket(ConnectProxy(hostname, port));
            }
            else
            {
                CopyTcpClientSocket(new TcpClient(hostname, port));
            }
        }

        /// <summary>
        ///     从TcpClient中复制属性
        /// </summary>
        /// <param name="tcpClient"></param>
        public void CopyTcpClientSocket(TcpClient tcpClient)
        {
            ReceiveTimeout = tcpClient.ReceiveTimeout;
            ReceiveBufferSize = tcpClient.ReceiveBufferSize;
            NoDelay = tcpClient.NoDelay;
            LingerState = tcpClient.LingerState;
            ExclusiveAddressUse = tcpClient.ExclusiveAddressUse;
            Client = tcpClient.Client;
            SendBufferSize = tcpClient.SendBufferSize;
            SendTimeout = tcpClient.SendTimeout;
            Active = tcpClient.Connected;
            m_remoteEndPoint = Client.RemoteEndPoint.ToString();
            m_localEndPoint = Client.LocalEndPoint.ToString();
        }

        /// <summary>
        ///     连接代理服务器http模式
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        protected virtual TcpClient ConnectProxy(string hostname, int port)
        {
            TcpClient client = null;
            switch (Proxy.ProxyType.ToLower())
            {
                case "http":
                    {
                        client = ConnectHttpProxy(hostname, port);
                    }
                    break;
                case "socket":
                    {
                        client = ConnectSocketProxy(hostname, port);
                    }
                    break;
                default:
                    {
                        client = null;
                    }
                    break;
            }
            return client;
        }
        /// <summary>
        ///     连接http类型的代理
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        protected virtual TcpClient ConnectHttpProxy(string hostname, int port)
        {
            TcpClient client = new TcpClient(Proxy.IP, Proxy.Port);
            NetworkStream ss = client.GetStream();
            string header = "";
            if (string.IsNullOrEmpty(Proxy.UserName))
            {
                header = String.Format("CONNECT {0}:{1} HTTP/1.1\r\nUser-Agent: Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/6.0)\r\n Proxy-Connection: Keep-Alive\r\n", hostname, port);
            }
            else
            {
                string authrity = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", Proxy.UserName, Proxy.Password)));
                header = String.Format("CONNECT {0}:{1} HTTP/1.1\r\nUser-Agent: Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/6.0)\r\n Proxy-Connection: Keep-Alive\r\nProxy-Authorization: Basic {2} \r\n\r\n", hostname, port, authrity);
            }
            byte[] request = System.Text.Encoding.ASCII.GetBytes(header);
            ss.Write(request, 0, request.Length);
            byte[] readBuffer = new byte[10240];
            int length = ss.Read(readBuffer, 0, readBuffer.Length);
            String tempstr = System.Text.Encoding.UTF8.GetString(readBuffer.Take(length).ToArray());
            string[] spStr = tempstr.Split(' ');
            if (!spStr.Contains("200"))
            {
                throw new Exception("连接失败！");
            }
            return client;
        }

        protected virtual TcpClient ConnectSocketProxy(string hostname, int port)
        {
            TcpClient ret = null;
            try
            {
                ret = ConnectSocket4Proxy(hostname, port);
            }
            catch
            {
                ret = ConnectSocket5Proxy(hostname, port);
            }
            return ret;
        }
        protected virtual TcpClient ConnectSocket4Proxy(string hostname, int port)
        {
            TcpClient client = null;
            try
            {
                client = new TcpClient(Proxy.IP, Proxy.Port);
            }
            catch {
                throw new Exception("连接Socket4服务器失败");
            }
            NetworkStream ss = client.GetStream();
            List<byte> bytes = new List<byte>() { 04, 01 };
            ushort intTemp = (ushort)port;
            bytes.AddRange(BitConverter.GetBytes(intTemp).Reverse());
            bytes.AddRange(IPAddress.Parse(hostname).GetAddressBytes());
            bytes.Add(0x00);
            ss.Write(bytes.ToArray(), 0, bytes.Count);
            byte[] readBuffer = new byte[28];
            int length = ss.Read(readBuffer, 0, 28);
            if (length == 8 && readBuffer[1] == 0x5A)
            {
            }
            else
            {
                //代理失败
                throw new Exception("使用Socket4代理失败");
            }
            return client;
        }
        protected virtual TcpClient ConnectSocket5Proxy(string hostname, int port)
        {
            TcpClient client = null;
            try
            {
                client = new TcpClient(Proxy.IP, Proxy.Port);
            }
            catch
            {
                throw new Exception("使用Socket5代理失败");
            }
            NetworkStream ss = client.GetStream();
            List<byte> bytes = new List<byte>() { 05, 02, 00, 02 };
            ss.Write(bytes.ToArray(), 0, bytes.Count);
            byte[] readBuffer = new byte[28];
            int length = ss.Read(readBuffer, 0, 28);
            if (length == 2 && readBuffer[1] == 0)
            {
                bytes.Clear();
                bytes.AddRange(new byte[] { 05, 01, 00, 01 });
                bytes.AddRange(IPAddress.Parse(hostname).GetAddressBytes());
                bytes.AddRange(BitConverter.GetBytes((ushort)port).Reverse());
                ss.Write(bytes.ToArray(), 0, bytes.Count);
                length = ss.Read(readBuffer, 0, 28);
                if (length != 10) throw new Exception("使用Socket5代理失败");

            }
            else if (length == 2 && readBuffer[1] == 2)
            {
                bytes.Clear();
                bytes.AddRange(new byte[] { 01 });
                byte[] userName =Encoding.UTF8.GetBytes(Proxy.UserName);
                bytes.Add((byte)userName.Length);
                bytes.AddRange(userName);
                byte[] password = Encoding.UTF8.GetBytes(Proxy.Password);
                bytes.Add((byte)password.Length);
                bytes.AddRange(password);
                ss.Write(bytes.ToArray(), 0, bytes.Count);
                length = ss.Read(readBuffer, 0, 28);
                if (length >= 2 && readBuffer[1] != 0) throw new Exception("使用Socket5代理失败，密码错误");
            }
            else
            {
                //代理失败
                throw new Exception("使用Socket5代理失败");
            }
            return client;
        }

        public string Token { set; get; } = Guid.NewGuid().ToString();
        public P2PTcpClient ToClient { set; get; }
        public bool IsAuth { set; get; } = false;

        public static P2PTcpProxy Proxy { set; get; } = new P2PTcpProxy();

        String m_remoteEndPoint = "";
        String m_localEndPoint = "";
        public String RemoteEndPoint
        {
            get
            {
                return m_remoteEndPoint;
            }
        }
        public String LocalEndPoint
        {
            get
            {
                return m_localEndPoint;
            }
        }
    }
    public class P2PTcpProxy
    {
        /// <summary>
        ///     代理服务器ip
        /// </summary>
        public string IP { set; get; }
        /// <summary>
        ///     代理服务器port
        /// </summary>
        public int Port { set; get; }
        /// <summary>
        ///     密码
        /// </summary>
        public string Password { set; get; }
        /// <summary>
        ///     用户名
        /// </summary>
        public string UserName { set; get; }
        /// <summary>
        ///     代理服务器类型
        /// </summary>
        public string ProxyType { set; get; } = "http";
        /// <summary>
        ///     使用代理的地址集合
        /// </summary>
        public List<string> Address { set; get; } = new List<string>();
    }
}
