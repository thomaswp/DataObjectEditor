using System;

namespace Emigre.Json
{
    public class ParseDataException : Exception
    {
        public ParseDataException(string message) : base(message) { }
    }
}
