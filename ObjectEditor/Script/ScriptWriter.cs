
using Emigre.Editor.Reflect;
using Emigre.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Emigre.Editor.Script
{
    public static class ScriptWriter
    {
        public static void Read(IScriptable scriptable, string content)
        {
            if (scriptable == null) return;
            IContext context = JsonSerializer.createContext(scriptable);

            Stack<IScriptable> parents = new Stack<IScriptable>();

            string[] lines = content.Split('\n');
            string body = "";
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (body.Length == 0 && line.Length == 0) continue;
                

                bool newId = line.StartsWith("<!-- [");
                bool newAction = line.StartsWith("`") && line.EndsWith("`");
                bool newAddition = line.StartsWith(">+");

                if (body.Length > 0 && parents.Count > 0 && (newId || newAction || newAddition))
                {
                    body = body.Trim();
                    IScriptable obj = parents.Peek();
                    ReadObject(obj, body);
                    body = "";
                }

                if (newId)
                {
                    int closeIndex = line.IndexOf("]");
                    int indent = int.Parse(line.Substring(6, closeIndex - 6));
                    while (parents.Count > indent) parents.Pop();
                    Guid guid = Guid.Parse(line.Substring(closeIndex + 2, 36));
                    parents.Push(context.GetObject<IScriptable>(guid));
                    continue;
                }
                else if (parents.Count > 1 && newAddition)
                {
                    IScriptable sibling = parents.Pop();
                    IScriptable parent = parents.Peek();

                    IScriptable child = Constructor.Construct(sibling.GetType()) as IScriptable;
                    parents.Push(child);
                    line = line.Substring(2).Trim();
                    foreach (Accessor accessor in FieldAccessor.GetForObject(parent))
                    {
                        IList list = accessor.Get() as IList;
                        if (list == null) continue;

                        int index = list.IndexOf(sibling);
                        if (index < 0) continue;
                        if (index + 1 < list.Count)
                        {
                            IScriptable nextSibling = list[index + 1] as IScriptable;
                            if (nextSibling != null)
                            {
                                string text = WriteObject(nextSibling, new Indent());
                                if (text.Contains(line)) break;
                            }
                        }
                        list.Insert(index + 1, child);
                        break;
                    }
                }
                else if (newAction)
                {
                    continue;
                }
                else if (line.StartsWith("-->"))
                {
                    line = line.Substring(3);
                }

                if (line.StartsWith("~~") && line.EndsWith("~~") && parents.Count > 1)
                {
                    IScriptable child = parents.Pop();
                    IScriptable parent = parents.Peek();
                    parents.Push(child);

                    foreach (Accessor accessor in FieldAccessor.GetForObject(parent))
                    {
                        IList list = accessor.Get() as IList;
                        if (list == null) continue;

                        int index = list.IndexOf(child);
                        if (index < 0) continue;
                        list.RemoveAt(index);
                        break;
                    }
                }

                if (body.Length == 0)
                {
                    // TODO: Make more robust
                    while (line.StartsWith("#") || line.StartsWith(">")) line = line.Substring(1);

                    int typeIndex = line.IndexOf(" `[");
                    if (typeIndex >= 0) line = line.Substring(0, typeIndex);
                }

                body += line.Trim() + "\r\n";
            }
        }

        public static string Write(IScriptable scriptable)
        {
            return "``` Last Update: " + DateTime.Now + " by Auto-Generated ```\n\n" +
                Write(scriptable, new Indent(), 0);   
        }

        private static string Write(IScriptable scriptable, Indent indent, int depth)
        {
            if (scriptable == null) return "";

            StringBuilder sb = new StringBuilder();

            string text = WriteObject(scriptable, indent);
            if (text != null)
            {
                if (!(scriptable is IReadOnlyScriptable))
                {
                    sb.Append("<!-- [" + depth + "](" + scriptable.GetGuid() + ") ");
                    //if (indent.Peek() == null) sb.Append("\n-->");
                    //else sb.AppendLine("-->");
                    sb.AppendLine("-->");
                }
                sb.AppendLine(text);
            }
            
            foreach (Accessor accessor in FieldAccessor.GetForObject(scriptable))
            {
                IList list = accessor.Get() as IList;
                if (list == null) continue;

                for (int i = 0; i < list.Count; i++)
                {
                    IScriptable child = list[i] as IScriptable;
                    if (child == null) continue;
                    IIgnorable ignorable = child as IIgnorable;
                    if (ignorable != null && ignorable.Ignored) continue;
                    sb.AppendLine();
                    sb.Append(Write(child, indent, depth + 1));
                }
            }

            if (text != null)
            {
                indent.Pop();
            }

            return sb.ToString();
        }


        internal static string WriteObject(IScriptable scriptable, Indent indent = null)
        {
            if (scriptable == null) return null;
            IIgnorable ignorable = scriptable as IIgnorable;
            if (ignorable != null && ignorable.Ignored) return null;
            if (scriptable is ICustomScriptable)
            {
                ICustomScriptable s = scriptable as ICustomScriptable;
                string r = s.Write();
                if (indent != null)
                {
                    r = indent.GetIndent(s.Indent()) + r;
                    if (s.Indent() == "#") r += " `[" + scriptable.GetType().Name + "]`";
                }
                return r;
            }

            if (scriptable is IReadOnlyScriptable)
            {
                string r = scriptable.ToString();
                if (indent != null)
                {
                    indent.Push(null);
                    r = "`" + r + "`";
                }
                return r;
            }

            foreach (Accessor accessor in FieldAccessor.GetForObject(scriptable))
            {
                foreach (FieldTag tag in accessor.GetTags())
                {
                    if (tag.flag == FieldTags.Title)
                    {
                        string r = "" + accessor.Get();
                        if (indent != null)
                        {
                            r = indent.GetIndent(tag.arg) + r;
                            if (tag.arg == "#") r += " `[" + scriptable.GetType().Name + "]`";
                        }
                        return r;
                    }
                }
            }
            return null;
        }

        internal static void ReadObject(IScriptable scriptable, string body)
        {
            if (scriptable == null) return;
            if (scriptable is IReadOnlyScriptable) return;

            if (scriptable is ICustomScriptable)
            {
                ICustomScriptable s = scriptable as ICustomScriptable;
                s.Read(body);
                return;
            }

            foreach (Accessor accessor in FieldAccessor.GetForObject(scriptable))
            {
                foreach (FieldTag tag in accessor.GetTags())
                {
                    if (tag.flag == FieldTags.Title)
                    {
                        accessor.Set(body);
                        return;
                    }
                }
            }
        }

        internal class Indent
        {
            private Dictionary<string, int> indents = new Dictionary<string, int>();
            private Stack<string> stack = new Stack<string>();

            public string GetIndent(string symbol)
            {
                if (symbol == null || symbol.Length == 0)
                {
                    stack.Push(null);
                    return "";
                }

                int count = 0;
                if (indents.ContainsKey(symbol)) count = indents[symbol];
                indents[symbol] = ++count;
                stack.Push(symbol);

                string use = "";
                for (int i = 0; i < count && i < 4; i++) use += symbol;
                use += " ";
                return use;
            }

            public void Push(string arg)
            {
                stack.Push(arg);
            }

            public void Pop()
            {
                string symbol = stack.Pop();
                if (symbol != null) indents[symbol] = indents[symbol] - 1;
            }

            public string Peek()
            {
                return stack.Peek();
            }
        }
    }
}
