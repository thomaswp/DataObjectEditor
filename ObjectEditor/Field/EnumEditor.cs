using Emigre.Editor.Reflect;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Emigre.Editor.Field
{
    public class EnumEditor : FieldEditor<Enum>
    {
        private ComboBox combobox;
        private Enum[] values;

        public EnumEditor(Accessor accessor)
            : base(accessor) 
        {
            combobox = new NoScrollComboBox();
            values = Enum.GetValues(accessor.GetAccessorType()).Cast<Enum>().ToArray();
            foreach (Enum item in values)
            {
                combobox.Items.Add(item.ToString());
            }
            combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            combobox.Width = 200;
            SetUIValue(GetValue());
            combobox.SelectedIndexChanged += UpdateValue;
        }

        public override Control GetControl()
        {
            return combobox;
        }

        public override Enum GetUIValue()
        {
            return values[combobox.SelectedIndex];
        }

        public override void SetUIValue(Enum value)
        {
            combobox.Text = value.ToString();
        }
    }

    class NoScrollComboBox : ComboBox
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
