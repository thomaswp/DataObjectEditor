using Emigre.Editor.Reflect;
using System.Windows.Forms;

namespace Emigre.Editor.Field
{
    public class FloatEditor : FieldEditor<float>
    {
        private NumericUpDown nud;

        public override bool EditsCanCombine
        {
            get
            {
                return true;
            }
        }

        public FloatEditor(Accessor accessor)
            : base(accessor) 
        {
            nud = new NoScrollNumericUpDown();
            nud.Increment = 1;
            nud.DecimalPlaces = 3;
            nud.Maximum = decimal.MaxValue;
            nud.Minimum = decimal.MinValue;
            nud.Width = 100;
            SetUIValue(GetValue());
            nud.ValueChanged += UpdateValue;
        }

        public override Control GetControl()
        {
            return nud;
        }

        public override float GetUIValue()
        {
            return (float) nud.Value;
        }

        public override void SetUIValue(float value)
        {
            nud.Value = (decimal)value;
        }
    }
}
