using System;
using System.Collections.Generic;

namespace ObjectEditor.Json
{
    public abstract class Reference
    {
        public IContext Context { get; set; }

        private Guid _guid;
        public Guid Guid
        {
            get { return _guid; }
            set { _guid = value; }
        }

        public Reference() { }

        public Reference(IContext context)
        {
            Context = context;
        }

        public bool IsNull { get { return _guid == Guid.Empty; } }

        public GuidDataObject Get(IContext context = null)
        {
            if (context == null) context = Context;
            if (context == null) return null;

            return context.GetObject<GuidDataObject>(_guid);
        }

        public void Set(GuidDataObject value)
        {
            _guid = value == null ? Guid.Empty : value.GetGuid();
            if (value != null && Context == null)
            {
                JsonSerializer.MapContext context = new JsonSerializer.MapContext();
                context.Add(value);
                this.Context = context;
            }
        }

        public override string ToString()
        {
            if (Guid == Guid.Empty) return "<null>";
            GuidDataObject obj = Get();
            return obj == null ? ("{" + Guid.ToString() + "}") : obj.ToString();
        }
    }

    public class Reference<T> : Reference where T : GuidDataObject
    {
        public T Value
        {
            get
            {
                return Context == null ? default(T) : Context.GetObject<T>(Guid);
            }
            set
            {
                Set(value);
            }
        }

        public Reference() { }

        public Reference(IContext context, T value = default(T)) 
            : base(context)
        {
            Value = value; 
        }

        public new T Get(IContext context = null)
        {
            if (context == null) context = Context;
            if (context == null) return default(T);

            return context.GetObject<T>(Guid);
        }

        public static implicit operator T(Reference<T> value)
        {
            return value == null ? default(T) : value.Value;
        }
    }

    public interface IContext
    {
        T GetObject<T>(Guid guid) where T : GuidDataObject;
        IEnumerable<GuidDataObject> EnumerateObjects();
    }

    public interface IWritableContext : IContext
    {
        void Add(GuidDataObject obj);
    }
}
