using System;
using System.Runtime.Serialization;

namespace SAPHub.ConnectorModule
{
    /// <summary>
    /// This exception will be thrown when no RFC configuration has been configured.
    /// </summary>
    [Serializable]
    public class RfcConfigMissingException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public RfcConfigMissingException()
        {
        }

        public RfcConfigMissingException(string message) : base(message)
        {
        }

        public RfcConfigMissingException(string message, Exception inner) : base(message, inner)
        {
        }

        protected RfcConfigMissingException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}