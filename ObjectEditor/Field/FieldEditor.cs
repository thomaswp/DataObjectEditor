using System;
using System.Collections.Generic;
using System.Linq;
using Emigre.Editor.Reflect;
using System.Windows.Forms;
using Emigre.Data;
using Emigre.Json;

namespace Emigre.Editor.Field
{
    public abstract class FieldEditor
    {
        public abstract Control GetControl();
        public abstract void Refresh();

        public readonly Accessor accessor;
        public readonly bool needsRefresh;

        public event EventHandler<FieldEditedEventArgs> OnEdited;

        private static Dictionary<Type, Func<Accessor, DataObjectEditor, FieldEditor>> fieldEditors = new Dictionary<Type, Func<Accessor, DataObjectEditor, FieldEditor>>();

        public virtual int SelectedIndex
        {
            get
            {
                return -1;
            }
            set { }
        }

        public virtual int LastSelectedIndex
        {
            get
            {
                return SelectedIndex;
            }
        }

        public virtual bool EditsCanCombine { get { return false; } }

        static FieldEditor()
        {
            fieldEditors.Add(typeof(string), (a, l) => new StringEditor(a));
            fieldEditors.Add(typeof(int), (a, l) => new IntEditor(a));
            fieldEditors.Add(typeof(float), (a, l) => new FloatEditor(a));
            fieldEditors.Add(typeof(bool), (a, l) => new BoolEditor(a));
            fieldEditors.Add(typeof(JourneyPath), (a, l) => new PathEditor(a));
            fieldEditors.Add(typeof(Location.MapPoint), (a, l) => new MapPointEditor(a, l));
            fieldEditors.Add(typeof(Guid), (a, l) => null);
        }

        protected virtual void SetValueImpl(object value)
        {
            accessor.Set(value);
        }

        protected virtual object GetValueImpl()
        {
            return accessor.Get();
        }

        public virtual void SetValue(object value, bool fromUI = true)
        {
            object oldValue = GetValueImpl();
            if (value == oldValue) return;
            SetValueImpl(value);
            if (fromUI && OnEdited != null) OnEdited(null, new FieldEditedEventArgs(oldValue, value));
        }

        public FieldEditor(Accessor accessor)
        {
            this.accessor = accessor;
            this.needsRefresh = accessor.GetTags().Any(tag => tag.flag == FieldTags.Refresh);
        }

        public static FieldEditor CreateEditor(Accessor accessor, DataObjectEditor lookup)
        {
            if (accessor.GetTags().Any(tag => tag.flag == FieldTags.Hide)) return null;
            Type type = accessor.GetAccessorType();
            if (type.IsEnum) return new EnumEditor(accessor);
            if (accessor.GetAccessorType() == typeof(string) && accessor.GetTags().Any(tag => tag.flag == FieldTags.Image))
            {
                return new ImageEditor(accessor);
            }
            if (accessor.GetAccessorType() == typeof(string) && accessor.GetTags().Any(tag => tag.flag == FieldTags.File))
            {
                return new FileEditor(accessor);
            }
            if (typeof(Reference).IsAssignableFrom(accessor.GetAccessorType()))
            {
                return new ReferenceEditor(accessor, lookup);
            }
            if (fieldEditors.ContainsKey(type)) return fieldEditors[type](accessor, lookup);
            Console.WriteLine("No editor for type " + type.Name);
            return null;
        }
    }

    public abstract class FieldEditor<T> : FieldEditor
    {

        public abstract T GetUIValue();
        public abstract void SetUIValue(T value);

        public FieldEditor(Accessor accessor) : base(accessor) { }

        private bool updatingUI;

        public override void Refresh()
        {
            SetUIValue(GetValue());
        }

        public override void SetValue(object value, bool fromUI = true)
        {
            base.SetValue(value, fromUI && !updatingUI);
            if (!updatingUI && !fromUI)
            {
                updatingUI = true;
                SetUIValue((T)value);
                updatingUI = false;
            }
        }

        protected void UpdateValue(object sender, EventArgs e)
        {
            SetValue(GetUIValue());
        }

        public T GetValue()
        {
            return (T) GetValueImpl();
        }
    }

    public class FieldEditedEventArgs : EventArgs
    {
        public readonly object valueBefore, valueAfter;

        public FieldEditedEventArgs(object valueBefore, object valueAfter)
        {
            this.valueBefore = valueBefore;
            this.valueAfter = valueAfter;
        }
    }
}
