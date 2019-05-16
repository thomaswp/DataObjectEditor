using System.Collections.Generic;

namespace Emigre.Json
{
    public interface IPrintable
    {
        Dictionary<string, IPrintable> GetItems();
    }

    public class LeafPrintable : IPrintable
    {
        private readonly string value;

        public LeafPrintable(object obj)
        {
            value = obj == null ? "<null>" : obj.ToString();
        }

        public Dictionary<string, IPrintable> GetItems() { return null; }

        public static implicit operator LeafPrintable(string obj)
        {
            return new LeafPrintable(obj);
        }

        public override string ToString()
        {
            return value;
        }
    }
}
