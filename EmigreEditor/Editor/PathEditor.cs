using Emigre.Data;
using ObjectEditor.Editor;
using ObjectEditor.Editor.Field;
using ObjectEditor.Editor.Reflect;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Emigre.Editor.Field
{
    public class PathEditor : FieldEditor<JourneyPath>
    {

        private PathEditorForm editor = new PathEditorForm();
        private PictureBox box;

        private string folder;

        public PathEditor(Accessor accessor)
            : base(accessor) 
        {
            box = new PictureBox();
            box.Width = box.Height = 200;
            box.SizeMode = PictureBoxSizeMode.Zoom;
            box.BorderStyle = BorderStyle.FixedSingle;
            box.Click += box_Click;
            folder = Constants.DIR_IMAGES_MAPS;

            SetUIValue(GetValue());
        }

        void box_Click(object sender, EventArgs e)
        {
            editor.Path = ObjectEditor.Json.JsonSerializer.copy(GetValue());
            editor.StartPosition = FormStartPosition.CenterParent;
            if (editor.ShowDialog() == DialogResult.OK)
            {
                SetValue(editor.Path);
                SetUIValue(editor.Path);
            }
        }

        public override Control GetControl()
        {
            return box;
        }

        public override JourneyPath GetUIValue()
        {
            return GetValue();
        }

        public override void SetUIValue(JourneyPath value)
        {
            box.Image = null;
            string fullPath = MainForm.ResourcesPath + folder + value.map;
            if (File.Exists(fullPath))
            {
                try
                {
                    box.Image = new Bitmap(fullPath);
                }
                catch
                {

                }
            }
        }
    }
}
