using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Emigre.Editor.Script
{
    public partial class DiffForm : Form
    {

        private string originalText;

        public string NewText
        {
            get { return this.textBoxNew.Text; }
        }

        public DiffForm()
        {
            InitializeComponent();
        }

        public void Init(string title, string compare, string edit)
        {
            this.Text = title;
            this.textBoxOriginal.Text = compare;
            this.textBoxNew.Text = this.originalText = edit;
        }

        private void textBoxNew_TextChanged(object sender, EventArgs e)
        {
            string s = this.textBoxOriginal.Text, t = ScriptCorrector.RemoveHTML(this.textBoxNew.Text);
            List<string> diff = Diff(s, t);
            this.richTextBoxCompare.Text = "";
            foreach (string d in diff)
            {
                string action = d.Substring(0, 1);
                string text = d.Substring(1);
                Color color = Color.Black;
                if (action == "+") color = Color.DarkGreen;
                else if (action == "-") color = Color.DarkRed;
                AppendText(text, color, color != Color.Black);
            }

            this.richTextBoxCompare.BackColor = (s == t) ? SystemColors.ControlLight : SystemColors.Control;
            this.buttonOk.Text = (originalText == this.textBoxNew.Text) ? "Next" : "OK";
        }

        private void AppendText(string text, Color color, bool underline)
        {
            RichTextBox box = this.richTextBoxCompare;

            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            Font f = new Font(box.SelectionFont, underline ? FontStyle.Underline : FontStyle.Regular);
            box.SelectionFont = f;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }

        internal static List<string> Diff(string s, string t)
        {
            int[,] d;
            ScriptCorrector.StringEditDistance(s, t, out d);

            List<string> list = new List<string>();

            string current = "";
            string currentOp = "=";

            int i = d.GetLength(0) - 1, j = d.GetLength(1) - 1;
            while (i > 0 && j > 0)
            {
                int del = d[i - 1, j], ins = d[i, j - 1], same = d[i - 1, j - 1];
                if (s[i - 1] != t[j - 1]) same += 1000;

                if (currentOp == "=" && same <= ins && same <= del)
                {
                    i--; j--;
                    if (s[i] != t[j])
                    {

                    }
                    current = s[i] + current;
                    continue;
                }
                if (currentOp == "+" && ins <= same && ins <= del)
                {
                    j--;
                    current = t[j] + current;
                    continue;
                }
                if (currentOp == "-" && del <= ins && del <= same)
                {
                    i--;
                    current = s[i] + current;
                    continue;
                }

                if (current.Length > 0)
                {
                    list.Insert(0, currentOp + current);
                    current = "";
                }

                if (same <= del && same <= ins)
                {
                    i--; j--;
                    currentOp = "=";
                    if (s[i] != t[j])
                    {

                    }
                    current = s[i] + current;
                    continue;
                }
                if (ins <= del && ins <= same)
                {
                    j--;
                    currentOp = "+";
                    current = t[j] + current;
                    continue;
                }

                i--;
                currentOp = "-";
                current = s[i] + current;
                continue;
            }

            while (i > 0)
            {
                if (currentOp != "-")
                {
                    list.Insert(0, currentOp + current);
                    current = "";
                    currentOp = "-";
                }
                current = s[--i] + current;
            }

            while (j > 0)
            {
                if (currentOp != "+")
                {
                    list.Insert(0, currentOp + current);
                    current = "";
                    currentOp = "+";
                }
                current = t[--j] + current;
            }

            if (current.Length > 0)
            {
                list.Insert(0, currentOp + current);
            }

            return list;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = (NewText == originalText) ? DialogResult.Cancel : DialogResult.OK;
            Close();
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            this.textBoxNew.Text = originalText;
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Abort;
            Close();
        }

        private void buttonAccept_Click(object sender, EventArgs e)
        {
            this.textBoxNew.Text = this.textBoxOriginal.Text;
        }

        private void textBoxNew_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Enter)
            {
                buttonOk_Click(null, null);
            }
        }
    }
}
