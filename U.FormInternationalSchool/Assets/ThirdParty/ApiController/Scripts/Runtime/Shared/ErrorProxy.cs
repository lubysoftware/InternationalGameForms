using System;

namespace API
{
    [Serializable]
    public class ErrorProxy
    {
        public StatusCode statusCode;
        public DateTime timestamp;
        public string path;
        public object message;
        public string error;
    }
}