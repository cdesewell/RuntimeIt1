using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace RuntimeIt1.Exceptions
{

    public class ServiceException : SystemException
    {
        public HttpStatusCode HTTPCode { get; set; }

        public string HTTPMessage { get; set; }

        public ServiceException(HttpStatusCode HTTPCode, string HTTPMessage)
        {
            this.HTTPCode = HTTPCode;
            this.HTTPMessage = HTTPMessage;
        }

    }
}