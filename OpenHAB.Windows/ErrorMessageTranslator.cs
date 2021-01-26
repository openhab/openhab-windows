using System;
using OpenHAB.Core.Common;

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