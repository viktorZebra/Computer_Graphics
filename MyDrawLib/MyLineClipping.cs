using System;
using System.Drawing;
using System.Collections.Generic;

namespace MyDrawLib
{ 
    public class MyLineClipping
    {
        private struct Windw
        {
            public Point UpLeftP;
            public Point DownRightP;
        }

        private Point[] LinesArray; // последовательный массив точек линий 
        //private Point[,] PointsArray;
        private Windw windw;

        public MyLineClipping(Point UpLeftP, Point DownRightP, Point[] LinesArray)
        {
            windw.UpLeftP = UpLeftP;
            windw.DownRightP = DownRightP;

            this.LinesArray = LinesArray;
            //this.PointsArray = PointsArray;
        }

        public List<Point> Clipping()
        {
            List<Point> currLines = new List<Point>(1);

            for (int i = 0; i < LinesArray.Length - 1; i+=2)
            {
                int j = 1; // шаг отсечения

                Point startP = LinesArray[i];
                Point endP = LinesArray[i + 1];

                while (true)
                {
                    int codeStartP = ProcBinCode(startP); // вычисляем значения битовых кодов
                    int codeEndP = ProcBinCode(endP);

                    if (codeEndP + codeStartP == 0) // отрезок тривиально видим
                    {
                        currLines.Add(startP); // закрашиваем этот отрезок
                        currLines.Add(endP);


                        break; // заканчиваем просмотр данного отрезка
                    }

                    Point bufP;
                    Point tmp;
                    if ((codeEndP & codeStartP) == 0) // если побитовое произведение концевых точек равно нулю, то имеет смысл их рассматривать дальше
                    {
                        bufP = new Point(startP.X, startP.Y); // запоминаем концевую точку

                        if (j > 2) // если это не первый и не второй шаг отсечения
                        {
                            if ((codeEndP & codeStartP) == 0) // если лог произ. = 0, то точно оставшийся отрезок видим
                            {
                                currLines.Add(startP);
                                currLines.Add(endP);

                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                        // если побитовый код второй точки = 0, она лежит внутри
                        if (codeEndP == 0)
                        {
                            startP = endP; // меняем точки местами, чтобы два раза не прописывать алгоритм
                            endP = bufP;
                            j++;
                        }
                        else // если побитовый код первой точки = 0, она лежит внутри
                        {
                            // пока отрезок не вырождается в точку (проверяем через диагональ пикселя)
                            Point medP = new Point(0, 0);
                            double ACCURACY = Math.Sqrt(2);
                            while (Math.Abs(startP.X - endP.X) > ACCURACY || Math.Abs(startP.Y - endP.Y) > ACCURACY)
                            {
                                // вычисляем координаты средней точки
                                medP.X = (startP.X + endP.X) >> 1;
                                medP.Y = (startP.Y + endP.Y) >> 1;

                                tmp = startP; // запоминаем первую точку 
                                startP = medP; // теперь первая точка это средняя

                                codeStartP = ProcBinCode(startP); // побитовый код средней точки

                                if ((codeStartP & codeEndP) != 0)
                                {
                                    // если условие прошло, то откидываем эту часть отрезка
                                    startP = tmp;
                                    endP = medP;
                                }
                            }

                            startP = endP; // опять меняем точки местами
                            endP = bufP;
                            j++; // увеличиваем шаг отсечения
                        }
                    }
                    else
                    {
                        // если побитовое произведение кодов концов точек не равно 0, то тривиально неведим
                        break;
                    }
                }
            }

            return currLines;
        }

        private int ProcBinCode(Point p)
        {
            int code = 0;

            if (p.X < windw.UpLeftP.X && p.Y < windw.UpLeftP.Y)
                code = 9; // 1001
            if (p.X > windw.UpLeftP.X && p.X < windw.DownRightP.X && p.Y < windw.UpLeftP.Y)
                code = 8; // 1000
            if (p.X > windw.DownRightP.X && p.Y < windw.UpLeftP.Y)
                code = 10; // 1010
            if (p.X > windw.DownRightP.X && p.Y < windw.DownRightP.Y && p.Y > windw.UpLeftP.Y)
                code = 2; // 0010
            if (p.X > windw.DownRightP.X && p.Y > windw.DownRightP.Y)
                code = 6; // 0110
            if (p.X < windw.DownRightP.X && p.X > windw.UpLeftP.X && p.Y > windw.DownRightP.Y)
                code = 4; // 0100
            if (p.X < windw.UpLeftP.X && p.Y > windw.DownRightP.Y)
                code = 5; // 0101
            if (p.X < windw.UpLeftP.X && p.Y < windw.DownRightP.Y && p.Y > windw.UpLeftP.Y)
                code = 1; // 0001

            return code;
        }
    }
}
