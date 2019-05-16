using Emigre.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Emigre.Editor.Script
{
    public static class ScriptCorrector
    {
        public static string Correct(IScriptable scriptable, string script)
        {
            IContext context = JsonSerializer.createContext(scriptable);
            List<IScriptable> children = context.EnumerateObjects().Where(x => ScriptWriter.WriteObject(x as IScriptable) != null).Cast<IScriptable>().ToList();
            List<string> lines = children.Select(x => ScriptWriter.WriteObject(x)).Select(x => RemoveSpeaker(ReplaceSmartCharacters(x.Trim()))).ToList();
            
            string[] scriptLines = script.Split('\n');

            StringBuilder sb = new StringBuilder();

            DiffForm form = new DiffForm();

            string scriptLine = null;

            int index;
            for (index = 0; index < scriptLines.Length; index++)
            {
                if (scriptLine != null)
                {
                    if (!scriptLine.StartsWith("//")) scriptLine = "// " + scriptLine;
                    sb.AppendLine(scriptLine.Trim());
                }

                scriptLine = scriptLines[index];
                if (scriptLine.StartsWith("//")) continue;

                string trimmedLine = RemoveSpeaker(ReplaceSmartCharacters(scriptLine.Trim()));
                if (trimmedLine.Length == 0) continue;

                int minIndex = -1;
                int minDis = int.MaxValue;

                for (int i = 0; i < lines.Count; i++)
                {
                    string line = lines[i];
                    int dis = StringEditDistance(line, trimmedLine);
                    if (dis < minDis)
                    {
                        minDis = dis;
                        minIndex = i;
                    }
                }

                if (minIndex == -1 || children[minIndex] is IReadOnlyScriptable) continue;

                IScriptable minChild = children[minIndex];
                ScriptWriter.Indent indent = new ScriptWriter.Indent();
                ScriptWriter.WriteObject(minChild, indent);
                if (indent.Peek() == "#") continue;

                string minLine = lines[minIndex];

                if (RemoveHTML(minLine) != trimmedLine)
                {
                    int[,] d;
                    StringEditDistance(minLine, trimmedLine, out d);

                    //Console.WriteLine(minLine);
                    //Console.WriteLine(trimmedLine);
                    //Console.WriteLine(string.Join("//", Diff(minLine, trimmedLine)));
                    //Console.WriteLine("------------------------");

                    form.Init(children[minIndex].GetType().Name, trimmedLine, minLine);
                    var result = form.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        ScriptWriter.ReadObject(children[minIndex], form.NewText);
                    }
                    else if (result == System.Windows.Forms.DialogResult.Abort)
                    {
                        scriptLine = null;
                        break;
                    }
                }
            }

            if (scriptLine != null)
            {
                if (!scriptLine.StartsWith("//")) scriptLine = "// " + scriptLine;
                sb.AppendLine(scriptLine.Trim());
            }

            for (; index < scriptLines.Length; index++)
            {
                sb.AppendLine(scriptLines[index].Trim());
            }

            return sb.ToString();
        }

        // credit: http://stackoverflow.com/a/2205826/816458
        internal static string ReplaceSmartCharacters(string buffer)
        {
            if (buffer.IndexOf('\u2013') > -1) buffer = buffer.Replace('\u2013', '-');
            if (buffer.IndexOf('\u2014') > -1) buffer = buffer.Replace('\u2014', '-');
            if (buffer.IndexOf('\u2015') > -1) buffer = buffer.Replace('\u2015', '-');
            if (buffer.IndexOf('\u2017') > -1) buffer = buffer.Replace('\u2017', '_');
            if (buffer.IndexOf('\u2018') > -1) buffer = buffer.Replace('\u2018', '\'');
            if (buffer.IndexOf('\u2019') > -1) buffer = buffer.Replace('\u2019', '\'');
            if (buffer.IndexOf('\u201a') > -1) buffer = buffer.Replace('\u201a', ',');
            if (buffer.IndexOf('\u201b') > -1) buffer = buffer.Replace('\u201b', '\'');
            if (buffer.IndexOf('\u201c') > -1) buffer = buffer.Replace('\u201c', '\"');
            if (buffer.IndexOf('\u201d') > -1) buffer = buffer.Replace('\u201d', '\"');
            if (buffer.IndexOf('\u201e') > -1) buffer = buffer.Replace('\u201e', '\"');
            if (buffer.IndexOf('\u2026') > -1) buffer = buffer.Replace("\u2026", "...");
            if (buffer.IndexOf('\u2032') > -1) buffer = buffer.Replace('\u2032', '\'');
            if (buffer.IndexOf('\u2033') > -1) buffer = buffer.Replace('\u2033', '\"');
            return buffer;
        }

        internal static string RemoveSpeaker(string line)
        {
            line = line.Trim();
            int colon = line.IndexOf(": ");
            if (colon >= 0 && colon < 30 && colon < line.Length - 2) return line.Substring(colon + 2);
            return line;
        }

        internal static string RemoveHTML(string line)
        {
            return line.Replace("<i>", "").Replace("</i>", "");
        }

        internal static int StringEditDistance(string s, string t)
        {
            int[,] d;
            return StringEditDistance(s, t, out d);
        }

        // source: http://www.dotnetperls.com/levenshtein
        internal static int StringEditDistance(string s, string t, out int[,] d)
        {
            int n = s.Length;
            int m = t.Length;
            d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1000;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }
    }
}
