﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        HTTP10, // HTTP/1.0
        HTTP11,// HTTP/1.1
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        public string relativeURI;
        Dictionary<string, string> headerLines= new Dictionary<string, string>();

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }
        string requestString;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there
        /// is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()

        {
            //TODO: parse the receivedRequest using the \r\n delimeter
            requestLines = requestString.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)

            if (requestLines.Length < 3)
            {
                return false;
            }
            // Parse Request line
            
            if (!ParseRequestLine())
            {
                return false;
            }
            // Validate blank line exists
            
            if (!ValidateBlankLine())
            {
                return false;
            }
            // Load header lines into HeaderLines dictionary
           
            if (!LoadHeaderLines())
            {
                return false;
            }
            return true;
        }

        private bool ParseRequestLine()

        {
            string[] tokens = requestLines[0].Split(' ');
            if ("GET" != tokens[0]&&"POST"!= tokens[0]&& "HEAD" != tokens[0])
            {
                return false;
            }
            string[] http = tokens[2].Split('/');
            string[] version = http[1].Split('.');
            string httpVer = http[0] + version[0] + version[1];
            if ("HTTP10" != httpVer && "HTTP11" != httpVer && "HTTP09" != httpVer)
            {
                return false;
            }
            relativeURI = tokens[1];
           
            if (!ValidateIsURI(relativeURI))
            {
                return false;
            }
            return true;
        }

        private bool ValidateIsURI(string uri)
        {
            if (uri == null || uri == "")
            {
                return false;
            }
            else
            {
                return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
            }
            return true;
        }

        private bool LoadHeaderLines()
        {
            for (int i = 1; i < requestLines.Length - 2; i++)
            {
                string[] parts = requestLines[i].Split(new string[] { ": " }, StringSplitOptions.None);
                headerLines.Add(parts[0], parts[1]);
            }
            return true;
        }

        private bool ValidateBlankLine()
        {
            if (requestLines[requestLines.Length - 2] != "")
                return false;
            return true;
        }

    }
}
