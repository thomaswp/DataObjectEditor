using Emigre.Editor.Reflect;
using Emigre.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;

namespace Emigre.Editor
{
    public class HtmlExporter
    {
        public readonly DataObject root;

        public HtmlExporter(DataObject root)
        {
            this.root = root;
        }

        public string Export()
        {
            return ToHtml(root);
        }

        public static string ToHtml(DataObject obj)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<!doctype html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<link rel='stylesheet' type='text/css' href='style.css'>");
            sb.AppendLine("<script type='text/javascript' src='script.js'></script>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.Append(ToHtml(ToPrintable(obj)));
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            return sb.ToString();
        }

        public static string ToHtml(IPrintable printable)
        {
            Dictionary<string, IPrintable> map = printable.GetItems();
            string name = SecurityElement.Escape(printable.ToString());
            if (map == null || map.Count == 0)
            {
                return name;
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<div>");
            //if (printable is ReflectionPrintable) sb.AppendFormat("<b>{0}</b><br />\n", name);
            foreach (var item in map)
            {
                string key = SecurityElement.Escape(item.Key);
                string itemHtml = ToHtml(item.Value);
                if (!(item.Value is LeafPrintable))
                {
                    itemHtml = "\n" + string.Join("\n", itemHtml.Split('\n').Select(x => "  " + x));
                    sb.AppendFormat("<i>{0}</i>: <a href='#' onclick='toggle(this); return false;'>{1}</a>\n", key, SecurityElement.Escape(item.Value.ToString()));
                    sb.AppendFormat("<div class='hidden'>{0}</div>\n", itemHtml);
                }
                else
                {
                    sb.AppendFormat("<i>{0}</i>: {1}<br/>\n", key, itemHtml);
                }
            }
            sb.AppendLine("</div>");

            return sb.ToString();
        }

        private static IPrintable ToPrintable(object obj)
        {
            if (obj is IPrintable)
            {
                return ((IPrintable)obj);
            }
            else if (obj is DataObject)
            {
                return new ReflectionPrintable(obj as DataObject);
            }
            else if (obj is IList)
            {
                return new ListPrintable(obj as IList);
            }

            else if (obj is Reference)
            {
                return ToPrintable(((Reference)obj).ToString());
            }
            return new LeafPrintable(obj);
        }

        private class ListPrintable : IPrintable
        {
            private Dictionary<string, IPrintable> map = new Dictionary<string, IPrintable>();

            public ListPrintable(IList list)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    map.Add("" + i, ToPrintable(list[i]));
                }
            }

            public Dictionary<string, IPrintable> GetItems()
            {
                return map;
            }

            public override string ToString()
            {
                return "[" + map.Count + "]";
            }
        }

        private class ReflectionPrintable : IPrintable
        {
            private readonly Dictionary<string, IPrintable> map = new Dictionary<string, IPrintable>();
            private readonly string toString;

            public ReflectionPrintable(DataObject obj)
            {
                var accessors = FieldAccessor.GetForObject(obj);
                accessors.AddRange(PropertyAccessor.GetForObject(obj));
                foreach (Accessor accessor in accessors)
                {
                    object value = accessor.Get();
                    if (value is Guid) continue;
                    string name = accessor.GetName();
                    map.Add(name, ToPrintable(value));
                }
                this.toString = obj == null ? "<null>" : obj.ToString();
            }

            public Dictionary<string, IPrintable> GetItems()
            {
                return map;
            }

            public override string ToString()
            {
                return toString;
            }
        }
    }
}
