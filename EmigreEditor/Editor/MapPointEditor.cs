using Emigre.Data;
using ObjectEditor.Editor;
using ObjectEditor.Editor.Field;
using ObjectEditor.Editor.Reflect;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Emigre.Editor.Field
{
    public class MapPointEditor : FieldEditor<Location.MapPoint>
    {
        private readonly Button button;
        private readonly MapPointEditorForm editor = new MapPointEditorForm();
        private readonly DataObjectEditor lookup;

        public MapPointEditor(Accessor accessor, DataObjectEditor lookup)
            : base(accessor)
        {
            this.lookup = lookup;
            button = new Button();
            button.Click += button_Click;
            SetUIValue(GetValue());
        }

        void button_Click(object sender, EventArgs e)
        {
            City parent = lookup.ListObjects(typeof(City)).Cast<City>().Where(
                city => city.locations.Contains(lookup.DataObject)).FirstOrDefault();
            editor.City = parent;

            editor.Point = ObjectEditor.Json.JsonSerializer.copy(GetValue());
            editor.StartPosition = FormStartPosition.CenterParent;
            if (editor.ShowDialog() == DialogResult.OK)
            {
                SetValue(editor.Point);
                SetUIValue(editor.Point);
            }
        }

        public override Location.MapPoint GetUIValue()
        {
            return GetValue();
        }

        public override void SetUIValue(Location.MapPoint value)
        {
            button.Text = value.ToString();
        }

        public override System.Windows.Forms.Control GetControl()
        {
            return button;
        }
    }
}
