using System;
using System.Drawing;

namespace MyDrawLib
{
    public class MyLine
    {
        private PointF start;
        private PointF end;

        public PointF Start { get { return start; } }
        public PointF End { get { return end; } }
        public MyLine(float x1, float y1, float x2, float y2)
        {
            this.start.X = x1;
            this.end.X = x2;

            this.start.Y = y1;
            this.end.Y = y2;
        }

        public void BresenhamFloat(Graphics g, Color color)
        {
            int step;

            float dx = end.X - start.X;
            float dy = end.Y - start.Y;

            int signDX = sign(dx);
            int signDY = sign(dy);

            dx = Math.Abs(dx);
            dy = Math.Abs(dy);

            if (dy >= dx)
            {
                float buf = dx;
                dx = dy;
                dy = buf;

                step = 1;
            }
            else
            {
                step = 0;
            }

            float tg = dy / dx;
            float e = tg - 1 / 2;


            int xi = (int)Math.Round(start.X);
            int yi = (int)Math.Round(start.Y);

            while (Math.Abs(xi - end.X) >= 1 || Math.Abs(yi - end.Y) >= 1)
            {
                PutPixel(g, xi, yi, color);

                if (e >= 0)
                {
                    if (step == 1)
                    {
                        xi += signDX;
                    }
                    else
                    {
                        yi += signDY;
                    }

                    e -= 1;
                }
                else
                {
                    if (step == 0)
                    {
                        xi += signDX;
                    }
                    else
                    {
                        yi += signDY;
                    }

                    e += tg;
                }
            }
        }

        public void BresenhamInt(Graphics g, Color color)
        {
            int step;

            float dx = end.X - start.X;
            float dy = end.Y - start.Y;

            int signDX = sign(dx);
            int signDY = sign(dy);

            dx = Math.Abs(dx);
            dy = Math.Abs(dy);

            if (dy >= dx)
            {
                float buf = dx;
                dx = dy;
                dy = buf;

                step = 1;
            }
            else
            {
                step = 0;
            }

            float e = 2 * dy - dx;
            float dex = 2 * dx;
            float dey = 2 * dy;


            int xi = (int)Math.Round(start.X);
            int yi = (int)Math.Round(start.Y);


            while (Math.Abs(xi - end.X) >= 1 || Math.Abs(yi - end.Y) >= 1)
            {
                PutPixel(g, xi, yi, color);

                if (e >= 0)
                {
                    if (step == 1)
                    {
                        xi += signDX;
                    }
                    else
                    {
                        yi += signDY;
                    }

                    e -= dex;
                }
                else
                {
                    if (step == 0)
                    {
                        xi += signDX;
                    }
                    else
                    {
                        yi += signDY;
                    }

                    e += dey;
                }
            }
        }

        public void BresenhamSmooth(Graphics g, Color color)
        {
            int I = 255;
            int step;

            float dx = end.X - start.X;
            float dy = end.Y - start.Y;

            int signDX = sign(dx);
            int signDY = sign(dy);

            dx = Math.Abs(dx);
            dy = Math.Abs(dy);

            if (dy >= dx)
            {
                float buf = dx;
                dx = dy;
                dy = buf;

                step = 1;
            }
            else
            {
                step = 0;
            }

            float tg = I * dy / dx; // умножаем на инт. чтобы не умножать в цикле

            float e = I / 2;

            float w = I - tg;

            int xi = (int)Math.Round(start.X);
            int yi = (int)Math.Round(start.Y);

            PutPixel(g, xi, yi, 127, color);

            for (int i = 1; i < dx + 1; i++)
            {
                if (e < w)
                {
                    if (step == 0)
                    {
                        xi += signDX;
                    }
                    else
                    {
                        yi += signDY;
                    }

                    e += tg;
                }
                else
                {
                    xi += signDX;
                    yi += signDY;
                    e -= w;
                }

                PutPixel(g, xi, yi, (int)(e), color);
            }
        }

