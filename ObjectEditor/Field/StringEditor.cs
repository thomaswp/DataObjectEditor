using System;
using System.Linq;
using ObjectEditor.Editor.Reflect;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ObjectEditor.Editor.Field
{
    public class StringEditor : FieldEditor<string>
    {
        private NoScrollTextbox textbox;
        private int selection0, selection1;
        
        public StringEditor(Accessor accessor) : base(accessor) 
        {
            textbox = new NoScrollTextbox();
            textbox.Width = 350;
            if (accessor.GetTags().Any((tag) => tag.flag == ObjectEditor.Json.FieldTags.Multiline))
            {
                textbox.Multiline = true;
                textbox.ScrollBars = ScrollBars.Vertical;
                textbox.Height = 150;
            }
            else
            {
                textbox.Multiline = false;
                textbox.Height = new TextBox().Height;
            }
            SetUIValue(GetValue());
            textbox.KeyDown += UpdateSelected;
            textbox.KeyPress += UpdateSelected;
            textbox.GotFocus += UpdateSelected;
            textbox.MouseDown += UpdateSelected;
            textbox.TextChanged += textbox_TextChanged;
            
            textbox.KeyDown += Textbox_KeyDown;
        }

        private void Textbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                textbox.SelectAll();
                UpdateSelected(textbox, null);
                e.SuppressKeyPress = true;
            }
        }

        void UpdateSelected(object sender, EventArgs e)
        {
            if (selection0 == SelectedIndex) return;
            selection1 = selection0; // this seems not to be needed, but I'm not removing it yet
            selection0 = SelectedIndex;
        }

        private void textbox_TextChanged(object sender, EventArgs e)
        {
            _lastSelectedIndex = selection0;
            UpdateValue(sender, e);
        }

        public override Control GetControl()
        {
            return textbox;
        }

        public override string GetUIValue()
        {
            return textbox.Text;
        }

        public override int SelectedIndex
        {
            get
            {
                return textbox.SelectionStart + textbox.SelectionLength;
            }
            set
            {
                textbox.SelectionStart = value;
                textbox.SelectionLength = 0;
            }
        }

        private int _lastSelectedIndex;
        public override int LastSelectedIndex
        {
            get
            {
                return _lastSelectedIndex;
            }
        }

        public override bool EditsCanCombine
        {
            get
            {
                return true;
            }
        }

        private class NoScrollTextbox : TextBox
        {
            protected override void WndProc(ref Message m)
            {
                // Send WM_MOUSEWHEEL messages to the parent
                if (m.Msg == 0x20a && TextRenderer.MeasureText(this.Text, this.Font, this.Size).Height <= this.Height)
                {
                    SendMessage(this.Parent.Handle, m.Msg, m.WParam, m.LParam);
                    return;
                }
                base.WndProc(ref m);
            }

            [DllImport("user32.dll")]
            private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
        }

        public override void SetUIValue(string value)
        {
            textbox.Text = value == null ? "" : value;
        }
    }
}
