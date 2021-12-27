using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: Call CreateRedirectionRulesFile() function to create the rules of redirection 
            CreateRedirectionRulesFile();
            Server NewServer = new Server(1000, "redirectionRules.txt");
            Console.WriteLine("Server Object Created");
            NewServer.StartServer();
            Console.WriteLine("Server Started");
            Console.ReadLine();
            //Start server
            // 1) Make server object on port 1000
            // 2) Start Server

        }

        static void CreateRedirectionRulesFile()
        {
            StreamWriter wr = new StreamWriter("redirectionRules.txt");
            wr.Write("aboutus.html,aboutus2.html");
            wr.Close();

            // TODO: Create file named redirectionRules.txt
            // each line in the file specify a redirection rule
            // example: "aboutus.html,aboutus2.html"
            // means that when making request to aboustus.html,, it redirects me to aboutus2
        }

    }
}
