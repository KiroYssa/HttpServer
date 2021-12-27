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
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            iep = new IPEndPoint( ipAddress, portNumber);
            this.LoadRedirectionRules(redirectionMatrixPath);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            //TODO: initialize this.serverSocket
            serverSocket.Bind(iep);
        }

        public void StartServer()
        {
           
            serverSocket.Listen(1000);
            // TODO: Listen to connections, with large backlog.
        
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                Socket clientSocket = this.serverSocket.Accept();
                Console.WriteLine("New client accepted: {0}", 	clientSocket.RemoteEndPoint);

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
                    Console.WriteLine("Response is Sent");
                    
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
            //throw new NotImplementedException();
            Response NewRes;
            string content="";

            try
            {
                 bool flag=request.ParseRequest();
                string HTMLcontent;
                string redirect = request.relativeURI;

                //TODO: check for bad request 
                if (!flag)
                {
                    HTMLcontent = GetPhysicalFile(Configuration.BadRequestDefaultPageName);
                    NewRes = new Response(StatusCode.BadRequest, "text/html", HTMLcontent, "");

                }
                else
                {
                   
                    //TODO: map the relativeURI in request to get the physical path of the resource.
                    string NotFoundTemp = string.Format(Configuration.RootPath + "\\" + redirect.Trim('/'));
                    Console.WriteLine(NotFoundTemp);
                    //TODO: check for redirect
                    if (!File.Exists(NotFoundTemp))
                    {

                        Console.WriteLine("Not Found ");

                        HTMLcontent = GetPhysicalFile(Configuration.NotFoundDefaultPageName);
                        Console.WriteLine(HTMLcontent);
                        NewRes = new Response(StatusCode.NotFound, "text/html", HTMLcontent, "");

                    }
                    else
                    {
                        string RedirectedPage = GetRedirectionPagePathIFExist(redirect.Trim('/'));
                        Console.WriteLine(RedirectedPage+120);
                        if (!RedirectedPage.Equals(string.Empty))
                        {
                            Console.WriteLine("In Redirection");
                            HTMLcontent = GetPhysicalFile(Configuration.RedirectionDefaultPageName);
                            Console.WriteLine(HTMLcontent);
                            NewRes = new Response(StatusCode.Redirect, "text/html", HTMLcontent, RedirectedPage);
                          
                            
                        }
                        else
                        {
                            HTMLcontent = GetPhysicalFile(redirect);
                            NewRes = new Response(StatusCode.OK, "text/html", HTMLcontent, "");

                        }
                    }
                }

               

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
            return NewRes;
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            
           
            //Read the first line of text


            //Continue to read until you reach end of file
                foreach(var Rule in Configuration.RedirectionRules)
            {
                Console.WriteLine(Rule.Key);

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

        private string GetPhysicalFile(string relativeURI)
        {

            string PathFile = string.Format(Configuration.RootPath + "\\" + relativeURI);
            string data;
            // Console.WriteLine(PathFile);
            using (WebClient web1 = new WebClient())
                data = web1.DownloadString(PathFile);//main.html
            return data;
        }
    }
}
