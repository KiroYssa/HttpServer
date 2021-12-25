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
        Socket serverSocket;
        IPEndPoint iep;
        public Server(int portNumber, string redirectionMatrixPath)
        {
             iep = new IPEndPoint( IPAddress.Any, portNumber);
            this.LoadRedirectionRules(redirectionMatrixPath);
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            //TODO: initialize this.serverSocket
        }

        public void StartServer()
        {
            serverSocket.Bind(iep);
            serverSocket.Listen(1000);
            // TODO: Listen to connections, with large backlog.

            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                Socket clientSocket = this.serverSocket.Accept();

                Thread newthread = new Thread(new ParameterizedThreadStart(HandleConnection));
                newthread.Start(clientSocket);
                //TODO: accept connections and start thread for each accepted connection.

            }
        }

        public void HandleConnection(object obj)
        {
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
                    data = new byte[1024];
                    receivedLength = clientSock.Receive(data);
                   
                    if (receivedLength == 0)
                    {
                        Console.WriteLine("Client: {0} ended the connection", clientSock.RemoteEndPoint);
                        break;
                    }

                    // TODO: Receive request

                    // TODO: break the while loop if receivedLen==0
                   string DataStr = Encoding.ASCII.GetString(data, 0, receivedLength);
                    Request newRequest = new Request(DataStr);
                    // TODO: Create a Request object using received request string
                   Response NewResponse= HandleRequest(newRequest);
                    string Response = NewResponse.ResponseString;
                    // TODO: Call HandleRequest Method that returns the response
                    byte[] Res = Encoding.ASCII.GetBytes(Response);
                    clientSock.Send(Res);
                    // TODO: Send Response back to client

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
            throw new NotImplementedException();
            Response NewRes;
            string content="";
            try
            {
              bool flag=request.ParseRequest();
                string redirect = request.relativeURI;
                //TODO: check for bad request 
                if (flag)
                {
                   string c= Configuration.BadRequestDefaultPageName;
                    NewRes = new Response(StatusCode.BadRequest, "text/html",content ,c);
                }
                //TODO: map the relativeURI in request to get the physical path of the resource.
                GetRedirectionPagePathIFExist(redirect);
                //TODO: check for redirect
                
                
                //TODO: check file exists

                //TODO: read the physical file

                // Create OK response
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                // TODO: log exception using Logger class
                NewRes = new Response(StatusCode.InternalServerError, "text/html", content, Configuration.InternalErrorDefaultPageName);
                // TODO: in case of exception, return Internal Server Error. 
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            
           
            //Read the first line of text


            //Continue to read until you reach end of file
                foreach(var Rule in Configuration.RedirectionRules)
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
                    Configuration.RedirectionRules.Add(Arr[0],Arr[1]);
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
