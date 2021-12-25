using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines = new Dictionary<string, string>();

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }


        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        /// 
        int count = 0;
        public bool ParseRequest()
        {

            //throw new NotImplementedException();

            //TODO: parse the receivedRequest using the \r\n delimeter   


            if (requestString.Contains("\r\n"))
            {
                requestLines = requestString.Split(new[] { "\r\n" }, 2, StringSplitOptions.None);
                //splits request string into request line and headers

            }
            count = requestLines.Length;
            // Validate blank line exists
            ValidateBlankLine();

            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)

            if (count == 3)
            {
                // Parse Request line
                ParseRequestLine(); // requestLines[0]
                // Load header lines into HeaderLines dictionary
                if (ParseRequestLine() == true)
                    LoadHeaderLines(); //request Lines [1]


               
                return true;
            }
            else
            {
                return false;
            }




        }

        private bool ParseRequestLine()
        {
            //throw new NotImplementedException();
            // requestlines [0] has the request Line 
            string[] tempString = requestLines[0].Split(' ');
            if (tempString.Length == 3)
            {
                // make sure request line has 3 things(method, uri, version) 

                method = RequestMethod.GET;
                //since our program only deals with GET
                relativeURI = tempString[1];
                //uri is the second string in the array after spliting 

                if (ValidateIsURI(relativeURI) == true) // to validate URI is okay to continue 
                {
                    string[] HTTPVerTemp = tempString[2].Split('/');

                    // splitting the version to define which version it is and to make sure its HTTP 
                    if (HTTPVerTemp[0] == "HTTP")
                    {
                        // for each case of versions
                        if (HTTPVerTemp[1].Contains("0.9"))
                        {
                            httpVersion = HTTPVersion.HTTP09;
                            return true;
                        }
                        else if (HTTPVerTemp[1].Contains("1.0"))
                        {
                            httpVersion = HTTPVersion.HTTP10;
                            return true;
                        }
                        else if (HTTPVerTemp[1].Contains("1.1"))
                        {
                            httpVersion = HTTPVersion.HTTP11;
                            return true;
                        }
                        else
                        {
                            return false;
                            // if its not any of the above versions 
                        }
                    }
                    else
                        return false;
                    // if its not HTTP
                }
                else
                    return false;
                // if URI is false
            }
            else
                return false;
            // if the request line doesnt have all the valid things 

        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {

            // throw new NotImplementedException();

            string[] temp = requestLines[1].Split(new[] { "\r\n\r\n" }, StringSplitOptions.None);

            string[] tempheaderlines = temp[0].Split(new[] { "\r\n" }, StringSplitOptions.None);
            string[] temp3;
            for (int i = 0; i < tempheaderlines.Length; i++)
            {

                temp3 = tempheaderlines[i].Split(new[] { ':' }, 2, StringSplitOptions.None);
                headerLines.Add(temp3[0], temp3[1]);
            }
            return true;
        }

        private bool ValidateBlankLine()
        {
            //  throw new NotImplementedException();
            if (requestString.Contains("\r\n\r\n"))
            {
                // makes sure blank line exists 
                count++;
                return true;
            }
            else
                return false;

        }

    }
}
