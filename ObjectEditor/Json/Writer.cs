using System.Collections.Generic;

namespace Emigre.Json
{
    public class Writer
    {
        private Stack<object> writing = new Stack<object>();
        private List<object> finished = new List<object>();

        internal string writeObject()
        {
            if (writing.Count > 0) throw new ParseDataException("Did not end an object or array");
            foreach (object obj in finished)
            {
                return MiniJSON.Json.Serialize(obj);
            }
            return "";
        }

        internal void nul()
        {
            nul(null);
        }

        internal void nul(string key)
        {
            value(key, null);
        }

        internal void value(object value)
        {
            this.value(null, value);
        }

        internal void value(string key, object value)
        {
            if (writing.Count == 0) return;
            object toWrite = writing.Peek();
            if (toWrite is List<object>)
            {
                (toWrite as List<object>).Add(value);
            }
            else if (toWrite is Dictionary<object, object>)
            {
                if (key == null) throw new ParseDataException("Cannot add null-key data to object");
                (toWrite as Dictionary<object, object>)[key] = value;
            }
            else
            {
                throw new ParseDataException("Can only add values to objects or arrays");
            }
        }

        internal void @object()
        {
            @object(null);
        }

        internal void @object(string key)
        {
            Dictionary<object, object> obj = new Dictionary<object, object>();
            value(key, obj);
            writing.Push(obj);
        }

        internal void array()
        {
            array(null);
        }

        internal void array(string key)
        {
            List<object> list = new List<object>();
            value(key, list);
            writing.Push(list);
        }

        internal void end()
        {
            if (writing.Count == 0) throw new ParseDataException("Ended too many objects or arrays");
            object finished = writing.Pop();
            if (writing.Count == 0) this.finished.Add(finished);
        }
    }
}
