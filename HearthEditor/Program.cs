using ObjectEditor.Editor;
using ObjectEditor.Editor.Field;
using System;
using System.Windows.Forms;
using HearthEditor.Data;

namespace HearthEditor
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
            //FieldEditor.RegisterEditor();
            Game.Load();
            //DataObjectEditor.SafeNamer = new HearthSafeNamer();
        }
    }
}
