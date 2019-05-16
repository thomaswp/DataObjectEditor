using Emigre.Data;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Emigre.Editor.Field
{
    public partial class MapPointEditorForm : Form
    {
        private Location.MapPoint _point;
        public Location.MapPoint Point
        {
            get { return _point; }
            set
            {
                _point = value;
                Redraw();
            }
        }

        private City _city;
        public City City
        {
            get { return _city; }
            set
            {
                _city = value;
                LoadCity();
            }
        }

        private bool mouseDown;

        public MapPointEditorForm()
        {
            InitializeComponent();
        }

        private void LoadCity()
        {
            this.pictureBox.Image = null;
            if (City == null) return;
            string path = MainForm.ResourcesPath + Constants.DIR_IMAGES_MAPS + City.map;
            if (!File.Exists(path)) return;
            try
            {
                this.pictureBox.Image = new Bitmap(path);
            }
            catch { }
        }

        private void UpdatePoint(MouseEventArgs e)
        {
            if (this.pictureBox.Image == null) return;
            Size imageSize = this.pictureBox.Image.Size;
            Size boxSize = this.pictureBox.Size;
            float scaleX = (float)boxSize.Width / imageSize.Width;
            float scaleY = (float)boxSize.Height / imageSize.Height;
            if (scaleX > scaleY)
            {
                Point.Set((int)((e.X - boxSize.Width / 2) / scaleY + imageSize.Width / 2 + 0.5f), (int)(e.Y / scaleY + 0.5f));
            }
            else
            {
                Point.Set((int)(e.X / scaleX + 0.5f), (int)((e.Y - boxSize.Height / 2) / scaleX + imageSize.Height / 2 + 0.5f));
            }
            Redraw();
        }

        private void Redraw()
        {
            this.labelPoint.Text = Point.ToString();
            this.pictureBox.Refresh();
        }

        private void buttonOk_Click(object sender, EventArgs args)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (this.pictureBox.Image == null) return;
            Graphics g = e.Graphics;
            Size imageSize = this.pictureBox.Image.Size;
            Size boxSize = this.pictureBox.Size;
            float scaleX = (float)boxSize.Width / imageSize.Width;
            float scaleY = (float)boxSize.Height / imageSize.Height;
            PointF p;
            if (scaleX > scaleY)
            {
                p = new PointF((Point.x - imageSize.Width / 2) * scaleY + boxSize.Width / 2, Point.y * scaleY);
            }
            else
            {
                p = new PointF(Point.x * scaleX, (Point.y - imageSize.Height / 2) * scaleX + boxSize.Height / 2);
            }
            g.FillEllipse(Brushes.Red, p.X - 5, p.Y - 5, 10, 10);
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            UpdatePoint(e);
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown) UpdatePoint(e);
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }
    }
}
