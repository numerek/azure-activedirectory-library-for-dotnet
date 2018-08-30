﻿using System;

namespace Test.Microsoft.Identity.Core.Unit.Mocks
{
    public class TestClientException : TestException
    {
      
        /// <summary>
        /// Initializes a new instance of the exception class with a specified
        /// error code.
        /// </summary>
        /// <param name="errorCode">
        /// The error code returned by the service or generated by client. This is the code you can rely on
        /// for exception handling.</param>
        public TestClientException(string errorCode) : base(errorCode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the exception class with a specified
        /// error code and error message.
        /// </summary>        
        /// <param name="errorCode">
        /// The error code returned by the service or generated by client. This is the code you can rely on
        /// for exception handling.
        /// </param>
        /// <param name="errorMessage">The error message that explains the reason for the exception.</param>
        public TestClientException(string errorCode, string errorMessage) : base(errorCode, errorMessage)
        {
        }

        /// <summary>
        /// Initializes a new instance of the exception class with a specified
        /// error code, error message and inner exception.
        /// </summary>
        /// <param name="errorCode">
        /// The error code returned by the service or generated by client. This is the code you can rely on
        /// for exception handling.
        /// </param>
        /// <param name="errorMessage">The error message that explains the reason for the exception.</param>
        /// <param name="innerException"></param>
        public TestClientException(string errorCode, string errorMessage, Exception innerException) : base(errorCode, errorMessage, innerException)
        {
        }
    }
}