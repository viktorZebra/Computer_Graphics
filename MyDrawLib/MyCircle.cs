using System;
using System.Drawing;

namespace MyDrawLib
{
    public class MyCircle
    {
        private Point center;
        private int radius;

        public Point Center { get { return center; } }
        public int Radius { get { return radius; } }
        public MyCircle(int x, int y, int radius)
        {
            center.X = x;
            center.Y = y;

            this.radius = radius;
        }

        public void Canonical(Graphics g, Color color)
        {

            int r2 = radius * radius;
            int i = 0;
            int j = radius;
            int x = center.X;
            int y = center.Y;

            while (i < j)
            {
                PutPixel(x + i, y + j, g, color);
                PutPixel(x + i, y - j, g, color);
                PutPixel(x - i, y + j, g, color);
                PutPixel(x - i, y - j, g, color);
                PutPixel(x + j, y + i, g, color);
                PutPixel(x + j, y - i, g, color);
                PutPixel(x - j, y + i, g, color);
                PutPixel(x - j, y - i, g, color);

                i++;

                j = (int)Math.Round(Math.Sqrt(r2 - i * i));
            }

        }

        public void Parametric(Graphics g, Color color)
        {
            float step = 1 / (float)radius;

            int i, j;
            int x = center.X;
            int y = center.Y;

            double M_PI_4 = Math.PI / 4;
            for (float t = 0; t < M_PI_4; t += step)

            {

                i = (int)Math.Round(radius * Math.Cos(t));

                j = (int)Math.Round(radius * Math.Sin(t));

                PutPixel(x + i, y + j, g, color);
                PutPixel(x + i, y - j, g, color);
                PutPixel(x - i, y + j, g, color);
                PutPixel(x - i, y - j, g, color);
                PutPixel(x + j, y + i, g, color);
                PutPixel(x + j, y - i, g, color);
                PutPixel(x - j, y + i, g, color);
                PutPixel(x - j, y - i, g, color);

            }

            i = (int)Math.Round(radius * 1 / Math.Sqrt(2));
            j = i;

            PutPixel(x + i, y + j, g, color);
            PutPixel(x + i, y - j, g, color);
            PutPixel(x - i, y + j, g, color);
            PutPixel(x - i, y - j, g, color);
        }

        private void DiagonalStepCicrle(ref int x, ref int y, ref int d)
        {
            x++;
            y--;
            d += 2 * (x - y + 1);
        }
        private void HorizontalStepCicrle(ref int x, ref int y, ref int d)
        {
            x++;
            d += 2 * x + 1;
        }

        private void VerticalStepCicrle(ref int x, ref int y, ref int d)
        {
            y--;
            d += -2 * y + 1;
        }

        public void Breshenham(Graphics g, Color color)
        {
            int x = center.X;
            int y = center.Y;

            int r2 = radius * radius;

            int d = 2 * (1 - radius); // первоначальная ошибка

            int i = 0;
            int j = radius;

            while (i < j)

            {

                PutPixel(x + i, y + j, g, color);
                PutPixel(x + i, y - j, g, color);
                PutPixel(x - i, y + j, g, color);
                PutPixel(x - i, y - j, g, color);
                PutPixel(x + j, y + i, g, color);
                PutPixel(x + j, y - i, g, color);
                PutPixel(x - j, y + i, g, color);
                PutPixel(x - j, y - i, g, color);

                if (d == 0) // диагональная точка лежит на окружности
                    DiagonalStepCicrle(ref i, ref j, ref d);

                else if (d < 0) // диагональная точка внутри окружности
                {
                    // раст. до гор пикселя раст. до диаг. пикселя
                    // |(xi+1)^2 + yi^2 - r^2| - |(xi+1)^2 + (yi-1)^2 - r^2|

                    int delta1 = 2 * (d + j) - 1;

                    if (delta1 > 0)
                        DiagonalStepCicrle(ref i, ref j, ref d);
                    else
                        HorizontalStepCicrle(ref i, ref j, ref d);
                }
                else // диагональная точка вне окружности
                {
                    // раст. до диаг. пикселя раст. до верт пикселя
                    // |(xi+1)^2 + (yi-1)^2 - r^2| - |xi^2 + (yi-1)^2 - r^2|

                    int delta2 = 2 * (d - i) - 1;

                    if (delta2 < 0)
                        DiagonalStepCicrle(ref i, ref j, ref d);
                    else
                        VerticalStepCicrle(ref i, ref j, ref d);
                }
            }

            PutPixel(x + i, y + j, g, color);
            PutPixel(x + i, y - j, g, color);
            PutPixel(x - i, y + j, g, color);
            PutPixel(x - i, y - j, g, color);
        }

        public void MiddlePoint(Graphics g, Color color)
        {
            int x = center.X;
            int y = center.Y;

            int i = 0;
            int j = radius;

            // t = x^2 + y^2 - r^2

            // t(1, r-1/2.)

            // t = 5 / 4 - r

            // 0,25 можно отбросить

            // начинаем с целого числа и каждый раз прибавляем целое число

            // невозможна ситация целое число + 0,25 получить ноль

            int t = 1 - radius;

            // t += 2 * i + 3; при горизонтальном шаге

            // t += 2 * (i - j) + 5 при диагональном шаге

            int incrA = 3; // i = 0
            int incrB = 5 - 2 * radius; // i = 0, j = r

            while (i < j)
            {
                PutPixel(x + i, y + j, g, color);
                PutPixel(x + i, y - j, g, color);
                PutPixel(x - i, y + j, g, color);
                PutPixel(x - i, y - j, g, color);
                PutPixel(x + j, y + i, g, color);
                PutPixel(x + j, y - i, g, color);
                PutPixel(x - j, y + i, g, color);
                PutPixel(x - j, y - i, g, color);

                // если диагональный пиксель ближе

                if (t >= 0)
                {

                    // скорректировать пробную функцию

                    // x^2 + (y-1-1/2)^2 - r^2 - (x^2 + (y-1-1/2)^2 - r^2) =

                    // -2y + 2

                    //

                    t += incrB;
                    incrB += 4;

                    // t += 2 * (1 - j);

                    j--;

                }
                else
                {
                    t += incrA;
                    incrB += 2;
                }

                incrA += 2;

                i++;

                // t += 2 * i + 1;

            }

            PutPixel(x + i, y + j, g, color);
            PutPixel(x + i, y - j, g, color);
            PutPixel(x - i, y + j, g, color);
            PutPixel(x - i, y - j, g, color);
        }

        public void StandartDrow(Graphics g, Color color)
        {
            g.DrawEllipse(new Pen(color), center.X - radius, center.Y - radius, radius * 2, radius * 2);
        }

        private void PutPixel(int x, int y, Graphics g, Color color)
        {
            g.FillRectangle(new SolidBrush(color), x, y, 1, 1);
        }
    }
}
