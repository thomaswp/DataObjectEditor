using ObjectEditor.Editor.Reflect;
using ObjectEditor.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ObjectEditor.Editor.Field
{
    public class ReferenceEditor : FieldEditor<Guid>
    {
        private ComboBox comboBox;
        private List<GuidDataObject> items;
        private Reference reference;
        private Lookup lookup;
        private Type type;
        private JsonSerializer.MapContext context = new JsonSerializer.MapContext();

        public ReferenceEditor(Accessor accessor, Lookup lookup)
            : base(accessor)
        {
            this.lookup = lookup;
            this.type = DataObjectEditor.GetFirstGenericType(accessor.GetAccessorType());

            comboBox = new NoScrollComboBox();
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.Width = 200;

            reference = (Reference)accessor.Get();
            if (reference != null) reference.Context = context;
            else
            {
                throw new Exception("Reference not initialized");
            }

            SetUIValue(GetValue());
            comboBox.SelectedValueChanged += UpdateValue;
        }

        public override Guid GetUIValue()
        {
            GuidDataObject obj = comboBox.SelectedIndex < 0 ? null : items[comboBox.SelectedIndex];
            return obj == null ? new Guid() : obj.GetGuid();   
        }

        public override void SetUIValue(Guid value)
        {
            items = lookup.ListObjects(type).ToList();
            items.Insert(0, null);
            comboBox.Items.Clear();
            comboBox.Items.Add("");
            comboBox.SelectedIndex = 0;
            if (reference == null) return;
            context.map.Clear();
            for (int i = 1; i < items.Count; i++)
            {
                if (context.map.ContainsKey(items[i].GetGuid())) continue;
                context.Add(items[i]);
                comboBox.Items.Add(items[i].ToString());
                if (items[i].GetGuid() == value)
                {
                    comboBox.SelectedIndex = i;
                }
            }
        }

        protected override object GetValueImpl()
        {
            Reference reference = accessor.Get() as Reference;
            if (reference == null) return Guid.Empty;
            return reference.Guid;
        }

        protected override void SetValueImpl(object value)
        {
            if (value == null) return;
            ((Reference)accessor.Get()).Guid = (Guid)value;
        }

        public override System.Windows.Forms.Control GetControl()
        {
            return comboBox;
        }
    }

    public interface Lookup
    {
        IEnumerable<GuidDataObject> ListObjects(Type type);
    }
}
