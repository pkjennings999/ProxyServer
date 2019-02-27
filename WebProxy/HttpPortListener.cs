using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace WebProxy
{
    /// <summary>
    /// Listens for connection requests and deals with them
    /// </summary>
    class HttpPortListener
    {
        /// <summary>
        /// Port and address to listen on
        /// </summary>
        public Int32 port { get; }
        public string ipAddress { get; }

        /// <summary>
        /// Cache for HTTP requests
        /// </summary>
        public Cache cache;

        /// <summary>
        /// List of blocked hosts
        /// </summary>
        public static HashSet<string> blockedHosts = new HashSet<string>();


        /// <summary>
        /// Data about the cache performance
        /// </summary>
        int totalGetRequests = 0;
        int totalBytesFromServer = 0;
        int totalBytesFromCache = 0;
        long totalTime = 0;
        Stopwatch stopwatch = new Stopwatch();


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="port">Port to listen on</param>
        /// <param name="ipAddress">IP address to listen on</param>
        /// <param name="cacheSize">Max amount of entries the cache can hold</param>
        public HttpPortListener(Int32 port, string ipAddress, int cacheSize = 100)
        {
            this.port = port;
            this.ipAddress = ipAddress;
            this.cache = new Cache(cacheSize);
        }


        /// <summary>
        /// Listens for connection requests
        /// </summary>
        /// <param name="managementConsole">Console for displaying cache performance data and blocked URLs</param>
        public void PortListener(ManagementConsole managementConsole = null)
        {
            //Display cache size on the console
            managementConsole.UpdateCacheSizeTextBox(cache.size.ToString());
            TcpListener server = null;
            try
            {
                Int32 port = this.port;
                IPAddress localAddr = IPAddress.Parse(this.ipAddress);

                server = new TcpListener(localAddr, port);

                server.Start();

                //Whenever a request is made to the specified address and port, open up a TCP client and deal with it in a new thread
                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    Thread requestThread = new Thread(() => RedirectRequests(client, managementConsole));
                    requestThread.SetApartmentState(ApartmentState.STA);
                    requestThread.Start();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException:\n{0}\n", e);
            }
            finally
            {
                server.Stop();
            }
        }


        /// <summary>
        /// Redirect the requests to the appropriate places, be it by sending the data to its destination,
        /// tunneling the client and server, or blocking the request
        /// </summary>
        /// <param name="entryClient">Entry point TCP client</param>
        /// <param name="managementConsole">Console for displaying cache performance data and blocked urls</param>
        private void RedirectRequests(TcpClient entryClient, ManagementConsole managementConsole)
        {
            //Create the objects for exit point TCP client, data streams, TCP tunnels and threads
            string exitClientAddress = "";
            TcpClient exitClient = null;

            NetworkStream entryStream = null;
            NetworkStream exitStream = null;

            StreamReader entryStreamReader = null;
            StreamReader exitStreamReader = null;

            StreamWriter entryStreamWriter = null;

            TCPTunnel tunnelEntryExit = null;
            TCPTunnel tunnelExitEntry = null;

            Thread clientThread = null;
            Thread serverThread = null;
            try
            {
                string data = "";

                // Maximum HTTP request for almost all browsers
                Byte[] bytes = new Byte[8192];

                //Get the data streams for the entry point TCP client
                entryStream = entryClient.GetStream();
                entryStreamReader = new StreamReader(entryStream);
                entryStreamWriter = new StreamWriter(entryStream);
                entryStream.ReadTimeout = 10000;

                //Read the initial request from the entry point
                string tempRead = null;
                while (entryStreamReader.Peek() != -1 && !entryStreamReader.EndOfStream)
                {
                    while (!(tempRead = entryStreamReader.ReadLine()).Equals(""))
                    {
                        data += tempRead;
                        data += "\r\n";
                    }
                    data += "\r\n\r\n";
                }

                //Parse info on connection type and host
                string connectionType = Regex.Match(data, "[A-Z]+").Value;
                string host = Regex.Match(data, "Host: (.*)\r\n").Groups[1].Value;


                //Print the request onto the console
                managementConsole.UpdateRequestsTextBox(Regex.Match(data, ".*\r\n").Value);

                //Used as a read counter
                int i = 0;
                //Check if the host is blocked on the proxy
                if (!blockedHosts.Contains(host) &&
                    !blockedHosts.Contains(Regex.Replace(host, "http://", "")) &&
                    !blockedHosts.Contains(Regex.Replace(host, "https://", "")) &&
                    !blockedHosts.Contains(Regex.Replace(host, "http://www.", "")) &&
                    !blockedHosts.Contains(Regex.Replace(host, "https://www.", "")) &&
                    !blockedHosts.Contains(Regex.Replace(host, "www.", "")) &&
                    !blockedHosts.Contains(Regex.Replace(host, ":443", "")))
                {
                    switch (connectionType)
                    {
                        //If there is no valid connection type, close the TCP client
                        case "":
                            entryStreamReader.Close();
                            entryStreamWriter.Close();
                            entryStream.Close();
                            entryClient.Close();
                            break;
                        case "GET":
                            do
                            {
                                //Setup to gather timing data
                                stopwatch.Reset();
                                stopwatch.Start();
                                totalGetRequests++;

                                //If we loop around for a second time, we will need to get the string value of the data
                                if (i > 0)
                                {
                                    data = Encoding.ASCII.GetString(bytes, 0, i);
                                }

                                //Get the address of the destination of the data
                                exitClientAddress = Regex.Match(data, "Host: (.*)\r\n").Groups[1].Value;

                                //The URL of the request is used as the key in the cache
                                string cacheKey = Regex.Match(data, "([A-Z]+.*) HTTP").Groups[1].Value;
                                byte[] cachedBytes = null;

                                //Remove '/' at the end of the address
                                if (exitClientAddress[exitClientAddress.Length - 1].Equals('/'))
                                {
                                    exitClientAddress = exitClientAddress.Remove(exitClientAddress.Length - 1);
                                }

                                //Create the exit point TCP client and data streams
                                exitClient = new TcpClient(exitClientAddress, 80);
                                exitStream = exitClient.GetStream();
                                exitStreamReader = new StreamReader(exitStream);
                                exitStream.ReadTimeout = 1000;

                                //If there already exists data in the cache for this request, we do this section
                                if ((cachedBytes = cache.GetFromCache(cacheKey)) != null)
                                {
                                    string cachedString = Encoding.ASCII.GetString(cachedBytes);
                                    //Grab the last modified header
                                    string lastModified = Regex.Match(cachedString, "Last-Modified: (.*)\r\n").Groups[1].Value;

                                    //Construct a new request, using the If-Modified-Since header.
                                    //This will return 304 Not Modified header if it has not been modified since the specified date, saving data
                                    string firstLine = Regex.Match(data, ".*\r\n").Value;
                                    string temp = Regex.Replace(firstLine, "\\?", @"\?");
                                    temp = Regex.Replace(temp, "\\(", @"\(");
                                    temp = Regex.Replace(temp, "\\)", @"\)");
                                    string[] split = Regex.Split(data, temp);
                                    string newRequest = firstLine + "If-Modified-Since: " + lastModified + "\r\n" + split[1];
                                    exitStream.Write(Encoding.ASCII.GetBytes(newRequest), 0, Encoding.ASCII.GetBytes(newRequest).Length);

                                    bool firstRead = true;
                                    //Useful loop end condition. If the data has not been modified, we don't need to listen for more from the exit point client
                                    bool isNotModified = false;
                                    while (!isNotModified && (i = exitStream.Read(bytes, 0, bytes.Length)) > 0)
                                    {
                                        data = Encoding.ASCII.GetString(bytes, 0, i);
                                        //If we get this status code, we know the data in the cache is up to date
                                        if (Regex.IsMatch(data, "HTTP/1.1 304 Not Modified\r\n"))
                                        {
                                            //Grab the cache data and send it to the entry point client.
                                            //Also update the cache to say this data was recently used
                                            entryStream.Write(cachedBytes, 0, cachedBytes.Length);
                                            cache.MarkReused(cacheKey);
                                            totalBytesFromCache += cachedBytes.Length;
                                            isNotModified = true;
                                        }
                                        else
                                        {
                                            //Read on loop until no more data is coming in.
                                            //We need to know if the read is the first read or not as we have to update the cached version.
                                            //Due to the proxy being multi-threaded, it is possible for the cache data to be overwritten as we loop,
                                            //so it is necessary to keep track of the data as we go and enter the full amount back into the cache.
                                            //The cache will handle if the data is being updated, or a new entry due to being overwritten
                                            if (firstRead)
                                            {
                                                byte[] toStore = new byte[i];
                                                Array.Copy(bytes, 0, toStore, 0, i);
                                                cache.UpdateCacheEntry(cacheKey, toStore);
                                            }
                                            else
                                            {
                                                byte[] toStore = cache.GetFromCache(cacheKey);
                                                int originalLen = toStore.Length;
                                                Array.Resize<byte>(ref toStore, toStore.Length + i);
                                                Array.Copy(bytes, 0, toStore, originalLen, i);
                                                cache.UpdateCacheEntry(cacheKey, toStore);
                                            }

                                            entryStream.Write(bytes, 0, i);
                                        }
                                    }
                                    //Request is finished, so gather timing data
                                    stopwatch.Stop();
                                    totalTime += stopwatch.ElapsedMilliseconds;
                                    stopwatch.Reset();
                                }
                                //If there does not exit data in the cache for this request, do this
                                else
                                {
                                    //Write the request to the exit point client
                                    if (i > 0)
                                    {
                                        exitStream.Write(bytes, 0, i);
                                    }
                                    else
                                    {
                                        exitStream.Write(Encoding.ASCII.GetBytes(data), 0, Encoding.ASCII.GetBytes(data).Length);
                                    }

                                    bool firstRead = true;
                                    byte[] totalBytes = null;
                                    //Read the response on loop
                                    //We need to know if the read is the first read or not as we have to update the cached version.
                                    //Due to the proxy being multi-threaded, it is possible for the cache data to be overwritten as we loop,
                                    //so it is necessary to keep track of the data as we go and enter the full amount back into the cache.
                                    //The cache will handle if the data is being updated, or a new entry due to being overwritten
                                    while ((i = exitStream.Read(bytes, 0, bytes.Length)) > 0)
                                    {
                                        totalBytesFromServer += i;
                                        if (firstRead)
                                        {
                                            byte[] toStore = new byte[i];
                                            Array.Copy(bytes, 0, toStore, 0, i);
                                            cache.InsertIntoCache(cacheKey, toStore);
                                            firstRead = false;
                                            totalBytes = toStore;
                                        }
                                        else
                                        {
                                            int originalLen = totalBytes.Length;
                                            Array.Resize<byte>(ref totalBytes, totalBytes.Length + i);
                                            Array.Copy(bytes, 0, totalBytes, originalLen, i);
                                            string temp55 = Encoding.ASCII.GetString(totalBytes);
                                            cache.UpdateCacheEntry(cacheKey, totalBytes);
                                        }
                                        entryStream.Write(bytes, 0, i);
                                    }
                                    stopwatch.Stop();
                                    totalTime += stopwatch.ElapsedMilliseconds;
                                    stopwatch.Reset();
                                }
                                //Request is finished, so gather timing data
                                exitStreamReader.Close();
                                exitStream.Close();
                                exitClient.Close();
                            }
                            while (entryStreamReader.Peek() != -1 && !entryStreamReader.EndOfStream && (i = entryStream.Read(bytes, 0, bytes.Length)) != 0);

                            //Once there is no more data coming from the entry point client, close it
                            entryStreamReader.Close();
                            entryStream.Close();
                            entryClient.Close();
                            break;

                        // For all other HTTP requests, do the following
                        case "POST":
                        case "PUT":
                        case "PATCH":
                        case "DELETE":
                        case "COPY":
                        case "HEAD":
                        case "OPTIONS":
                        case "LINK":
                        case "UNLINK":
                        case "PURGE":
                        case "LOCK":
                        case "UNLOCK":
                        case "PROPFIND":
                        case "VIEW":
                            do
                            {
                                //If we loop around for a second time, we will need to get the string value of the data
                                if (i > 0)
                                {
                                    data = Encoding.ASCII.GetString(bytes, 0, i);
                                }

                                //Get the address of the destination of the data
                                exitClientAddress = Regex.Match(data, "Host: (.*)\r\n").Groups[1].Value;

                                //Remove '/' at the end of the address
                                if (exitClientAddress[exitClientAddress.Length - 1].Equals('/'))
                                {
                                    exitClientAddress = exitClientAddress.Remove(exitClientAddress.Length - 1);
                                }

                                //Create the exit point TCP client and data streams
                                exitClient = new TcpClient(exitClientAddress, 80);
                                exitStream = exitClient.GetStream();
                                exitStreamReader = new StreamReader(exitStream);
                                exitStream.ReadTimeout = 1000;

                                //Write the request to the exit point client
                                if (i > 0)
                                {
                                    exitStream.Write(bytes, 0, i);
                                }
                                else
                                {
                                    exitStream.Write(Encoding.ASCII.GetBytes(data), 0, Encoding.ASCII.GetBytes(data).Length);
                                }

                                //Read the response on loop
                                while ((i = exitStream.Read(bytes, 0, bytes.Length)) > 0)
                                {
                                    entryStream.Write(bytes, 0, i);
                                }

                                //Once there is no more data coming from the exit point client, close it
                                exitStreamReader.Close();
                                exitStream.Close();
                                exitClient.Close();
                            }
                            while (entryStreamReader.Peek() != -1 && !entryStreamReader.EndOfStream && (i = entryStream.Read(bytes, 0, bytes.Length)) != 0);

                            //Once there is no more data coming from the entry point client, close it
                            entryStreamReader.Close();
                            entryStream.Close();
                            entryClient.Close();
                            break;

                        //For a https request:
                        case "CONNECT":
                            //Grab the exit client address and port
                            Match exitClientMatch = Regex.Match(data, "CONNECT (.*):(.*) HTTP");
                            exitClientAddress = exitClientMatch.Groups[1].Value;
                            int exitClientPort = Convert.ToInt32(exitClientMatch.Groups[2].Value);

                            //Connect to the exit point client
                            exitClient = new TcpClient(exitClientAddress, exitClientPort);

                            //Create tunnels for entry -> exit, and vice versa
                            //This will allow them to send data back and forth indefinitely
                            //We cannot read the data as it is encrypted, so all we can do is pass it on
                            //This also means we cannot cache it
                            tunnelEntryExit = new TCPTunnel(entryClient, exitClient);
                            tunnelExitEntry = new TCPTunnel(exitClient, entryClient);

                            entryStream = entryClient.GetStream();
                            entryStreamWriter = new StreamWriter(entryStream);

                            //Send the required connection established message
                            entryStreamWriter.WriteLine("HTTP/1.0 200 Connection established\r\n\r\n");
                            entryStreamWriter.Flush();

                            //Start the tunnels in separate threads
                            clientThread = new Thread(() => tunnelEntryExit.StartTunnel(50000));
                            serverThread = new Thread(() => tunnelExitEntry.StartTunnel(50000));

                            clientThread.Start();
                            serverThread.Start();
                            break;


                        default:
                            throw new NotSupportedException(connectionType + " requests not yet supported");
                    }
                }
                else
                {
                    //If the URL is blocked, send this message instead and close the connections
                    entryStreamWriter.WriteLine("HTTP/1.0 403 Forbidden\r\n\r\n<HTML><BODY><H1>no</H1></BODY></HTML>");
                    entryStreamWriter.Flush();

                    entryStreamWriter.Close();
                    entryStreamReader.Close();
                    entryStream.Close();
                    entryClient.Close();
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Exception while redirecting request\n{0}\n", e);
                if (tunnelEntryExit != null)
                {
                    tunnelEntryExit.CloseTunnel();
                }
                if (tunnelExitEntry != null)
                {
                    tunnelExitEntry.CloseTunnel();
                }
                if (entryClient != null)
                {
                    entryClient.Close();
                }
                if (exitClient != null)
                {
                    exitClient.Close();
                }
            }
            catch (System.Net.Sockets.SocketException se)
            {
                Console.WriteLine("Couldn't find host\n{0}\n", se);
                if (tunnelEntryExit != null)
                {
                    tunnelEntryExit.CloseTunnel();
                }
                if (tunnelExitEntry != null)
                {
                    tunnelExitEntry.CloseTunnel();
                }
                if (entryClient != null)
                {
                    entryClient.Close();
                }
                if (exitClient != null)
                {
                    exitClient.Close();
                }

            }
            finally
            {
                //Parse the gathered data and display it on the console
                if (stopwatch.IsRunning)
                {
                    stopwatch.Stop();
                    totalTime += stopwatch.ElapsedMilliseconds;
                    stopwatch.Reset();
                }
                managementConsole.UpdateRequestCountTextBox(totalGetRequests.ToString());
                managementConsole.UpdateBytesFromServerTextBox(totalBytesFromServer);
                managementConsole.UpdateBytesFromCacheTextBox(totalBytesFromCache);
                if (totalGetRequests > 0)
                {
                    managementConsole.UpdateAverageBytesTextBox((totalBytesFromServer / totalGetRequests));
                    managementConsole.UpdateCacheTimingTextBox((totalTime / totalGetRequests).ToString() + "ms");
                }
            }
        }


        /// <summary>
        /// Block a URL
        /// </summary>
        /// <param name="host">URL to be blocked</param>
        public static void BlockHost(string host)
        {
            blockedHosts.Add(host);
        }
    }
}