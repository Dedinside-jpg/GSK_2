using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSK_2
{
    public partial class Form1 : Form
    {
        Graphics g;
        Pen DrawPen = new Pen(Color.Black, 1);
        int PaintType = 0; // Код типа закрашивания
        List<Point> VertexList = new List<Point>(); // Список вершин многоугольника

        public Form1()
        {
            InitializeComponent();
            g = pictureBox1.CreateGraphics(); //инициализация графики

        }
        //Обработчик события выбора цвета
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox2.SelectedIndex) // выбор цвета
            {
                case 0:
                    DrawPen.Color = Color.Black;
                    break;
                case 1:
                    DrawPen.Color = Color.Red;
                    break;
                case 2:
                    DrawPen.Color = Color.Green;
                    break;
                case 3:
                    DrawPen.Color = Color.Blue;
                    break;
            }
        }
        //Очистка окна
        private void button1_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            VertexList.Clear();
        }
        //Обработчик события выбора типа закрашивания
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            PaintType = comboBox1.SelectedIndex;
        }
        //Обработчик события
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {

            VertexList.Add(new Point() { X = e.X, Y = e.Y });
            g.DrawEllipse(DrawPen, e.X - 2, e.Y - 2, 5, 5);

            if (e.Button == MouseButtons.Right)
            {
                if (VertexList.Count() < 3)
                {
                    MessageBox.Show("Слишком мало точек, добавьте ещё", "Предупреждение");
                    VertexList.Clear();
                }

                else
                {
                    if (PaintType == 0)
                    {
                        FirstPicture(DrawPen, VertexList);
                    }

                    else
                    {
                        SecondPicture(DrawPen, VertexList);
                    }

                    VertexList.Clear();
                }
            }

        }

        //Прорисовка не ориентированная
        public void FirstPicture(Pen DrPen, List<Point> mass)
        {
            List<int> Xb = new List<int>();
            int n = mass.Count();
            
            bool CW = false;
            int Ymin, Ymax, Yemax, Y;
            Ymin = mass[0].Y;
            Ymax = mass[0].Y;
            
            Yemax = pictureBox1.ClientRectangle.Height;

            for (int i = 1; i < mass.Count(); i++)
            {
                if (Ymin > mass[i].Y) { Ymin = mass[i].Y; }
                if (Ymax < mass[i].Y) { Ymax = mass[i].Y; }
            }

            Ymin = Math.Max(Ymin, 0);
            Ymax = Math.Min(Ymax, Yemax);
            Y = Ymin;
            
            do
            {
                Xb.Clear();
                int k = 0;

                for (int i = 0; i < n; i++)
                {
                    if (i < n - 1) k = i + 1;
                    else k = 0;

                    if (((mass[i].Y < Y) && (mass[k].Y >= Y)) || ((mass[i].Y >= Y) && (mass[k].Y < Y)))
                    {
                        int Xx = (mass[k].X - mass[i].X) * (Y - mass[i].Y) / (mass[k].Y - mass[i].Y) + mass[i].X;
                        Xb.Add(Xx);
                    }
                }

                if (CW)
                {
                    Xb.Add(0);
                }

                Xb.Sort();
                for (int i = 0; i < Xb.Count-1; i+=2)
                {
                        g.DrawLine(DrPen, Xb[i], Y, Xb[i+1], Y);
                }
                Y++;
            }
            while (Y != Ymax);

            if (CW)
            {
                for (int y = Ymax; y <= Yemax; y++)
                {
                    g.DrawLine(DrPen, 0, y, pictureBox1.ClientRectangle.Width, y);
                }
            }
        }

        //Прорисовка ориентированная
        public void SecondPicture(Pen DrPen, List<Point> Dots)
        {
            List<int> Xl = new List<int>();
            List<int> Xr = new List<int>();
            int n = Dots.Count();
            int indx;
            bool CW = false;
            int Ymin, Ymax, Yemax, Y;
            Ymin = Dots[0].Y;
            Ymax = Dots[0].Y;
            indx = 0;
            Yemax = pictureBox1.ClientRectangle.Height;

            for (int i = 1; i < Dots.Count(); i++)
            {
                if (Ymin > Dots[i].Y) { Ymin = Dots[i].Y; }
                if (Ymax < Dots[i].Y) { Ymax = Dots[i].Y; indx = i; }
            }

            Ymin = Math.Max(Ymin, 0);
            Ymax = Math.Min(Ymax, Yemax);
            Y = Ymin;

            int indxp;
            int indxm;
            if (indx < n - 1) indxp = indx + 1;
            else indxp = 0;

            if (indx != 0) indxm = indx - 1;
            else indxm = n - 1;

            //Внутреннее закрашивание
            if (Dots[indxp].X < Dots[indxm].X) CW = true;

            if (CW)
            {
                for (int y = 0; y <= Ymin; y++)
                {
                    g.DrawLine(DrPen, 0, y, pictureBox1.ClientRectangle.Width, y);//закрашивания от верхней точки вверх
                }
            }

            do
            {
                Xl.Clear();
                Xr.Clear();
                int k = 0;

                for (int i = 0; i < n; i++)
                {
                    if (i < n - 1) k = i + 1;
                    else k = 0;

                    if (((Dots[i].Y < Y) && (Dots[k].Y >= Y)) || ((Dots[i].Y >= Y) && (Dots[k].Y < Y)))
                    {
                        int Xx = (Dots[k].X - Dots[i].X) * (Y - Dots[i].Y) / (Dots[k].Y - Dots[i].Y) + Dots[i].X;
                        if (Dots[k].Y - Dots[i].Y > 0) Xr.Add(Xx);
                        else Xl.Add(Xx);
                    }
                }

                if (CW)
                {
                    Xr.Add(0);
                    Xl.Add(pictureBox1.ClientRectangle.Width);
                }

                Xl.Sort();
                Xr.Sort();
                for (int i = 0; i < Xl.Count; i++)
                {
                    if (Xl[i] > Xr[i]) //Внешннее закрашивание
                    {
                        g.DrawLine(DrPen, Xl[i], Y, Xr[i], Y);
                    }
                }
                Y++;
            }
            while (Y != Ymax);

            if (CW)
            {
                for (int y = Ymax; y <= Yemax; y++)
                {
                    g.DrawLine(DrPen, 0, y, pictureBox1.ClientRectangle.Width, y);//Закрашивание от нижней точки вниз
                }
            }

        }
    }
}
