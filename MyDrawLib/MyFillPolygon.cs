using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace MyDrawLib
{
    public class MyFillPolygon
    {
        private Graphics g;
        private List<List<Point>> allFigures; // все фигуры нарисованные на форме (для обработки каждой фигуры отдельно)

        private Bitmap picture;
        private List<Point> extremums = new List<Point>(1);

        public MyFillPolygon(Graphics g, Bitmap picture, List<List<Point>> allFigures)
        {
            this.g = g;
            this.allFigures = allFigures;
            this.picture = picture;
        }

        public Bitmap FillPolygon(Color background, Color fill, Color border)
        {
            g.Clear(Color.White);
            for (int i = 0; i < allFigures.Count; i++) // рисуем контур каждой фигуры отдельно, чтобы обработать проблемные точки (локальные макс мин, сливание острых углов в одну прямую)
            {
                SearchMaxes(allFigures[i]);
                for (int j = 0; j < allFigures[i].Count; j++)
                {
                    DrawBorder(allFigures[i][j], allFigures[i][(j + 1) % allFigures[i].Count], border);
                }
            }

            int xMin = 0, xMax = 0, yMin = 0, yMax = 0;
            MaxAndMinAllFigures(ref xMin, ref xMax, ref yMin, ref yMax);

            for (int y = yMin; y <= yMax; y++) // закрашиваем все фигуры
            {
                bool Flag = false;
                for (int x = xMin; x <= xMax; x++)
                {
                    Color currentPixel = picture.GetPixel(x, y);
                    if (IsEquColors(currentPixel, border))
                    {
                        Flag = !Flag;
                    }

                    if (Flag)
                    {
                        picture.SetPixel(x, y, fill);
                    }
                    else
                    {
                        picture.SetPixel(x, y, background);
                    }
                }
            }

            return picture;
        }

        private void DrawBorder(Point pointfrom, Point pointto, Color border) // рисую границы многоугольника только самую левую(первую) точку на прямой методом брезенхема ИНТ
        {
            int dx = (int)(Math.Abs(pointto.X - pointfrom.X));
            int dy = (int)(Math.Abs(pointto.Y - pointfrom.Y));
            int stepx = Math.Sign(pointto.X - pointfrom.X);
            int stepy = Math.Sign(pointto.Y - pointfrom.Y);


            int flag;
            if (dy > dx)
            {
                Swap<int>(ref dx, ref dy);
                flag = 1;
            }
            else
                flag = 0;

            int e = 2 * dy - dx;
            int lasty = pointfrom.Y;

            Point calculated = new Point(pointfrom.X, pointfrom.Y);

            //проверка вершины на экстремум
            if (extremums.Contains(calculated) == true)
            {
                //две точки если экстремум
                SetPixel(ref calculated, border);
                SetPixel(ref calculated, border);

            }
            else
            {
                //одна точка если не экстремум
                SetPixel(ref calculated, border);
            }

            for (int i = 0; i < dx; i++)
            {
                if (lasty != calculated.Y) // вот тут я и смотрю, если новая точка прямой, лежит на одной горизонтальной с прошлой точкой, то ее ставить не надо, поставится только в том случае, если произошло смещение по оси У, таким образом я добиваюсь, чтобы на ребре была одна точка, кроме случаев экстремумов
                {
                    lasty = calculated.Y;
                    SetPixel(ref calculated, border);
                }

                if (e >= 0)
                {
                    if (flag == 1)
                        calculated.X += stepx;
                    else
                        calculated.Y += stepy;

                    e -= 2 * dx;
                }
                if (e < 0)
                {
                    if (flag == 1)
                        calculated.Y += stepy;
                    else
                        calculated.X += stepx;

                }
                e += 2 * dy;

                if (calculated.Y == pointto.Y) // дошли до конца
                    return;
            }
        }

        private void Swap<T>(ref T x, ref T y)
        {
            T temp = x;
            x = y;
            y = temp;
        }

        private void SetPixel(ref Point p, Color border)
        {
            Color currentPixel = picture.GetPixel(p.X, p.Y);

            if (currentPixel.ToArgb() == border.ToArgb())
                picture.SetPixel(p.X + 1, p.Y, border);
            else
                picture.SetPixel(p.X, p.Y, border);
        }

        private void SearchMaxes(List<Point> pointsPolygon)
        {
            int y0, y1, y2;
            for (int i = 0; i < pointsPolygon.Count(); i++)
            {
                y0 = pointsPolygon[i].Y;
                y1 = pointsPolygon[(pointsPolygon.Count() + i - 1) % pointsPolygon.Count()].Y;
                y2 = pointsPolygon[(i + 1) % pointsPolygon.Count()].Y;

                if ((y0 < y1 && y0 < y2) || (y0 >= y1 && y0 >= y2))
                {
                    extremums.Add(pointsPolygon[i]);
                }
            }
        }

        private void MaxAndMinAllFigures(ref int xMin, ref int xMax, ref int yMin, ref int yMax)
        {
            xMax = SearchMaxCoor(allFigures[0], true);
            xMin = SearchMinCoor(allFigures[0], true);

            yMax = SearchMaxCoor(allFigures[0], false);
            yMin = SearchMinCoor(allFigures[0], false);

            for (int i = 0; i < allFigures.Count; i++)
            {
                int bufXMax = SearchMaxCoor(allFigures[i], true);
                int bufXMin = SearchMinCoor(allFigures[i], true);

                int bufYMax = SearchMaxCoor(allFigures[i], false);
                int bufYMin = SearchMinCoor(allFigures[i], false);

                if (bufXMax > xMax)
                {
                    xMax = bufXMax;
                }

                if (bufXMin < xMin)
                {
                    xMin = bufXMin;
                }

                if (bufYMax > yMax)
                {
                    yMax = bufYMax;
                }

                if (bufYMin < yMin)
                {
                    yMin = bufYMin;
                }
            }
        }

        private bool IsEquColors(Color clr1, Color clr2)
        {
            return clr1.R == clr2.R && clr1.G == clr2.G && clr1.B == clr2.B && clr1.A == clr2.A;
        }

        private int SearchMinCoor(List<Point> points, bool FlagsX)
        {
            Point min = points[0];

            for (int i = 0; i < points.Count; i++)
            {
                if (FlagsX)
                {
                    if (points[i].X < min.X)
                    {
                        min = points[i];
                    }
                }
                else
                {
                    if (points[i].Y < min.Y)
                    {
                        min = points[i];
                    }
                }
            }

            if (FlagsX)
            {
                return min.X;
            }
            else
            {
                return min.Y;
            }
        }

        private int SearchMaxCoor(List<Point> points, bool FlagsX)
        {
            Point max = points[0];

            for (int i = 0; i < points.Count; i++)
            {
                if (FlagsX)
                {
                    if (points[i].X > max.X)
                    {
                        max = points[i];
                    }
                }
                else
                {
                    if (points[i].Y > max.Y)
                    {
                        max = points[i];
                    }
                }
            }

            if (FlagsX)
            {
                return max.X;
            }
            else
            {
                return max.Y;
            }
        }
    }
}
