using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.Timers;

namespace TestSocketServer
{
    class Program
    {
        private const int SEND_DATA_INTERVAL = 200;

        private static IPEndPoint localEndPoint;
        private static int portID = 8001;

        private static Timer sendDataTimer;

        private static Socket mySocket;
        private static Socket clientConnected;

        #region 获取本机IP
        public static IPAddress GetLocalIP()
        {
            try
            {
                //得到主机名
                string localHostName = Dns.GetHostName();
                IPHostEntry localIPEntry = Dns.GetHostEntry(localHostName);

                for (int i = 0; i < localIPEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4
                    //AddressFamily.InterNetworkV6表示此IP为IPv6
                    //Console.Write(localIPEntry.AddressList[i].ToString());
                    if (localIPEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        return localIPEntry.AddressList[i];
                    }
                }
                Console.WriteLine("not found!!");
                return IPAddress.Any;
            }
            catch (Exception ex)
            {
                Console.WriteLine("获取本机IP出错！错误信息：" + ex.Message);
                return IPAddress.Any;
            }
        }
        #endregion

        #region timer相关
        public static void InitializingTimers()
        {
            sendDataTimer = new Timer(SEND_DATA_INTERVAL);
            sendDataTimer.Elapsed += SendDataTimer_Elapsed;
            sendDataTimer.Enabled = true;
            sendDataTimer.Start();
        }

        public static void SendDataTimer_Elapsed(Object source, ElapsedEventArgs e)
        {
            if (clientConnected != null && clientConnected.Connected)
            {
                try
                {
                    clientConnected.Send(Encoding.Unicode.GetBytes("Message from server at " + DateTime.Now.ToString()));
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex.Message);
                    sendDataTimer.Stop();
                }
            }
            else
            {
                Console.WriteLine("The client is disconnect (Sign by WXY)");
                sendDataTimer.Stop();
            }
        }

        #endregion

        //接收到客户端请求后的事件（个人理解。。。
        public static void ReceivingClientRequest(IAsyncResult ar)
        {
            //客户端的socket
            clientConnected = mySocket.EndAccept(ar);
            //发送欢迎消息
            clientConnected.Send(Encoding.Unicode.GetBytes("Hi, I accept you request at " + DateTime.Now.ToString()));
            //启动定时器
            InitializingTimers();

            //伪·递归调用（不造会不会有问题。。。
            mySocket.BeginAccept(new AsyncCallback(ReceivingClientRequest), null);
        }

        public static bool SetupServer()
        {
            try
            {
                mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                //设置本地终结点
                localEndPoint = new IPEndPoint(GetLocalIP(), portID);

                //将socket绑定到主机上面的某个端口
                mySocket.Bind(localEndPoint);

                //自动监听，队列长度暂定为4（我也不知什么概念
                mySocket.Listen(4);

                //开始接受客户端连接请求
                mySocket.BeginAccept(new AsyncCallback(ReceivingClientRequest), null);

                Console.WriteLine("server is ready~");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("创建服务器失败，信息：" + ex.Message);
                return false;
            }
        }

        static void Main(string[] args)
        {
            SetupServer();

            Console.Read();
        }
    }
}
