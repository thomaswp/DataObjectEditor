using Emigre.Data;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Emigre.Editor.Field
{
    public partial class PathEditorForm : Form
    {

        private JourneyPath _path = new JourneyPath();
        public JourneyPath Path
        {
            get { return _path; }
            set
            {
                _path = value;
                if (_path == null) _path = new JourneyPath();
                LoadMap();
            }
        }

        private int selectedIndex;
        private bool mouseDown;
        private Point lastClick;
        private Point lastMouseDown;
        private Point lastPanelScroll;

        public PathEditorForm()
        {
            InitializeComponent();

            pictureBox.Paint += pictureBox_Paint;
        }

        private void LoadMap()
        {
            pictureBox.Image = null;

            selectedIndex = -1;
            mouseDown = false;
            pictureBox.MinimumSize = new System.Drawing.Size(0, 0);
            string path = MainForm.ResourcesPath + Constants.DIR_IMAGES_MAPS + Path.map;
            try
            {
                if (File.Exists(path))
                {
                    Random random = new Random();
                    Bitmap bitmap = new Bitmap(path);
                    pictureBox.MinimumSize = new System.Drawing.Size(bitmap.Width, bitmap.Height);
                    pictureBox.Image = bitmap;
                }
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }

            comboBoxOp.SelectedIndex = 0;
            nudTransform.Value = 0;

            Redraw();
        }

        void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (pictureBox.Image == null) return;

            Graphics g = e.Graphics;
            Pen pen = new Pen(Color.Red, 2);
            Brush brush = new SolidBrush(Color.Red);
            Brush selectedBrush = new SolidBrush(Color.Orange);
            Path.Bound(0, 0, pictureBox.Image.Width, pictureBox.Image.Height);
            if (Path.points.Count > 1) g.DrawCurve(pen, Path.points.Select(point => new Point(point.x, point.y)).ToArray(), 0.5f);
            for (int i = 0; i < Path.points.Count; i++)
            {
                Point2D point = Path.points[i];
                g.FillEllipse(i == selectedIndex ? selectedBrush : brush, point.x - 5, point.y - 5, 10, 10);
            }
        }

        private void Redraw()
        {
            buttonInsert.Enabled = buttonDelete.Enabled = selectedIndex >= 0;
            pictureBox.Refresh();
        }

        private void ChooseMap()
        {
            openFileDialog.InitialDirectory = MainForm.ResourcesPath + Constants.DIR_IMAGES_MAPS.Replace("/", "\\");
            if (Path.map != null) openFileDialog.FileName = Path.map;
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Path.map = System.IO.Path.GetFileName(openFileDialog.FileName);
                LoadMap();
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void buttonMap_Click(object sender, EventArgs e)
        {
            ChooseMap();
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            if (Path.map == null)
            {
                ChooseMap();
                return;
            }
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastClick = new Point(e.X, e.Y);
            lastMouseDown = Cursor.Position;
            lastPanelScroll = new Point(panel.HorizontalScroll.Value, panel.VerticalScroll.Value);
            selectedIndex = -1;
            for (int i = 0; i < Path.points.Count; i++)
            {
                if (Path.points[i].Distance(e.X, e.Y) <= 7)
                {
                    selectedIndex = i;
                    break;
                }
            }
            Redraw();
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                if (selectedIndex >= 0)
                {
                    Path.points[selectedIndex] = new Point2D(e.X, e.Y);
                    Redraw();
                }
                else
                {
                    Point mousePoint = Cursor.Position;
                    int x = lastPanelScroll.X + lastMouseDown.X - mousePoint.X;
                    x = Math.Min(panel.HorizontalScroll.Maximum, Math.Max(panel.HorizontalScroll.Minimum, x));
                    panel.HorizontalScroll.Value = x;
                    int y = lastPanelScroll.Y + lastMouseDown.Y - mousePoint.Y;
                    y = Math.Min(panel.VerticalScroll.Maximum, Math.Max(panel.VerticalScroll.Minimum, y));
                    panel.VerticalScroll.Value = y;
                }
            }
            labelPoint.Text = "(" + e.X + "," + e.Y + ")";
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void buttonInsert_Click(object sender, EventArgs e)
        {
            if (selectedIndex >= 0) 
            {
                Point2D selected = Path.points[selectedIndex];
                Path.points.Insert(selectedIndex + 1, new Point2D(selected.x + 15, selected.y + 15));
                selectedIndex++;
                Redraw();
            }
        }

        private void pictureBox_DoubleClick(object sender, EventArgs e)
        {
            Path.points.Add(new Point2D(lastClick.X, lastClick.Y));
            selectedIndex = Path.points.Count - 1;
            Redraw();
        }

        private void DeletePoint()
        {
            if (selectedIndex >= 0)
            {
                Path.points.RemoveAt(selectedIndex);
                selectedIndex = -1;
                Redraw();
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            DeletePoint();
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            decimal value = nudTransform.Value;
            foreach (Point2D point in Path.points)
            {
                switch (comboBoxOp.SelectedIndex)
                {
                    case 0:
                        point.x = (int)(point.x * value);
                        point.y = (int)(point.y * value);
                        break;
                    case 1:
                        point.x += (int)value;
                        break;
                    case 2:
                        point.y += (int)value;
                        break;
                }
            }
            Redraw();
        }

        private void comboBoxOp_SelectedIndexChanged(object sender, EventArgs e)
        {
            nudTransform.DecimalPlaces = comboBoxOp.SelectedIndex == 0 ? 4 : 0;
        }
    }
}
