using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Multi_Server
{
    class Program
    {
        private static Socket _serverSocket;
        private static readonly List<Socket> _clientSockets = new List<Socket>();
        private const int _BUFFER_SIZE = 2048;
        private const int _PORT = 100;
        private static readonly byte[] _buffer = new byte[_BUFFER_SIZE];

        static void Main()
        {
            Console.Title = "Server";
            SetupServer();
           while(true)
           {
               Console.WriteLine("\n 1- Press 1-6 to choose socket \n 2- Type and enter to send text \n 3- quit to reset!");
               //string y = Console.WriteLine("1-Unicast (1 to 6 to send message stream to Socket#)\n 2- BroadCast on all Client Sockets");
               string x = Console.ReadLine(); // When we press enter close everything
               if(x == "exit")
                    CloseAllSockets();
               else if(x == "1")
               {
                   while(true)
                   {
                       string input = Console.ReadLine();
                       if (input != "quit")
                       {
                           Console.WriteLine("Sending Message to:\n",x);
                           Send(_clientSockets[Convert.ToInt32(x)-1], input);
                       }
                       else
                       {
                          break;
                       }
                   }
               }
               else if (x == "2")
               {
                   string input = Console.ReadLine();
                   if (input != "quit")
                   {
                       Console.WriteLine("Sending Message to:\n", x);
                       Send(_clientSockets[Convert.ToInt32(x) - 1], input);
                   }
                   else
                   {
                       break;
                   }
               }
               else if (x == "3")
               {
                   string input = Console.ReadLine();
                   if (input != "quit")
                   {
                       Console.WriteLine("Sending Message to:\n", x);
                       Send(_clientSockets[Convert.ToInt32(x) - 1], input);
                   }
                   else
                   {
                       break;
                   }
               }
               else if (x == "4")
               {
                   string input = Console.ReadLine();
                   if (input != "quit")
                   {
                       Console.WriteLine("Sending Message to:\n", x);
                       Send(_clientSockets[Convert.ToInt32(x) - 1], input);
                   }
                   else
                   {
                       break;
                   }
               }
               else if (x == "5")
               {
                   string input = Console.ReadLine();
                   if (input != "quit")
                   {
                       Console.WriteLine("Sending Message to:\n", x);
                       Send(_clientSockets[Convert.ToInt32(x) - 1], input);
                   }
                   else
                   {
                       break;
                   }
               }
               else if (x == "6")
               {
                   string input = Console.ReadLine();
                   if (input != "quit")
                   {
                       Console.WriteLine("Sending Message to:\n", x);
                       Send(_clientSockets[Convert.ToInt32(x) - 1], input);
                   }
                   else
                   {
                       break;
                   }
               }
           }
            
        }

        private static void SetupServer()
        {
            Console.WriteLine("Setting up server...");
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, _PORT));
            _serverSocket.Listen(100);
            _serverSocket.BeginAccept(AcceptCallback, null);
            Console.WriteLine("Server setup complete");
        }

        /// <summary>
        /// Close all connected client (we do not need to shutdown the server socket as its connections
        /// are already closed with the clients)
        /// </summary>
        private static void CloseAllSockets()
        {
            foreach (Socket socket in _clientSockets)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }

            _serverSocket.Close();
        }

        private static void AcceptCallback(IAsyncResult AR)
        {
            Socket socket;

            try
            {
                socket = _serverSocket.EndAccept(AR);
            }
            catch (ObjectDisposedException) // I cannot seem to avoid this (on exit when properly closing sockets)
            {
                return;
            }

           _clientSockets.Add(socket);
           //socket.BeginReceive(_buffer, 0, _BUFFER_SIZE, SocketFlags.None, ReceiveCallback, socket);
           Console.WriteLine("Client connected, waiting for request...");
           _serverSocket.BeginAccept(AcceptCallback, null);
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
            Console.WriteLine("Received Text: " + text);

            if (text.ToLower() == "get time") // Client requested time
            {
                Console.WriteLine("Text is a get time request");
                byte[] data = Encoding.ASCII.GetBytes(DateTime.Now.ToLongTimeString());
                current.Send(data);
                Console.WriteLine("Time sent to client");
            }
            else if (text.ToLower() == "exit") // Client wants to exit gracefully
            {
                // Always Shutdown before closing
                current.Shutdown(SocketShutdown.Both);
                current.Close();
                _clientSockets.Remove(current);
                Console.WriteLine("Client disconnected");
                return;
            }
            else
            {
                Console.WriteLine("Text is an invalid request");
                byte[] data = Encoding.ASCII.GetBytes("Invalid request");
                current.Send(data);
                Console.WriteLine("Warning Sent");
            }
            current.BeginReceive(_buffer, 0, _BUFFER_SIZE, SocketFlags.None, ReceiveCallback, current);
        }

        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                //handler.Shutdown(SocketShutdown.Both);
                //handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
