using System;
using System.Net.Sockets;

namespace WebProxy
{
    /// <summary>
    /// Creates a tunnel between 2 TCP clients, allowing for data to be sent from the entry to the exit indefinitely
    /// </summary>
    class TCPTunnel
    {
        /// <summary>
        /// Entry and exit points of the connection
        /// </summary>
        TcpClient entryClient;
        TcpClient exitClient;

        /// <summary>
        /// Data streams for the entry and exit points
        /// </summary>
        private NetworkStream entryStream;
        private NetworkStream exitStream;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="entryClient">Entry point client</param>
        /// <param name="exitClient">Exit point client</param>
        public TCPTunnel(TcpClient entryClient, TcpClient exitClient)
        {
            this.entryClient = entryClient;
            this.exitClient = exitClient;

            this.entryStream = entryClient.GetStream();
            this.exitStream = exitClient.GetStream();
        }


        /// <summary>
        /// Start tunneling the data from the entry point to the exit point
        /// </summary>
        /// <param name="timeout">Timeout length for the data streams</param>
        public void StartTunnel(int timeout = 10000)
        {
            byte[] buffer = new byte[1024];
            int read = 0;
            int emptyRead = 0;
            try
            {
                //Set the timeouts
                entryStream.ReadTimeout = timeout;
                entryStream.WriteTimeout = timeout;
                exitStream.ReadTimeout = timeout;
                exitStream.WriteTimeout = timeout;
                entryClient.ReceiveTimeout = timeout;
                entryClient.SendTimeout = timeout;
                exitClient.ReceiveTimeout = timeout;
                exitClient.SendTimeout = timeout;

                //While the clients are both connected, send data from the entry point to the exit point
                //Break out of the loop of when get 100 empty reads in a row, indicating that there is no more data left
                while (entryClient.Connected && exitClient.Connected && emptyRead < 100)
                {
                    if ((read = entryStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        emptyRead = 0;
                        exitStream.Write(buffer, 0, read);
                    }
                    else if (read == 0)
                    {
                        emptyRead++;
                    }
                }
            }
            catch (System.ObjectDisposedException ode)
            {
                Console.WriteLine("Exception: trying to access disposed object\n{0}\n", ode);
            }
            catch (System.IO.IOException ioe)
            {
                Console.WriteLine("Exception: IOException, most likely due to a timeout, and expected\n{0}\n", ioe);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception while tunneling\n{0}\n", e);
            }
            finally
            {
                CloseTunnel();
            }
        }

        /// <summary>
        /// Close the tunnel
        /// </summary>
        public void CloseTunnel()
        {
            try
            {
                if (entryClient.Connected)
                {
                    entryClient.GetStream().Close();
                    entryClient.Close();
                }
                if (exitClient.Connected)
                {
                    exitClient.GetStream().Close();
                    exitClient.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception while closing connections:\n{0}\n", e);
            }
        }
    }
}
