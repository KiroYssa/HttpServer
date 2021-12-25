using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            //throw new NotImplementedException();
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            headerLines.Add(GetStatusLine(code));
            headerLines.Add("Content-type: " + "text\\html");
            headerLines.Add("Content-Length: " + content.Length);
            headerLines.Add("Location: " + redirectoinPath);

            // TODO: Create the request string
            for(int i = 0; i < 0; i++)
            {
                responseString += headerLines[i] + "\r\n";
            }
            
        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = string.Empty;
            statusLine = Configuration.ServerHTTPVersion + " ";
            if(code.Equals(200))
            {
                statusLine += "200 OK";
            }
            else if (code.Equals(500))
            {
                statusLine += "500 InternalServerError";
            }
            else if (code.Equals(404))
            {
                statusLine += "404 NotFound";
            }
            else if (code.Equals(400))
            {
                statusLine += "400 BadRequest";
            }
            else if (code.Equals(301))
            {
                statusLine += "301 Redirect";
            }
            return statusLine;
        }
    }
}
