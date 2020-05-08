using System;
using System.Drawing;
using System.Collections.Generic;

namespace MyDrawLib
{
    public class MyLineClippingKyr_Bek
    {
        private List<Point> LinesArray;
        private List<Point> Cutter;
        private List<Vector> NormalVec = new List<Vector>(1);

        public MyLineClippingKyr_Bek(List<Point> LinesArray, List<Point> Cutter)
        {
            this.LinesArray = LinesArray;
            this.Cutter = Cutter;
        }

        public List<Point> Clipping()
        {
            List<Point> currLines = new List<Point>(1);
            
            int walkDirection = ConvexityCheck();

            if (walkDirection != 0)
            {
                FindNormalVectors(walkDirection);

                for (int j = 0; j < LinesArray.Count - 1; j += 2)
                {
                    float t_down = 0, t_up = 1;
                    float t_tmp;

                    Vector D = new Vector(LinesArray[j + 1], LinesArray[j]);
                    Vector w;

                    float D_sc;
                    float W_sc;

                    bool breakFlag = false;
                    for (int i = 0; i < Cutter.Count; i++)
                    {
                        w = new Vector(LinesArray[j], Cutter[i]);
                        D_sc = Vector.ScalarMultiplication(D, NormalVec[i]);
                        W_sc = Vector.ScalarMultiplication(w, NormalVec[i]);

                        if (D_sc == 0) // отрезок выродился в точку / D и сторона парралельны
                        {
                            if (W_sc < 0)
                            {
                                breakFlag = true;
                                break;
                            }
                            // точка видима относительно текущей границы
                        }
                        else
                        {
                            t_tmp = -1 * (W_sc / D_sc);
                            if (D_sc > 0) // поиск нижнего предела
                            {
                                if (t_tmp > 1)
                                {
                                    breakFlag = true;
                                    break;
                                }
                                t_down = Math.Max(t_down, t_tmp);
                            }
                            else // поиск верхнего предела
                            {
                                if (t_tmp < 0)
                                {
                                    breakFlag = true;
                                    break;
                                }
                                t_up = Math.Min(t_up, t_tmp);
                            }
                        }
                    }

                    if (t_down > t_up)
                    {
                        breakFlag = true;
                        break;
                    }

                    if (!breakFlag)
                    {
                        currLines.Add(GetDot(t_down, LinesArray[j], LinesArray[j + 1]));
                        currLines.Add(GetDot(t_up, LinesArray[j], LinesArray[j + 1]));

                    }
                }
            }
            else
            {
                Console.WriteLine("Ошибка!\nМногоугольник невыпуклый!");
            }

            return currLines;
        }

        private int GetCoordX(float t, Point start, Point end)
        {
            return (int)Math.Round(start.X + (end.X - start.X) * t);
        }
        private int GetCoordY(float t, Point start, Point end)
        {
            return (int)Math.Round(start.Y + (end.Y - start.Y) * t);
        }
        public Point GetDot(float t, Point start, Point end)
        {
            return new Point(GetCoordX(t, start, end), GetCoordY(t, start, end));
        }

        // Получение вершины по индексу
        public PointF GetVertex(int index)
        {
            if (index < 0)
                return Cutter[Cutter.Count + index]; // Вершина с конца
            else
                return Cutter[index % Cutter.Count];
        }

        // Проверка выпуклости; возвращает направление обхода
        private int ConvexityCheck()
        {
            if (Cutter.Count < 3)
                return 0;

            Vector a = new Vector(Cutter[1], Cutter[0]);
            Vector b = new Vector();
            Vector res = new Vector();

            int sign = 0;

            for (int i = 0; i < Cutter.Count; i++)
            {
                b = new Vector(GetVertex(i + 1), GetVertex(i));
                Vector.VectorMultiplication(a, b, ref res);

                if (sign == 0)
                    sign = Math.Sign(res.z);
                else if ((sign != Math.Sign(res.z)) && (res.z != 0))
                    return 0;

                a = b;
            }

            if (sign == 0) // Вырождается в линию
                return 0;

            return sign;
        }

        // Нахождение векторов нормали для отсекателя
        private void FindNormalVectors(int direction)
        {
            Vector b;
            float tmp;
            NormalVec.Clear();

            for (int i = 0; i < Cutter.Count; i++)
            {
                b = new Vector(GetVertex(i + 1), GetVertex(i));

                tmp = b.x;
                b.x = b.y;
                b.y = tmp;

                if (direction == -1)
                    b.y *= -1;
                else
                    b.x *= -1;

                NormalVec.Add(b);
            }
        }
    }
}
 