using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Uwp.Connectivity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenHAB.Core.Common;
using OpenHAB.Core.Contracts.Services;
using OpenHAB.Core.Messages;
using OpenHAB.Core.Model;
using OpenHAB.Core.Model.Connection;

namespace OpenHAB.Core.SDK
{

    public class ErrorMessageTranslator
    {
        public static void FireUIError(string context, ErrorTypes error, Exception exception)
        {
            string message = GetErrorMessageByType(error);
        }

        private static string GetErrorMessageByType(ErrorTypes error)
        {
           string errorMessage = AppResources.Values.GetString(error.ToString());

           return errorMessage;
        }
    }
}