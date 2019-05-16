namespace Emigre.Json
{

    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class JsonSerializer : FieldData
    {

        private const String ID = "__id", CLASS = "__class";

        private Writer writer;
        private JsonObject obj;
        private bool write;
        private bool copyGuids;
        private List<DataObject> seenObjects = new List<DataObject>();
        private Dictionary<Guid, Guid> guidMap = new Dictionary<Guid, Guid>();
        private List<Reference> references = new List<Reference>();
        private IWritableContext context = new MapContext();

        private HashSet<String> keys = new HashSet<String>();

        private int seenObjectIndex(DataObject obj)
        {
            int size = seenObjects.Count;
            for (int i = 0; i < size; i++) if (obj == seenObjects[i]) return i;
            return -1;
        }

        public static T copy<T>(T obj) where T : DataObject
        {
            return fromJson<T>(toJson(obj), true);
        }

        public static String toJson(DataObject data)
        {
            JsonSerializer serializer = createWriteSerializer(data);
            return serializer.writer.writeObject();
        }

        public static IWritableContext createContext(DataObject data)
        {
            JsonSerializer serializer = createWriteSerializer(data);
            return serializer.context;
        }

        private static JsonSerializer createWriteSerializer(DataObject data)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.writer = new Writer();
            serializer.write = true;
            serializer.writeObject(data, null);
            return serializer;
        }

        public static void copyFields(DataObject o1, DataObject o2, bool copy = false)
        {
            string json = toJson(o1);
            JsonSerializer serializer = new JsonSerializer();
            serializer.write = false;
            serializer.copyGuids = copy;
            serializer.obj = new JsonObject(json);
            serializer.readObjectBody(() =>
            {
                o2.AddFields(serializer);
            }, serializer.obj);
            if (copy) serializer.UpdateReferences();
        }

        private void writeObject(DataObject data, String key)
        {
            writeObject(data, key, data == null ? null : data.GetType());
        }

        private void writeObject(DataObject data, String key, Type type)
        {
            if (data == null)
            {
                if (key == null) writer.nul();
                else writer.nul(key);
                return;
            }
            int index = seenObjectIndex(data);
            if (index >= 0)
            {
                if (key == null) writer.value(index);
                else writer.value(key, index);
                return;
            }
            index = seenObjects.Count;
            seenObjects.Add(data);
            
            writeObjectJson(key, () =>
                {
                    writer.value(makeKey(CLASS), className(type));
                    writer.value(makeKey(ID), index);

                    if (data is GuidDataObject)
                    {
                        context.Add(data as GuidDataObject);
                    }

                    data.AddFields(this);
                });
        }

        private void writeObjectJson(String key, Action body)
        {
            if (key == null)
            {
                writer.@object();
            }
            else
            {
                writer.@object(key);
            }
            HashSet<String> stackKeys = this.keys;
            this.keys = new HashSet<String>();
            body();
            this.keys = stackKeys;
            writer.end();
        }

        public static T fromJson<T>(String json, bool copy = false, IWritableContext context = null) where T : DataObject
        {
            return (T)fromJson(json, copy, context);
        }

        public static DataObject fromJson(String json, bool copy = false, IWritableContext context = null)
        {
            if (json == null || json.Length == 0) return null;
            JsonSerializer serializer = new JsonSerializer();
            serializer.write = false;
            serializer.copyGuids = copy;
            if (context != null) serializer.context = context;
            DataObject obj = serializer.read(new JsonObject(json));
            if (copy) serializer.UpdateReferences();
            return obj;
        }

        private DataObject read(String key)
        {
            if (isIndex(obj.getRaw(key)))
            {
                int index = obj.getInt(key);
                return seenObjects[index];
            }
            return read(obj.getObject(key));
        }

        private DataObject read(JsonObject obj, bool skipUnknownClasses = false)
        {
            if (obj == null) return null;
            if (!obj.containsKey(ID)) return null;

            // First check to see if it's been created, and if so simply return it
            int id = obj.getInt(ID);
            if (id < seenObjects.Count)
            {
                DataObject seen = seenObjects[id];
                if (seen != null) return seen;
            }

            // Create the (unpopulated) object and register it
            String className = obj.getString(CLASS);
            if (className == null) return null;
            DataObject data = Constructor.Construct(className, skipUnknownClasses);
            while (seenObjects.Count <= id) seenObjects.Add(null);
            seenObjects[id] = data;

            // Instantiate the children of this object in the JSON file
            // This is done to create objects whose parents may no longer
            // exist in the data-structure, but who are themselves still
            // referenced. This occurs when fields are removed from a class.
            foreach (var kvp in obj.Entries)
            {
                object value = kvp.Value;
                List<object> list = value as List<object>;
                if (list != null)
                {
                    foreach (object entry in list)
                    {
                        read(entry as Dictionary<string, object>, true);
                    }
                }
                read(value as Dictionary<string, object>, true);
            }

            // If there's not corresponding C# class, we stop here.
            // This may happen if the class was deleted, but its data
            // is still in an older JSON file.
            if (data == null) return null;
            
            // Now actually populate the object, having guaranteed
            // that all its children have been created (but not
            // necessarily populated!)
            readObjectBody(() =>
                {
                    makeKey(CLASS);
                    makeKey(ID);

                    data.AddFields(this);
                    if (data is GuidDataObject)
                    {
                        context.Add(data as GuidDataObject);
                    }
                }, obj);

            return data;
        }

        private void readObjectBody(Action body, JsonObject obj)
        {
            JsonObject stackObj = this.obj;
            HashSet<String> stackKeys = this.keys;
            this.obj = obj;
            this.keys = new HashSet<String>();
            body();
            this.obj = stackObj;
            this.keys = stackKeys;
        }

        private void UpdateReferences()
        {
            foreach (Reference reference in references)
            {
                if (guidMap.ContainsKey(reference.Guid))
                {
                    reference.Guid = guidMap[reference.Guid];
                }
            }
        }

        private String nextField()
        {
            return null;
        }

        private String makeKey()
        {
            return "__k" + keys.Count;
        }

        private String keyName(String key)
        {
            if (key == null)
            {
                key = makeKey();
            }
            while (keys.Contains(key))
            {
                key += "_";
            }
            return key;

        }

        private String makeKey(String key)
        {
            key = keyName(key);
            if (readMode() && !obj.containsKey(key))
            {
                String tempKey = makeKey();
                if (obj.containsKey(tempKey)) key = tempKey;
            }
            keys.Add(key);
            return key;
        }

        private String className(Type clazz)
        {
            return Constructor.ClassName(clazz);
        }

        public bool writeMode()
        {
            return write;
        }

        public bool readMode()
        {
            return !write;
        }

        private void invalidArrayLength()
        {
            throw new ParseDataException("JsonArray length mismatch");
        }


        public int add(int x)
        {
            return add(x, nextField());
        }


        public int add(int x, String field)
        {
            String key = makeKey(field);
            if (write)
            {
                writer.value(key, x);
                return x;
            }
            else
            {
                if (!obj.containsKey(key)) return x;
                return obj.getInt(key);
            }
        }


        public long add(long x)
        {
            return add(x, nextField());
        }


        public long add(long x, String field)
        {
            String key = makeKey(field);
            if (write)
            {
                writer.value(key, x);
                return x;
            }
            else
            {
                if (!obj.containsKey(key)) return x;
                return obj.getLong(key);
            }
        }


        public short add(short x)
        {
            return add(x, nextField());
        }


        public short add(short x, String field)
        {
            String key = makeKey(field);
            if (write)
            {
                writer.value(key, x);
                return x;
            }
            else
            {
                if (!obj.containsKey(key)) return x;
                return (short)obj.getInt(key);
            }
        }


        public float add(float x)
        {
            return add(x, nextField());
        }


        public float add(float x, String field)
        {
            String key = makeKey(field);
            if (write)
            {
                writer.value(key, x);
                return x;
            }
            else
            {
                if (!obj.containsKey(key)) return x;
                return (float)obj.getDouble(key);
            }
        }


        public double add(double x)
        {
            return add(x, nextField());
        }


        public double add(double x, String field)
        {
            String key = makeKey(field);
            if (write)
            {
                writer.value(key, x);
                return x;
            }
            else
            {
                if (!obj.containsKey(key)) return x;
                return obj.getDouble(key);
            }
        }


        public byte add(byte x)
        {
            return add(x, nextField());
        }


        public byte add(byte x, String field)
        {
            String key = makeKey(field);
            if (write)
            {
                writer.value(key, x);
                return x;
            }
            else
            {
                if (!obj.containsKey(key)) return x;
                return (byte)obj.getInt(key);
            }
        }


        public char add(char x)
        {
            return add(x, nextField());
        }


        public char add(char x, String field)
        {
            String key = makeKey(field);
            if (write)
            {
                writer.value(key, x);
                return x;
            }
            else
            {
                if (!obj.containsKey(key)) return x;
                String s = obj.getString(key);
                if (s.Length != 1) throw new ParseDataException("Char must be saved as string of length 1");
                return s[0];
            }
        }


        public bool add(bool x)
        {
            return add(x, nextField());
        }


        public bool add(bool x, String field)
        {
            String key = makeKey(field);
            if (write)
            {
                writer.value(key, x);
                return x;
            }
            else
            {
                if (!obj.containsKey(key)) return x;
                return obj.getBool(key);
            }
        }


        public String add(String x)
        {
            return add(x, nextField());
        }


        public String add(String x, String field)
        {
            String key = makeKey(field);
            if (write)
            {
                writer.value(key, x);
                return x;
            }
            else
            {
                if (!obj.containsKey(key)) return x;
                return obj.getString(key);
            }
        }

        public Reference<T> add<T>(Reference<T> x) where T : GuidDataObject
        {
            return addReference<T>(x, nextField());
        }

        public Reference<T> addReference<T>(Reference<T> x, String field) where T : GuidDataObject
        {
            if (write)
            {
                string str = null;
                if (x != null && x.Guid != Guid.Empty) str = x.Guid.ToString();
                add(x == null ? null : str, field);
            }
            else
            {
                if (x == null) x = new Reference<T>();
                string guidString = add((string)null, field);
                x.Guid = Guid.Empty;
                if (guidString != null)
                {
                    try
                    {
                        x.Guid = new Guid(guidString);
                    }
                    catch
                    { }
                }
                x.Context = context;
                references.Add(x);
            }
            return x;
        }

        public T addReference<T>(T x, String field) where T : GuidDataObject
        {
            T val = addReference(new Reference<T>(null, x), field).Value;
            if (write) return x;
            return val;
        }

        public Guid addGuid(Guid x)
        {
            return addGuid(x, nextField());
        }


        public Guid addGuid(Guid x, String field)
        {
            if (write)
            {
                add(x.ToString(), field);
                return x;
            }
            else
            {
                string guidString = add("", field);
                if (guidString == null || guidString.Length == 0) return x;
                try
                {
                    Guid guid = new Guid(guidString);
                    if (copyGuids)
                    {
                        Guid nGuid = Guid.NewGuid();
                        guidMap.Add(guid, nGuid);
                        return nGuid;
                    }
                    return guid;
                }
                catch 
                {
                    return x;
                }
            }

        }

        public T add<T>(T x) where T : DataObject
        {
            return add(x, nextField());
        }


        public T add<T>(T x, String field) where T : DataObject
        {
            String key = makeKey(field);
            if (write)
            {
                writeObject(x, key);
                return x;
            }
            else
            {
                if (!obj.containsKey(key)) return x;
                return (T)read(key);
            }
        }

        public T[] addPrimitiveArray<T>(T[] x, String field) where T : struct
        {
            String key = makeKey(field);
            if (write)
            {
                if (x == null)
                {
                    writer.nul(key);
                }
                else
                {
                    int length = x.Length;
                    writer.array(key);
                    for (int i = 0; i < length; i++) writer.value(x[i]);
                    writer.end();
                }
            }
            else
            {
                if (!obj.containsKey(key)) return x;
                List<object> a = obj.getArray(key);
                if (a == null) return null;
                int length = a.Count;
                if (x == null || x.Length != length)
                {
                    x = new T[length];
                }
                for (int i = 0; i < length; i++)
                {
                    x[i] = (T)a[i];
                }
            }
            return x;
        }


        public T[] addArray<T>(T[] x) where T : DataObject
        {
            return addArray(x, nextField());
        }

        public T[] addArray<T>(T[] x, String field) where T : DataObject
        {
            String key = makeKey(field);
            if (write)
            {
                writer.array(key);
                foreach (T v in x) writeObject(v, null);
                writer.end();
                return x;
            }
            else
            {
                if (!obj.containsKey(key)) return x;
                List<object> a = obj.getArray(key);
                if (x == null) throw new ParseDataException("JsonArray cannot be null");
                if (x.Length != a.Count) invalidArrayLength();
                for (int i = 0; i < x.Length; i++)
                {
                    if (isIndex(a[i]))
                    {
                        x[i] = (T)seenObjects[(int)a[i]];
                    }
                    else
                    {
                        x[i] = (T)read((JsonObject)a[i]);
                    }
                }
                return x;
            }
        }

        private static bool isIndex(object o)
        {
            return o is long || o is int;
        }


        public List<T> addList<T>(List<T> x) where T : DataObject
        {
            return addList(x, nextField());
        }

        public List<T> addList<T>(List<T> x, String field) where T : DataObject
        {
            String key = makeKey(field);
            if (write)
            {
                if (x == null)
                {
                    writer.nul(key);
                    return x;
                }
                writer.array(key);
                foreach (T v in x) writeObject(v, null);
                writer.end();
                return x;
            }
            else
            {
                if (!obj.containsKey(key)) return x;
                if (obj.isNull(key)) return null;
                List<object> a = obj.getArray(key);
                if (x == null) throw new ParseDataException("List cannot be null");
                x.Clear();
                int size = a.Count;
                for (int i = 0; i < size; i++)
                {
                    if (isIndex(a[i]))
                    {
                        x.Add((T)seenObjects[Convert.ToInt32(a[i])]);
                    }
                    else
                    {
                        x.Add((T)read(new JsonObject(a[i])));
                    }
                }
                return x;
            }
        }

        public List<T> addList<T>(List<T> x, String field, Func<T, T> f)
        {
            String key = makeKey(field);
            if (write)
            {
                if (x == null)
                {
                    writer.nul(key);
                    return x;
                }
                writer.array(key);
                for (int i = 0; i < x.Count; i++)
                {
                    writeObjectJson(null, () => f(x[i]));
                }
                writer.end();
                return x;
            }
            else
            {
                if (!obj.containsKey(key)) return x;
                if (obj.isNull(key)) return null;
                List<object> a = obj.getArray(key);
                if (x == null) throw new ParseDataException("List cannot be null");
                x.Clear();
                int size = a.Count;
                for (int i = 0; i < size; i++)
                {
                    JsonObject item = new JsonObject(a[i]);
                    readObjectBody(() =>
                        {
                            x.Add(f(default(T)));
                        }, item);
                }
                return x;
            }
        }

        public List<T> addPrimitiveList<T>(List<T> x) where T : struct
        {
            return addPrimitiveList<T>(x, nextField());
        }

        public List<T> addPrimitiveList<T>(List<T> x, String field) where T : struct
        {
            String key = makeKey(field);
            if (write)
            {
                if (x == null)
                {
                    writer.nul(key);
                }
                else
                {
                    int length = x.Count;
                    writer.array(key);
                    for (int i = 0; i < length; i++) writer.value(x[i]);
                    writer.end();
                }
            }
            else
            {
                if (!obj.containsKey(key)) return x;
                List<object> a = obj.getArray(key);
                if (a == null) return null;
                int length = a.Count;
                if (x == null || x.Count != length)
                {
                    x = new List<T>();
                }
                for (int i = 0; i < length; i++)
                {
                    x.Add((T)a[i]);
                }
            }
            return x;
        }

        public T addEnum<T>(T x, String field) where T : struct
        {
            return addEnum(x, x, field);
        }

        public T addEnum<T>(T x, T deflt, String field) where T : struct
        {
            if (write)
            {
                string name = Enum.GetName(typeof(T), x);
                int i = 0;
                foreach (string n in Enum.GetNames(typeof(T)))
                {
                    if (n == name) break;
                    i++;
                }
                // store as index:value
                add(i + ":" + name, field);
                return x;
            }
            else
            {
                string key = makeKey(field);
                if (obj.isNull(key)) return deflt;
                // old versions stored just the index as an int, so we have to case to a string
                string stored = "" + obj.getRaw(key);  

                Array values = Enum.GetValues(typeof(T));
                string indexStr = stored;

                int colon = stored.IndexOf(":");
                if (colon >= 0)
                {
                    // get the value and check if it's present in the enum
                    string value = stored.Substring(colon + 1);
                    foreach (object v in values)
                    {
                        if (v.ToString().Equals(value)) return (T)v;
                    }
                    // if not try to find it by the index
                    indexStr = stored.Substring(0, colon);
                }

                // try to parse the index
                int index;
                if (!int.TryParse(indexStr, out index)) return deflt;

                // return the enum with the given index if it's in range
                if (index >= values.Length) return deflt;
                return (T)Enum.GetValues(typeof(T)).GetValue(index);
            }
        }

        public Dictionary<T, U> addDictionary<T, U>(Dictionary<T, U> x, string field, Func<T, T> fT, Func<U, U> fU)
        {
            String key = makeKey(field);
            if (write)
            {
                if (x == null)
                {
                    writer.nul(key);
                    return x;
                }
                writer.array(key);
                foreach (var entry in x)
                {
                    writer.@object();
                    writeObjectJson("k", () => fT(entry.Key));
                    writeObjectJson("v", () => fU(entry.Value));
                    writer.end();
                }
                writer.end();
                return x;
            }
            else
            {
                if (!obj.containsKey(key)) return x;
                if (obj.isNull(key)) return null;
                List<object> a = obj.getArray(key);
                if (x == null) throw new ParseDataException("List cannot be null");
                x.Clear();
                int size = a.Count;
                for (int i = 0; i < size; i++)
                {
                    JsonObject item = new JsonObject(a[i]);
                    JsonObject keyObj = item.getObject("k");
                    JsonObject valueObj = item.getObject("v");
                    T k = default(T);
                    readObjectBody(() =>
                    {
                        k = fT(default(T));
                    }, keyObj);
                    U v = default(U);
                    readObjectBody(() =>
                    {
                        v = fU(default(U));
                    }, valueObj);
                    x.Add(k, v);
                }
                return x;
            }
        }

        protected static void error(String message, Exception exception)
        {
            Console.WriteLine("Error: " + message);
            Console.WriteLine(exception.StackTrace.ToString());
        }

        public class MapContext : IWritableContext
        {
            public Dictionary<Guid, GuidDataObject> map = new Dictionary<Guid, GuidDataObject>();

            public void Add(GuidDataObject obj)
            {
                if (obj != null) map.Add(obj.GetGuid(), obj);
            }

            public IEnumerable<GuidDataObject> EnumerateObjects()
            {
                return map.Values;
            }

            public T GetObject<T>(Guid guid) where T : GuidDataObject
            {
                if (map.ContainsKey(guid))
                {
                    GuidDataObject obj = map[guid];
                    if (obj is T) return (T)obj;
                }
                return default(T);
            }
        }
    }

}