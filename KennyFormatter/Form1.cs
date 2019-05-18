using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace KennyFormatter
{
    public enum Centering
    {
        Min, Center, Max
    }
    
    public partial class Form1 : Form
    {
        const string dir = @"C:\Users\Thomas\Desktop\kenney_natureKit\Isometric\";
        const string outDir = dir + @"resized\";
        const int width = 149;
        const int height = 107;
        const string compare = "ground_dirt_NE.png";

        Image compareImage, loadedImage;
        Centering xc = Centering.Center, yc = Centering.Center;

        public Form1()
        {
            InitializeComponent();

            foreach (Control child in tableLayoutPanelCentering.Controls)
            {
                if (child is Button)
                {
                    child.Click += centeringButton_Clicked;
                }
            }
        }

        private void centeringButton_Clicked(object sender, EventArgs e)
        {
            var pos = tableLayoutPanelCentering.GetCellPosition((Control)sender);
            xc = (Centering)pos.Column;
            yc = (Centering)(2 - pos.Row);
            foreach (Control child in tableLayoutPanelCentering.Controls)
            {
                ((Button)child).BackColor = Control.DefaultBackColor;
            }
            ((Button)sender).BackColor = Color.AliceBlue;
            refreshOffset();
            refreshImage();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (string file in Directory.GetFiles(dir))
            {
                comboBoxFiles.Items.Add(Path.GetFileName(file));
            }
            compareImage = Image.FromFile(dir + compare);
            comboBoxFiles.SelectedIndex = 0;
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            if (comboBoxFiles.SelectedIndex > 0) comboBoxFiles.SelectedIndex--;
        }

        private void buttonForward_Click(object sender, EventArgs e)
        {
            if (comboBoxFiles.SelectedIndex < comboBoxFiles.Items.Count - 1) comboBoxFiles.SelectedIndex++;
        }

        private void comboBoxFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxFiles.SelectedIndex < 0) return;
            loadedImage = Image.FromFile(dir + comboBoxFiles.Items[comboBoxFiles.SelectedIndex]);
            refreshOffset();
            refreshImage();
        }

        private void checkBoxDrawCenter_CheckedChanged(object sender, EventArgs e)
        {
            refreshImage();
        }

        private void refreshOffset()
        {
            nudOffX.Maximum = Math.Max(width - loadedImage.Width, 0);
            nudOffX.Minimum = Math.Min(0, width - loadedImage.Width);
            if (xc == Centering.Min) nudOffX.Value = 0;
            else if (xc == Centering.Center) nudOffX.Value = width / 2 - loadedImage.Width / 2;
            else nudOffX.Value = nudOffX.Maximum;
            
            if (yc == Centering.Min) nudOffY.Value = 0;
            else if (yc == Centering.Center) nudOffY.Value = Math.Max(0, height / 2 - loadedImage.Height / 2);
            else nudOffY.Value = Math.Max(0, height - loadedImage.Height);

        }

        private void refreshImage()
        {
            int offX = (int) nudOffX.Value;
            int offY = (int) nudOffY.Value;

            Bitmap bmp = new Bitmap(width * 2, height * 2);
            Graphics g = Graphics.FromImage(bmp);
            g.DrawRectangle(Pens.Black, 0, 0, width * 2 - 1, height * 2 - 1);
            drawImage(width / 2, height / 2, g, offX, offY, loadedImage);
            drawImage(3 * width / 2, height / 2, g, offX, offY, loadedImage);

            //create a color matrix object  
            ColorMatrix matrix = new ColorMatrix();
            //set the opacity  
            matrix.Matrix33 = 0.5f;
            //create image attributes  
            ImageAttributes attributes = new ImageAttributes();
            //set the color(opacity) of the image  
            attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);


            drawImage(width, height, g, 0, 0, compareImage, attributes);
            if (checkBoxDrawCenter.Checked) drawImage(width, height, g, offX, offY, loadedImage);
            drawImage(width / 2, 3 * height / 2, g, offX, offY, loadedImage);
            drawImage(3 * width / 2, 3 * height / 2, g, offX, offY, loadedImage);

            pictureBox.Image = bmp;
        }

        private void nudOffX_ValueChanged(object sender, EventArgs e)
        {
            refreshImage();
        }

        private void nudOffY_ValueChanged(object sender, EventArgs e)
        {
            refreshImage();
        }

        private void buttonWidthSave_Click(object sender, EventArgs e)
        {
            Directory.CreateDirectory(dir + "rewidth");
            foreach (string file in Directory.GetFiles(dir))
            {
                Bitmap bmp = new Bitmap(file);
                Bitmap convert = new Bitmap(width, bmp.Height);
                Graphics g = Graphics.FromImage(convert);
                g.DrawImage(bmp, width / 2 - bmp.Width / 2, 0);
                convert.Save(dir + "rewidth/" + Path.GetFileName(file));
            }
        }

        private void drawImage(int cx, int cy, Graphics g, int offX, int offY, Image image, ImageAttributes attrs = null)
        {
            if (attrs == null) attrs = new ImageAttributes();
            g.DrawImage(image, 
                new Rectangle(cx - width / 2 + offX, cy + height / 2 - image.Height - offY, image.Width, image.Height),
                0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attrs);
            g.FillEllipse(Brushes.Black, cx - 2, cy - 2, 4, 4);
        }

        private int topLeftPx(int center, int size, int extent, Centering centering)
        {
            if (centering == Centering.Center)
                return center - size / 2;
            else if (centering == Centering.Min)
                return center - extent / 2;
            else
                return center + extent / 2 - size;

        }
    }
}
