using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Collections.Generic;




namespace Multi_Client
{
    class Program
    {

        private static readonly List<Socket> _clientSockets = new List<Socket>();
        //private static readonly Socket _clientSocket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private const int _PORT = 100;
        private const int _BUFFER_SIZE = 2048;
        private static readonly byte[] _buffer = new byte[_BUFFER_SIZE];
        static void Main()
        {
            Console.Title = "Client";
            InitializeAndConnect();
            //RequestLoop();
            string x = Console.ReadLine();
            Exit();
        }

        private static void InitializeAndConnect()
        {
            Socket A = null, B=null, C=null, D=null, E=null, F=null;

            _clientSockets.Add(A);
            _clientSockets.Add(B);
            _clientSockets.Add(C);
            _clientSockets.Add(D);
            _clientSockets.Add(E);
            _clientSockets.Add(F);

            int index = _clientSockets.Count;
            while(index !=0)
            { 
                index = index -1;
                ConnectToServer(index);
            }
        }

        private static void ConnectToServer(int sockNumber)
        {
            int attempts = 0;
                _clientSockets[sockNumber] = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            while (!_clientSockets[sockNumber].Connected)
            {
                try
                {
                    attempts++;
                    Console.WriteLine("Connection attempt" + attempts);
                    _clientSockets[sockNumber].Connect(IPAddress.Loopback, _PORT);
                    _clientSockets[sockNumber].BeginReceive(_buffer, 0, _BUFFER_SIZE, SocketFlags.None, ReceiveCallback, _clientSockets[sockNumber]);
                }
                catch (SocketException) 
                {
                    Console.Clear();
                }
            }

            Console.Clear();
            Console.WriteLine("ConnectedS ");
        }

        private static void ReceiveCallback(IAsyncResult AR)
        {
            Socket current = (Socket)AR.AsyncState;
            int received;

            try
            {
                received = current.EndReceive(AR);
            }
            catch (SocketException)
            {
                Console.WriteLine("Client forcefully disconnected");
                current.Close(); // Dont shutdown because the socket may be disposed and its disconnected anyway
                _clientSockets.Remove(current);
                return;
            }

            byte[] recBuf = new byte[received];
            Array.Copy(_buffer, recBuf, received);
            string text = Encoding.ASCII.GetString(recBuf);
            Console.WriteLine("Received Text at: " + text);

            //if (text.ToLower() == "get time") // Client requested time
            //{
            //    Console.WriteLine("Text is a get time request");
            //    byte[] data = Encoding.ASCII.GetBytes(DateTime.Now.ToLongTimeString());
            //    current.Send(data);
            //    Console.WriteLine("Time sent to client");
            //}
            //else if (text.ToLower() == "exit") // Client wants to exit gracefully
            //{
            //    // Always Shutdown before closing
            //    current.Shutdown(SocketShutdown.Both);
            //    current.Close();
            //    _clientSockets.Remove(current);
            //    Console.WriteLine("Client disconnected");
            //    return;
            //}
            //else
            //{
            //    Console.WriteLine("Text is an invalid request");
            //    byte[] data = Encoding.ASCII.GetBytes("Invalid request");
            //    current.Send(data);
            //    Console.WriteLine("Warning Sent");
            //}
            current.BeginReceive(_buffer, 0, _BUFFER_SIZE, SocketFlags.None, ReceiveCallback, current);
        }
        private static void RequestLoop()
        {
            Console.WriteLine(@"<Type ""exit"" to properly disconnect client>");
            //while (true)
            //{
            //    //SendRequest();
            //    //ReceiveResponse();
            //}
        }

        /// <summary>
        /// Close socket and exit app
        /// </summary>
        private static void Exit()
        {
            SendString("exit"); // Tell the server we re exiting
            for (int i = 0; i < _clientSockets.Count; i++)
            {
                _clientSockets[i].Shutdown(SocketShutdown.Both);
                _clientSockets[i].Close();               
            }
            Environment.Exit(0);
        }

        private static void SendRequest()
        {
            Console.Write("Send a request: ");
            string request = Console.ReadLine();
            SendString(request);

            if (request.ToLower() == "exit")
            {
                Exit();
            }
        }

        /// <summary>
        /// Sends a string to the server with ASCII encoding
        /// </summary>
        private static void SendString(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            _clientSockets[0].Send(buffer, 0, buffer.Length, SocketFlags.None);
        }

        private static void ReceiveResponse()
        {
            var buffer = new byte[2048];
            for (int i = 0; i < _clientSockets.Count; i++)
            {
                //int received = _clientSockets[i].Receive(buffer, SocketFlags.None);
                //if (received == 0) continue;
                //var data = new byte[received];
                //Array.Copy(buffer, data, received);
                //string text = Encoding.ASCII.GetString(data);
                //Console.WriteLine("Socket#"+i+"::"+text);
            }
        }
    }
}
