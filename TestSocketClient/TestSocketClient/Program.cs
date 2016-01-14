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
        private const int BUFFER_LENGTH = 1024;

        private static IPEndPoint remoteIPEndPoint;
        private static int portID = 8001;

        private static Socket myClientSocket;

        //接收到服务器消息后的事件（个人理解。。。
        public static void ReceivingServerMessage(IAsyncResult ar)
        {
            //接收到的消息长度？
            int messageLength = myClientSocket.EndReceive(ar);

            //接收到的信息内容（自己试的。。
            byte[] message = ar.AsyncState as byte[];

            //转换成string并print到控制台。。
            string s_message = Encoding.Unicode.GetString(message, 0, messageLength);
            Console.WriteLine(s_message);
        }

        #region 获取本机IP地址
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

        public static void StartReceivingData()
        {
            try
            {
                //设置一个缓冲区
                byte[] buffer = new byte[BUFFER_LENGTH];
                //开始接收信息
                myClientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceivingServerMessage, buffer);
            }
            catch (Exception ex)
            {
                Console.WriteLine("从服务器接收消息失败，信息：" + ex.Message);
            }
        }

        static void Main(string[] args)
        {
            SetupClient();
            StartReceivingData();
            Console.Read();
        }
    }
}
