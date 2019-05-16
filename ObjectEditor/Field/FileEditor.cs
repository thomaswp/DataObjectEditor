using Emigre.Editor.Reflect;
using Emigre.Json;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Emigre.Editor.Field
{
    public class FileEditor : FieldEditor<string>
    {

        private Button button;
        private OpenFileDialog openDialog;

        private string folder;

        public FileEditor(Accessor accessor)
            : base(accessor) 
        {
            button = new Button();
            button.MinimumSize = new Size(75, button.Height);
            button.AutoSize = true;
            button.Click += button_Click;
            folder = accessor.GetTags().Where((tag) => tag.flag == FieldTags.File).First().arg;

            openDialog = new OpenFileDialog();
            openDialog.InitialDirectory = Path.GetFullPath(MainForm.ResourcesPath + folder.Replace("/","\\"));
            openDialog.Filter = "All Files | *.*";

            SetUIValue(GetValue());
        }

        void button_Click(object sender, EventArgs e)
        {
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                SetUIValue(Path.GetFileName(openDialog.FileName));
                UpdateValue(null, null);
            }
        }

        public override Control GetControl()
        {
            return button;
        }

        public override string GetUIValue()
        {
            return button.Text;
        }

        public override void SetUIValue(string value)
        {
            openDialog.FileName = value;
            button.Text = value;
        }
    }
}