        public void CDA(Graphics g, Color color)
        {
            float dx = end.X - start.X;
            float dy = end.Y - start.Y;

            float l;
            if (Math.Abs(dx) > Math.Abs(dy))
            {
                l = Math.Abs(dx);
            }
            else
            {
                l = Math.Abs(dy);
            }

            dx = dx / l;
            dy = dy / l;

            float xi = start.X;
            float yi = start.Y;

            for (int i = 1; i < l + 1; i++)
            {
                PutPixel(g, (int)Math.Round(xi), (int)Math.Round(yi), color);
                xi += dx;
                yi += dy;
            }
        }

        public void Wu(Graphics g, Color color)
        {
            double startX = start.X;
            double startY = start.Y;
            double endX = end.X;
            double endY = end.Y;

            double dX = endX - startX;
            double dY = endY - startY;
            int Imax = 255;
            double tg = 1;

            // чем ближе пиксель к точке идеального отрезка, 
            // тем с большей интенсивностью он должен высвечиваться
            // на каждом шаге высвечивается два пикселя

            if (Math.Abs(endY - startY) > Math.Abs(endX - startX))
            {
                if (startY > endY)
                {
                    double temp = startX;
                    startX = endX;
                    endX = temp;

                    temp = startY;
                    startY = endY;
                    endY = temp;
                }

                // тангенс угла наклона
                if (dY != 0)
                    tg = dX / dY;

                int intStartY = (int)Math.Round(startY);
                int intEndY = (int)Math.Round(endY);

                double x = startX;

                for (int y = intStartY; y < intEndY + 1; y++)
                {
                    double d1 = x - ipart(x);
                    double d2 = 1 - d1;

                    //нижняя точка
                    PutPixel(g, ipart(x), y, (int)(d2 * Imax), color);

                    // верхняя точка
                    PutPixel(g, ipart(x) + 1, y, (int)(d1 * Imax), color);

                    x += tg;
                }
            }
            else
            {
                if (startX > endX)
                {
                    double temp = startX;
                    startX = endX;
                    endX = temp;

                    temp = startY;
                    startY = endY;
                    endY = temp;
                }

                if (dX != 0)
                    tg = dY / dX;

                int intStartX = (int)Math.Round(startX);
                int intEndX = (int)Math.Round(endX);

                double y = startY;

                for (int x = intStartX; x < intEndX + 1; x++)
                {
                    double d1 = y - ipart(y); // задано четре точки, четыре узла. этого достаточно чтобы построить полином 3 степени, таблица состоиит из 4 сторочек. по ней по всей будет построен полином, и по ней по всей будет построен сплайн. в середине каждого интервала выдать, чтобы будет выводить сплайн и полином
                    double d2 = 1 - d1;

                    //нижняя точка
                    PutPixel(g, x, ipart(y), (int)(d2 * Imax), color);

                    // верхняя точка
                    PutPixel(g, x, ipart(y) + 1, (int)(d1 * Imax), color);

                    y += tg;
                }
            }
        }

        private int ipart(double x)
        {
            if (x < 0)
                return (int)(x - 1);

            return (int)x;
        }

        //Метод, устанавливающий пиксел на форме с заданными цветом и прозрачностью
        private void PutPixel(Graphics g, int x, int y, int alpha, Color color)
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(alpha, color)), x, y, 1, 1);
        }

        private void PutPixel(Graphics g, int x, int y, Color color)
        {
            g.FillRectangle(new SolidBrush(color), x, y, 1, 1);
        }

        public void StandartDrow(Graphics g, Color color)
        {
            g.DrawLine(new Pen(color), start.X, start.Y, end.X, end.Y);
        }

        private int sign(float val)
        {
            if (val < 0)
            {
                return -1;
            }
            else if (val > 0)
            {
                return 1;
            }

            return 0;
        }
    }
}
