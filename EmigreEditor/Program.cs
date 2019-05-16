using System;
using System.Windows.Forms;
using Emigre.Data;
using Emigre.Editor.Field;
using System.Collections.Generic;
using Emigre.Editor.Reflect;
using System.Collections;

namespace Emigre.Editor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Load();
            Application.Run(new MainForm());
        }

        private static void Load()
        {
            FieldEditor.RegisterEditor(typeof(JourneyPath), (a, l) => new PathEditor(a));
            FieldEditor.RegisterEditor(typeof(Location.MapPoint), (a, l) => new MapPointEditor(a, l));
            Story.Load();
            DataObjectEditor.SafeNamer = new EmigreSafeNamer();
        }


    }

    class EmigreSafeNamer : SafeNamer
    {
        public override string GetSafeName(object obj, bool hasChildren)
        {
            String name = base.GetSafeName(obj, hasChildren);
            if (hasChildren)
            {
                if (obj is IHasEvents)
                {
                    List<StoryEvent> events = (obj as IHasEvents).GetEvents();
                    name += AddNameChildren(events);
                }
                else if (obj is Json.DataObject)
                {
                    var accessors = FieldAccessor.GetForObject(obj);
                    foreach (Accessor accessor in accessors)
                    {
                        Type type = accessor.GetAccessorType();
                        if (DataObjectEditor.IsGenericList(type))
                        {
                            IList list = (IList)accessor.Get();
                            if (list == null) continue;
                            Type listOf = DataObjectEditor.GetFirstGenericType(list);
                            if (typeof(Outcome).IsAssignableFrom(listOf))
                            {
                                name += AddNameChildren(list);
                            }
                        }
                    }
                }
            }
            return name;
        }
    }
}
