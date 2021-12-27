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
            headerLines.Add("Content-type: "+contentType);
            headerLines.Add("Content-Length: " + content.Length);
            headerLines.Add("Date: " + DateTime.Now.ToString("ddd, dd MMM yyy HH':'mm':'ss 'GMT'"));


            if (redirectoinPath.Length != 0)
            {
                headerLines.Add("Location: " + redirectoinPath);
            }
            // TODO: Create the request string
            foreach (var lines in headerLines)
            {
                responseString += lines + "\r\n";

            }

            responseString += "\r\n" + content;

        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = null;

            if (code == StatusCode.OK)
            {
                statusLine = Configuration.ServerHTTPVersion + " 200 OK";

            }
            else if (code == StatusCode.InternalServerError)
            {
                statusLine = Configuration.ServerHTTPVersion + " 500 InternalServerError";

            }
            else if (code == StatusCode.NotFound)
            {
                statusLine = Configuration.ServerHTTPVersion + " 404 NotFound";

            }
            else if (code == StatusCode.BadRequest)
            {
                statusLine = Configuration.ServerHTTPVersion + " 400 BadRequest";

            }
            else if (code == StatusCode.Redirect)
            {
                statusLine = Configuration.ServerHTTPVersion + " 301 Redirect";

            }
            return statusLine;
        }
    }
}
