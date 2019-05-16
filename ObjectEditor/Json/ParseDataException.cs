using System;

namespace ObjectEditor.Json
{
    public class ParseDataException : Exception
    {
        public ParseDataException(string message) : base(message) { }
    }
}
