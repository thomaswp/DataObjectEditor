using System;
using Emigre.Editor.Reflect;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Emigre.Editor.Field
{
    public class IntEditor : FieldEditor<int>
    {
        private NumericUpDown nud;

        public override bool EditsCanCombine
        {
            get
            {
                return true;
            }
        }

        public IntEditor(Accessor accessor)
            : base(accessor) 
        {
            nud = new NoScrollNumericUpDown();
            nud.Increment = 1;
            nud.Maximum = int.MaxValue;
            nud.Minimum = int.MinValue;
            nud.Width = 100;
            SetUIValue(GetValue());
            nud.ValueChanged += UpdateValue;
        }

        public override Control GetControl()
        {
            return nud;
        }

        public override int GetUIValue()
        {
            return (int) nud.Value;
        }

        public override void SetUIValue(int value)
        {
            nud.Value = value;
        }
    }

    class NoScrollNumericUpDown : NumericUpDown
    {
        protected override void WndProc(ref Message m)
        {
            // Send WM_MOUSEWHEEL messages to the parent
            if (m.Msg == 0x20a)
            {
                SendMessage(this.Parent.Handle, m.Msg, m.WParam, m.LParam);
                return;
            }
            base.WndProc(ref m);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
    }
}
