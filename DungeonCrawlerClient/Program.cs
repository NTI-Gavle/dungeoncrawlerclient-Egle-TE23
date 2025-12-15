using System.Net.Sockets;
using System.Net;
using System.Text;

namespace DungeonCrawlerClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IPAddress iPAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, 57575);
            TcpClient tcpClient = new TcpClient();
            string message = "";

            while (!tcpClient.Connected)
            {
                try
                {
                    tcpClient.Connect(iPEndPoint);
                    Console.WriteLine("connected to server");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not connect to server.\nPress ENTER to try again");
                    Console.ReadKey();
                }
            }

            bool taskRunning = false;
            while (tcpClient.Connected)
            {
                try
                {
                    if (!taskRunning)
                    {
                        Task.Run(() =>
                        {
                            while (true)
                            {
                                byte[] readBytes = new byte[1024];
                                tcpClient.GetStream().Read(readBytes, 0, readBytes.Length);
                                message = Encoding.UTF8.GetString(readBytes);
                                Console.WriteLine(message);
                                if (message.Contains("exiting game"))
                                {
                                    tcpClient.Close();
                                    Environment.Exit(0);
                                }
                            }
                        });
                        taskRunning = true;
                    }
                    string command = Console.ReadLine();
                    byte[] writeBytes = Encoding.UTF8.GetBytes(command);
                    tcpClient.GetStream().Write(writeBytes, 0, writeBytes.Length);
                }
                catch (Exception)
                {
                    Console.WriteLine("Disconnected from server\nPress ENTER to try to reconnect");
                    Console.ReadKey();
                    while (!tcpClient.Connected)
                    {
                        try
                        {
                            tcpClient = new TcpClient();
                            tcpClient.Connect(iPEndPoint);
                            Console.WriteLine("connected to server");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Could not connect to server.\nPress ENTER to try again");
                            Console.ReadKey();
                        }
                    }
                }
            }
        }
    }
}