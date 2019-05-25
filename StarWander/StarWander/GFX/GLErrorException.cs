using System;

namespace StarWander.GFX
{
    internal class GLErrorException : Exception
    {
        public string LogMessage { get; private set; }

        public GLErrorException(string logMessage) : base($"GL error occured: {logMessage}")
        {
            LogMessage = logMessage;
        }
    }
}
