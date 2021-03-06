﻿//----------------------------------------------------------------------
//
// Copyright (c) Microsoft Corporation.
// All rights reserved.
//
// This code is licensed under the MIT License.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//------------------------------------------------------------------------------

using System;
using System.Globalization;
using Microsoft.Identity.Core;

namespace Microsoft.Identity.Client.Internal
{
    internal class MsalLogger : CoreLoggerBase
    {
        internal MsalLogger(Guid correlationId, string component) : base(correlationId)
        {
            Component = string.Empty;
            if (!string.IsNullOrEmpty(component))
            {
                //space is intentional for formatting of the message
                Component = string.Format(CultureInfo.InvariantCulture, " ({0})", component);
            }

            PiiLoggingEnabled = Logger.PiiLoggingEnabled;
        }

        internal string Component { get; set; }

        public override void InfoPii(string message)
        {
            Log(LogLevel.Info, message, true);
        }

        public override void Verbose(string message)
        {
            Log(LogLevel.Verbose, message, false);
        }

        public override void VerbosePii(string message)
        {
            Log(LogLevel.Verbose, message, true);
        }

        public override void ErrorPii(string message)
        {
            Log(LogLevel.Error, message, true);
        }

        public override void Warning(string message)
        {
            Log(LogLevel.Warning, message, false);
        }

        public override void WarningPii(string message)
        {
            Log(LogLevel.Warning, message, true);
        }

        public override void Info(string message)
        {
            Log(LogLevel.Info, message, false);
        }

        public override void Error(Exception ex)
        {
            Log(LogLevel.Error,
                MsalExceptionFactory.GetPiiScrubbedExceptionDetails(ex),
                false);
        }

        public override void ErrorPii(Exception ex)
        {
            ErrorPii(ex.ToString());
        }

        public override void Error(string message)
        {
            Log(LogLevel.Error, message, false);
        }


        private static void ExecuteCallback(LogLevel level, string message, bool containsPii)
        {
            lock (Logger.LockObj)
            {
                Logger.LogCallback?.Invoke(level, message, containsPii);
            }
        }

        private void Log(LogLevel msalLogLevel, string logMessage, bool containsPii)
        {
            if ((msalLogLevel > Logger.Level) || (!Logger.PiiLoggingEnabled && containsPii))
            {
                return;
            }

            //format log message;
            string correlationId = (CorrelationId.Equals(Guid.Empty))
                ? string.Empty
                : " - " + CorrelationId;

            var msalIdParameters = MsalIdHelper.GetMsalIdParameters();
            string os = "N/A";
            if (msalIdParameters.ContainsKey(MsalIdParameter.OS))
            {
                os = msalIdParameters[MsalIdParameter.OS];
            }

            string log = string.Format(CultureInfo.InvariantCulture, "MSAL {0} {1} {2} [{3}{4}]{5} {6}",
                MsalIdHelper.GetMsalVersion(),
                msalIdParameters[MsalIdParameter.Product],
                os, DateTime.UtcNow, correlationId, Component, logMessage);

            if (Logger.PiiLoggingEnabled && containsPii)
            {
                const string piiLogMsg = "(True) ";
                log = piiLogMsg + log;
            }
            else
            {
                const string noPiiLogMsg = "(False) ";
                log = noPiiLogMsg + log;
            }

            if (Logger.DefaultLoggingEnabled)
            {
                PlatformPlugin.LogMessage(Logger.Level, log);
            }

            ExecuteCallback(msalLogLevel, log, containsPii);
        }
    }
}
