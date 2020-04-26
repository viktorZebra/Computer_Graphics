using System.Collections.Generic;
using System.Drawing;

namespace MyDrawLib
{
    public class MySeedPolygon
    {
        private Graphics g;
        private List<List<Point>> allFigures; // все фигуры нарисованные на форме (для обработки каждой фигуры отдельно)

        private List<List<Point>> pointsScanLines = new List<List<Point>>(1); // список списков, в которых хранится n-ое кол-во точек (пересечений скан. линии и фигуры)

        private Bitmap picture;
        private List<Point> extremums = new List<Point>(1);

        private Point seedPixel = new Point(0, 0);

        public long time;

        public MySeedPolygon(Graphics g, Bitmap picture, List<List<Point>> allFigures, Point seedPixel)
        {
            this.g = g;
            this.allFigures = allFigures;
            this.picture = picture;
            this.seedPixel = seedPixel;
        }

        public Bitmap SeedPolygon(Color background, Color fill, Color border)
        {
            Stack<Point> seedStack = new Stack<Point>(1);

            seedStack.Push(seedPixel);
            int bufX, xRight, xLeft;

            while (seedStack.Count > 0)
            {
                Point pixel = seedStack.Pop(); // извлекаем из стека пиксел и присваиваем ему новое значение
                picture.SetPixel(pixel.X, pixel.Y, fill);

                bufX = pixel.X; // сохраняем координату Х, для дальнейшего восстановления ее, чтобы рассматреть интервал слева
                int flag = 0;

                // заполняем интервал справа от затравки
                pixel.X++;
                while (picture.GetPixel(pixel.X, pixel.Y).ToArgb() != border.ToArgb())
                {
                    picture.SetPixel(pixel.X, pixel.Y, fill);
                    pixel.X++;
                }

                xRight = pixel.X - 1; // сохраняем крайний справа пиксел

                pixel.X = bufX; // восстанавливаем Х координату

                // теперь заполняем слева от затравки
                pixel.X--;
                while (picture.GetPixel(pixel.X, pixel.Y).ToArgb() != border.ToArgb())
                {
                    picture.SetPixel(pixel.X, pixel.Y, fill);
                    pixel.X--;
                }

                xLeft = pixel.X + 1; // сохраняем крайний слева пиксел

                pixel.Y++; // теперь проверяем строку выше прошлого затравочного пиксела, не является ли ни границей, ни уже полностью заполненой. если это не так, то находим затравку, начиная с левого края подынтервала скан. строки
                pixel.X = xLeft;

                while (pixel.X <= xRight)
                {
                    flag = 0;

                    // ищем затравку на строке выше начиная с левой границы подынтервала 
                    while (picture.GetPixel(pixel.X, pixel.Y).ToArgb() != border.ToArgb() &&
                           picture.GetPixel(pixel.X, pixel.Y).ToArgb() != fill.ToArgb() &&
                           pixel.X <= xRight) // пока не встретим границу или закрашенную область а так же пока не дайдем до правой границы подынтервала
                    {
                        if (flag == 0)
                        {
                            flag = 1;
                        }

                        pixel.X++;
                    }

                    // помещаем в стек крайний правый справа пиксел
                    if (flag == 1)
                    {
                        if (pixel.X == xRight &&
                            picture.GetPixel(pixel.X, pixel.Y).ToArgb() != border.ToArgb() &&
                            picture.GetPixel(pixel.X, pixel.Y).ToArgb() != fill.ToArgb()) // если дошли до правой границы подынтервала, а граница многоугольника не была встречена или закрашенная область
                        {
                            seedStack.Push(pixel); // помещаем данный пиксел в стек затравки
                        }
                        else
                        {
                            seedStack.Push(new Point(pixel.X - 1, pixel.Y)); // иначе помещаем предыдущий пиксел
                        }

                        flag = 0;
                    }

                    // продолжаем проверку, если интервал был прерван
                    bufX = pixel.X;
                    while ((picture.GetPixel(pixel.X, pixel.Y).ToArgb() == border.ToArgb() ||
                            picture.GetPixel(pixel.X, pixel.Y).ToArgb() == fill.ToArgb()) &&
                            pixel.X < xRight)
                    {
                        pixel.X++;
                    }

                    // удостоверимся, что кооридната пиксела увеличина
                    if (pixel.X == bufX)
                    {
                        pixel.X++;
                    }
                }

                // дальше идет аналогичная проверка, только для нижней строки относительно нынешнего затравочного пиксела
                pixel.Y -= 2;
                pixel.X = xLeft;

                while (pixel.X <= xRight)
                {
                    flag = 0;

                    while (picture.GetPixel(pixel.X, pixel.Y).ToArgb() != border.ToArgb() &&
                           picture.GetPixel(pixel.X, pixel.Y).ToArgb() != fill.ToArgb() &&
                           pixel.X <= xRight)
                    {
                        if (flag == 0)
                        {
                            flag = 1;
                        }

                        pixel.X++;
                    }

                    if (flag == 1)
                    {
                        if (pixel.X == xRight &&
                            picture.GetPixel(pixel.X, pixel.Y).ToArgb() != border.ToArgb() &&
                            picture.GetPixel(pixel.X, pixel.Y).ToArgb() != fill.ToArgb())
                        {
                            seedStack.Push(pixel);
                        }
                        else
                        {
                            seedStack.Push(new Point(pixel.X - 1, pixel.Y));
                        }

                        flag = 0;
                    }

                    bufX = pixel.X;
                    while ((picture.GetPixel(pixel.X, pixel.Y).ToArgb() == border.ToArgb() ||
                            picture.GetPixel(pixel.X, pixel.Y).ToArgb() == fill.ToArgb()) &&
                            pixel.X < xRight)
                    {
                        pixel.X++;
                    }

                    if (pixel.X == bufX)
                    {
                        pixel.X++;
                    }
                }
            }
            return picture;
        }
    }
}
