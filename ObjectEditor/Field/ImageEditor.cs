using ObjectEditor.Editor.Reflect;
using ObjectEditor.Json;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ObjectEditor.Editor.Field
{
    public class ImageEditor : FieldEditor<string>
    {

        private PictureBox box;
        private OpenFileDialog openDialog;

        private string path;
        private string folder;
        
        public ImageEditor(Accessor accessor) : base(accessor) 
        {
            box = new PictureBox();
            box.Width = box.Height = 200;
            box.SizeMode = PictureBoxSizeMode.Zoom;
            box.BorderStyle = BorderStyle.FixedSingle;
            box.MouseClick += Box_MouseClick;
            folder = accessor.GetTags().Where((tag) => tag.flag == FieldTags.Image).First().arg;
            if (!(folder.EndsWith("/") | folder.EndsWith("\\"))) folder += "\\";
            folder = folder.Replace("/","\\");

            openDialog = new OpenFileDialog();
            openDialog.FileName = path;
            openDialog.InitialDirectory = MainForm.ResourcesPath;
            openDialog.Filter = "Image Files | *.bmp; *.png; *.jpg; *.jpeg | All Files | *.*";

            SetUIValue(GetValue());
        }

        private void Box_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    SetUIValue(Path.GetFileName(openDialog.FileName));
                    UpdateValue(null, null);
                }
            }
            else
            {
                SetUIValue("");
                UpdateValue(null, null);
            }
        }

        public override Control GetControl()
        {
            return box;
        }

        public override string GetUIValue()
        {
            return path;
        }

        public override void SetUIValue(string value)
        {
            if (value == this.path) return;
            this.path = value;
            box.Image = null;
            string fullPath = MainForm.ResourcesPath + folder + value;
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
