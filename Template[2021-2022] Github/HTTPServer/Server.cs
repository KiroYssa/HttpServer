using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        int port;
        Socket serverSocket;
        IPEndPoint iep;
        public Server(int portNumber, string redirectionMatrixPath)
        {
            this.port = portNumber;
            iep = new IPEndPoint(IPAddress.Any, this.port);
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(iep);
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            serverSocket.Listen(1000);

            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                Socket clientSocket = this.serverSocket.Accept();
                Console.WriteLine("Client Accepted");

                Thread newthread = new Thread(new ParameterizedThreadStart(HandleConnection));
                newthread.Start(clientSocket);
                //TODO: accept connections and start thread for each accepted connection.
                
            }
        }

        public void HandleConnection(object obj)
        {
            Console.WriteLine("Handle Conn");
            Socket clientSock = (Socket)obj;
            clientSock.ReceiveTimeout = 0;
            byte[] data;
            int receivedLength;

            // TODO: Create client socket 
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period

            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    data = new byte[1024];
                    receivedLength = clientSock.Receive(data);

                    // TODO: break the while loop if receivedLen==0
                    if (receivedLength == 0)
                    {
                        Console.WriteLine("Client: {0} ended the connection", clientSock.RemoteEndPoint);
                        break;
                    }

                    // TODO: Create a Request object using received request string
                    string DataStr = Encoding.ASCII.GetString(data, 0, receivedLength);
                    Request newRequest = new Request(DataStr);

                    // TODO: Call HandleRequest Method that returns the response
                    Response NewResponse = HandleRequest(newRequest);
                    string response = NewResponse.ResponseString;
                    
                    // TODO: Send Response back to client
                    byte[] Res = Encoding.ASCII.GetBytes(response);
                    clientSock.Send(Res);

                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                    // TODO: log exception using Logger class
                }
            }
            clientSock.Close();
            // TODO: close client socket
        }

        Response HandleRequest(Request request)
        {
            //throw new NotImplementedException();
            Console.WriteLine("Handle Request");
            Response NewRes;
            string content = "";
            string Path;
            StreamWriter writer = new StreamWriter(string.Empty);
            
            try
            {
                bool flag = request.ParseRequest();
                string redirect = request.relativeURI;
                //TODO: check for bad request 
                if (!flag)
                {
                    Path = string.Format(Configuration.RootPath + "\\" + Configuration.BadRequestDefaultPageName);
                    writer = new StreamWriter(string.Format(Path));
                    NewRes = new Response(StatusCode.BadRequest, "text/html", writer.ToString(), Path);
                    
                }
                else
                {
                    //TODO: map the relativeURI in request to get the physical path of the resource.
                    string newRedirectPath = GetRedirectionPagePathIFExist(redirect);
                    //TODO: check for redirect
                    if(newRedirectPath != string.Empty)
                    {
                        //First Sending the Redirection Default Page
                        Path = string.Format(Configuration.RootPath + "\\" + Configuration.RedirectionDefaultPageName);
                        writer = new StreamWriter(string.Format(Path));
                        NewRes = new Response(StatusCode.OK, "text/html", writer.ToString(), Path);

                        //Second Sending the redirected Path
                        Path = string.Format(Configuration.RootPath + "\\" + newRedirectPath);
                        writer = new StreamWriter(string.Format(Path));
                        NewRes = new Response(StatusCode.OK, "text/html", writer.ToString(), Path);
                    }
                    else
                    {
                        //TODO: check file exists
                        Path = string.Format(Configuration.RootPath + "\\" + redirect);

                        if (File.Exists(Path))
                        {
                            //TODO: read the physical file
                            writer = new StreamWriter(string.Format(Path));
                            // Create OK response
                            NewRes = new Response(StatusCode.OK, "text/html", writer.ToString(), Path);
                        }
                        else
                        {
                            // Create Not Found response
                            NewRes = new Response(StatusCode.NotFound, "text/html", writer.ToString(), Configuration.NotFoundDefaultPageName);
                        }

                    }

                }
                Console.WriteLine(writer.ToString());
                writer.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                // TODO: log exception using Logger class
                NewRes = new Response(StatusCode.InternalServerError, "text/html", content, Configuration.InternalErrorDefaultPageName);
                // TODO: in case of exception, return Internal Server Error. 
            }
            return NewRes;
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {


            //Read the first line of text
            BinaryWriter wr = new BinaryWriter(File.Open("redirectionRules.txt", FileMode.Open));

            //Continue to read until you reach end of file
            foreach (var Rule in Configuration.RedirectionRules)
            {
                if (relativePath.Equals(Rule.Key))
                {
                    return Rule.Value;
                }
            }




            //close the file
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty

            return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string

            if (!File.Exists(filePath))
            {
                Exception ex = new FileNotFoundException();
                Logger.LogException(ex);
            }
            else
            {

                //Response NewRes = new Response(StatusCode.OK, "text/html", "", defaultPageName);
            }
            // else read file and return its content
            return string.Empty;
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                StreamReader sr = new StreamReader(filePath);
                //Read the first line of text
                string line;


                //Continue to read until you reach end of file
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    string[] Arr = line.Split(',');
                    Configuration.RedirectionRules.Add(Arr[0], Arr[1]);
                }
                sr.Close();

                // TODO: using the filepath paramter read the redirection rules from file 
                // then fill Configuration.RedirectionRules dictionary 
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                // TODO: log exception using Logger class
                Environment.Exit(1);
            }
        }
    }
}
