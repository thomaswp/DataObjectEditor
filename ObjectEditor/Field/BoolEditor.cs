using Emigre.Editor.Reflect;
using System.Windows.Forms;

namespace Emigre.Editor.Field
{
    public class BoolEditor : FieldEditor<bool>
    {
        private CheckBox checkbox;

        public BoolEditor(Accessor accessor)
            : base(accessor) 
        {
            checkbox = new CheckBox();
            checkbox.Text = "";
            SetUIValue(GetValue());
            checkbox.CheckedChanged += UpdateValue;
        }

        public override Control GetControl()
        {
            return checkbox;
        }

        public override bool GetUIValue()
        {
            return checkbox.Checked;
        }

        public override void SetUIValue(bool value)
        {
            checkbox.Checked = value;
        }
    }
}
