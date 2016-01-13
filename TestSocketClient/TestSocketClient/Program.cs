using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

namespace TestSocketClient
{
    class Program
    {
        private static IPEndPoint remoteIPEndPoint;
        private static int portID = 8001;

        private static Socket myClientSocket;

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

        public static bool SetupClient()
        {
            try
            {
                myClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                //设置目标服务器终结点
                remoteIPEndPoint = new IPEndPoint(GetLocalIP(), portID);

                //将socket连接到server的端口
                myClientSocket.Connect(remoteIPEndPoint);

                Console.WriteLine("client success to connect to the server~");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("连接服务器失败，信息：" + ex.Message);
                return false;
            }
        }

        static void Main(string[] args)
        {
            SetupClient();
            Console.Read();
        }
    }
}
