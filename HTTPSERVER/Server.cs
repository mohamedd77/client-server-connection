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
        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint HostEndPoint = new IPEndPoint(IPAddress.Any, portNumber);
            serverSocket.Bind(HostEndPoint);
            Console.WriteLine("Listening........");
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            serverSocket.Listen(1000);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            
            while (true)
            {
                
                //TODO: accept connections and start thread for each accepted connection.
                Socket ClientSocket = serverSocket.Accept();
                Console.WriteLine("New Client Accepted : {0}", ClientSocket.RemoteEndPoint);
                Thread NewThread = new Thread(new ParameterizedThreadStart(HandleConnection));
                NewThread.Start(ClientSocket);
               // NewThread.Join();

            }   
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            Socket ClientSocket = (Socket)obj;
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            ClientSocket.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    byte[] Data = new byte[1024];
                    int ReceivedLen = ClientSocket.Receive(Data);
                    // TODO: break the while loop if receivedLen==0
                    if(ReceivedLen == 0)
                    {
                        Console.WriteLine("Client : {0} ended the connection ", ClientSocket.RemoteEndPoint);
                        break;
                    }
                    // TODO: Create a Request object using received request string
                    Request data = new Request(Encoding.ASCII.GetString(Data));
                    // TODO: Call HandleRequest Method that returns the response
                    Response response = HandleRequest(data);
                    // TODO: Send Response back to client
                    ClientSocket.Send(Encoding.ASCII.GetBytes(response.ResponseString));
                   

                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
                

            }
            // TODO: close client socket
            ClientSocket.Close();
        }
         
        Response HandleRequest(Request request)
        {
            try
            {
                //TODO: check for bad request 
               
                if (!request.ParseRequest())
                {
                    string ldp = LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    Response RSP = new Response(StatusCode.BadRequest, ldp, "400 Bad Request","");
                    return RSP;
                }
                //TODO: map the relativeURI in request to get the physical path of the resource
                String content = LoadDefaultPage(request.relativeURI);
                //TODO: check for redirect
                String grp = GetRedirectionPagePathIFExist(request.relativeURI);
                if (grp != "")
                {
                    string gldp = LoadDefaultPage('/'+grp);
                    Response RSP = new Response(StatusCode.Redirect, "text/html", gldp, GetRedirectionPagePathIFExist(request.relativeURI));
                    return RSP;
                }
                //TODO: check file exists
                if (content == "")
                {
                    string nf = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                    Response RSP = new Response(StatusCode.NotFound, "text/html", nf, "");
                    return RSP;
                }
                //TODO: read the physical file
                // Create OK response
                Response RS = new Response(StatusCode.OK , "text/html", content, "");
                return RS;
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                // TODO: in case of exception,
                Logger.LogException(ex);
                string iedp = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);
                Response RSP = new Response(StatusCode.InternalServerError, "text/html", iedp, "");
                return RSP;
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            string[] rps = relativePath.Split('/');
            foreach (var transfer in Configuration.RedirectionRules)
            {
                if (transfer.Key == rps[1])
                {
                    return transfer.Value;
                }
            }
            return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            string filePath = Configuration.RootPath + defaultPageName ;
           
            if (!File.Exists(filePath))
            {
                Exception ex = new Exception(Configuration.NotFoundDefaultPageName);
                Logger.LogException(ex);
                return string.Empty;
            }
            // else read file and return its content
            else
            {
                string fp = File.ReadAllText(filePath);
                return fp;
            }
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                string sf = File.ReadAllText(filePath);
                string[] word = sf.Split(',');
                // then fill Configuration.RedirectionRules dictionary 
                Configuration.RedirectionRules.Add(word[0], word[1]);
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
