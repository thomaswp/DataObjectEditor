using System;
using System.Collections.Generic;

namespace ObjectEditor.Json
{
    public class JsonObject
    {
        private readonly Dictionary<string, object> map;

        public IEnumerable<KeyValuePair<string,object>> Entries
        {
            get { return map; }
        }

        public JsonObject(string json)
        {
            map = (Dictionary<string, object>)MiniJSON.Json.Deserialize(json);
        }

        public JsonObject(object map)
        {
            this.map = (Dictionary<string, object>)map;
        }

        internal object getRaw(string key)
        {
            return map[key];
        }

        internal int getInt(string key)
        {
            return Convert.ToInt32(map[key]);
        }

        internal JsonObject getObject(string key)
        {
            var m = map[key];
            if (m == null) return null;
            return new JsonObject(map[key]);
        }

        internal string getString(string key)
        {
            return (string) map[key];
        }

        internal bool containsKey(string key)
        {
            return map.ContainsKey(key);
        }

        internal long getLong(string key)
        {
            return (long)map[key];
        }

        internal double getDouble(string key)
        {
            return Convert.ToDouble(map[key]);
        }

        internal bool getBool(string key)
        {
            return (bool)map[key];
        }

        internal List<object> getArray(string key)
        {
            return (List<object>)map[key];
        }

        internal bool isNull(string key)
        {
            return !containsKey(key) || map[key] == null;
        }

        public static implicit operator JsonObject(Dictionary<string, object> map)
        {
            if (map == null) return null;
            return new JsonObject(map);
        }
    }
}
