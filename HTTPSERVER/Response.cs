﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200, ///////
        InternalServerError = 500,
        NotFound = 404,/////
        BadRequest = 400,/////
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
        
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            headerLines.Add("Content-Type: "+contentType);
            headerLines.Add("Content-Length: " +content.Length.ToString());
            headerLines.Add("Date: "+DateTime.Now.ToString());
            headerLines.Add("");
            headerLines.Add(content);
            if (code == StatusCode.Redirect)
            {
                headerLines.Add(redirectoinPath);
            }
            // TODO: Create the request string
            responseString = GetStatusLine(code) + "\r\n";
            foreach(var s in headerLines)
            {
                responseString += s;
                responseString += "\r\n";
            }
        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = string.Empty;
            if (code == StatusCode.OK)
            {
                statusLine = Configuration.ServerHTTPVersion + " 200 OK";
            }
            else if (code == StatusCode.BadRequest)
            {
                statusLine = Configuration.ServerHTTPVersion + " 400 BadRequest";
            }
            else if (code == StatusCode.InternalServerError)
            {
                statusLine = Configuration.ServerHTTPVersion + " 500 InternalServer";
            }
            else if (code == StatusCode.NotFound)
            {
                statusLine = Configuration.ServerHTTPVersion + " 404 NotFound";
            }
            else if (code == StatusCode.Redirect)
            {
                statusLine = Configuration.ServerHTTPVersion + " 301 Redirection";
            }
            return statusLine;
        }
    }
}
