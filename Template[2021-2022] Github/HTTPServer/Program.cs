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
            Server NewServer = new Server(1000, "");
            NewServer.StartServer();
            //Start server
            // 1) Make server object on port 1000
            // 2) Start Server
        }

        static void CreateRedirectionRulesFile()
        {
            BinaryWriter wr = new BinaryWriter(File.Open("redirectionRules.txt", FileMode.Create));
            wr.Write("aboutus.html,aboutus2.html");
            wr.Write("main.html,main.html");


            // TODO: Create file named redirectionRules.txt
            // each line in the file specify a redirection rule
            // example: "aboutus.html,aboutus2.html"
            // means that when making request to aboustus.html,, it redirects me to aboutus2
        }

    }
}
