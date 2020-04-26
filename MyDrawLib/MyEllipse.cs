using System;
using System.Drawing;

namespace MyDrawLib
{
    public class MyEllipse
    {
        private PointF center;
		private float height;
		private float width;

		public PointF Center { get { return center; } }
		public float Height { get { return height; } }
		public float Width { get { return width; } }

		public MyEllipse(float x, float y, float height, float width)
        {
			center.X = x;
			center.Y = y;

            this.height = height;
            this.width = width;
        }

		public void Breshenham(Graphics g, Color color)
		{
			int a = (int)Math.Round(width / 2);
			int b = (int)Math.Round(height / 2);

			// координаты центра
			int centX = (int)Math.Round(center.X);
			int centY = (int)Math.Round(center.Y);

			// координаты начальной точки, y_конечная == 0
			int x = 0;
			int y = b;

			// bb(x + 1)^2 + aa(y - 1)^2 - aabb = delta_diag
			int aa = a * a;
			int bb = b * b;
			int aabb = aa * bb;

			// для коррекции delta_diag в цикле
			int bb2 = bb * 2;
			int aa2 = aa * 2;

			int delta_diag = bb * (int)Math.Pow(x + 1, 2) + aa * (int)Math.Pow(y - 1, 2) - aabb;
			int delta_hd;
			int delta_vd;

			// для первого квадранта
			while (y >= 0)
			{
				// высвечиваем текущую точку
				PutPixel(centX - x, centY - y, g, color);
				PutPixel(centX - x, centY + y, g, color);
				PutPixel(centX + x, centY - y, g, color);
				PutPixel(centX + x, centY + y, g, color);

				// делаем шаг по критерию
				if (delta_diag == 0)
				{
					// делаем диагональный шаг
					y--;
					x++;
					delta_diag += bb2 * x + bb - aa2 * y + aa;
				}
				else if (delta_diag > 0)
				{
					// выбираем шаг: вертикальный или диагональный
					// ---------------------------------------------
					// отклонение для диагонального: delta_diag
					// отклонение для вертикального: delta_diag - bb * ( 2x + 1)
					// разность отклонений:  fabs(delta_diag) - fabs(delta_diag - bb * 2x - bb)
					// fabs(delta_diag) == delta_diag
					// fabs(delta_diag - bb * (2x + 1)) == -(delta_diag - bb * 2x - bb)
					// -> delta_vd = 2 * delta_diag - bb2 * x - bb

					delta_vd = 2 * delta_diag - bb2 * x - bb;
					if (delta_vd > 0)
					{
						// делаем вертикальный шаг
						y--;
						delta_diag += aa - aa2 * y;
					}
					else
					{
						// делаем диагональный шаг
						y--;
						x++;
						delta_diag += bb2 * x + bb - aa2 * y + aa;
					}
				}
				else
				{
					// выбираем шаг: горизонтальный или диагональный
					// ---------------------------------------------
					// отклонение для диагонального: delta_diag
					// отклонение для горизонтального: delta_diag - 2 * y * aa + aa
					// разность отклонений: fabs(delta_diag + 2 * y * aa - aa) - fabs(delta_diag)
					// fabs(delta_diag) == -delta_diag
					// fabs(delta_diag + 2 * y * aa - aa) == delta_diag + 2 * y * aa - aa
					// -> delta_vd = 2 * delta_diag + aa2 * y - aa

					delta_hd = 2 * delta_diag + aa2 * y - aa;
					if (delta_hd >= 0)
					{
						// делаем диагональный шаг
						y--;
						x++;
						delta_diag += bb2 * x + bb - aa2 * y + aa;
					}
					else
					{
						// делаем горизонтальный шаг
						x++;
						delta_diag += bb2 * x + bb;
					}
				}
			}
		}

		public void MiddlePoint(Graphics g, Color color)
		{
			int a = (int)Math.Round(width / 2);
			int b = (int)Math.Round(height / 2);

			int aa = a * a;
			int aa2 = aa * 2;

			int bb = b * b;
			int bb2 = bb * 2;

			int aabb = aa * bb;

			// центр рисования
			int centX = (int)Math.Round(center.X);
			int centY = (int)Math.Round(center.Y);

			// первая точка
			int y = b;
			int x = 0;

			// граница рисования по x
			int xend = (int)Math.Round((double)(aa / (int)Math.Sqrt(aa + bb)));

			// граница рисования по y
			int yend = 0;

			// для выбора следующей точки: f(M) - M(x + 1, y - 0.5),
			// f = bb * (x + 1)^2 + aa(y - 0.5)^2 - aabb, пробная функция
			double f = bb + aa * Math.Pow(y - 0.5, 2) - aabb;

			// коррекция пробной функции
			int delta_f = -aa2 * y + aa2;
			int df = bb2;

			for (; x < xend; x++)
			{
				PutPixel(centX - x, centY - y, g, color);
				PutPixel(centX - x, centY + y, g, color);
				PutPixel(centX + x, centY - y, g, color);
				PutPixel(centX + x, centY + y, g, color);

				// выбираем следующую точку
				if (f >= 0) // диагональный шаг
				{
					y--;

					// нужно скорректировать пробную функцию - она была для точки (x + 1, y - 0.5)
					// должна стать для точки (x + 1, y - 1.5)
					f += delta_f;
					delta_f += aa2;
				}

				// еще коррекция функции, потому что шаг по x тоже был
				f += df + bb;
				df += bb2;
			}

			// для выбора следующей точки: f(M) - M(x + 0.5, y - 1),
			// f = bb * (x + 0.5)^2 + aa(y - 1)^2 - aabb, пробная функция
			f = bb * Math.Pow(x + 0.5, 2) + aa * Math.Pow(y - 1, 2) - aabb;

			// коррекция пробной функции
			delta_f = bb2 * x + bb2;
			df = -aa2 * y + aa2;

			// начало рисования по y
			for (; y >= yend; y--)
			{
				PutPixel(centX - x, centY - y, g, color);
				PutPixel(centX - x, centY + y, g, color);
				PutPixel(centX + x, centY - y, g, color);
				PutPixel(centX + x, centY + y, g, color);

				// выбираем следующую точку
				if (f < 0) // диагональный шаг
				{
					x++;

					// нужно скорректировать пробную функцию - она была для точки (x + 0.5, y - 1)
					// должна стать для точки (x + 1.5, y - 1)
					f += delta_f;
					delta_f += bb2;
				}

				f += df + aa;
				df += aa2;
			}
		}

		public void Canonical(Graphics g, Color color)
		{
			// ( x^2 / a^2 ) + ( y^2 / b^2 ) = 1

			int a = (int)Math.Round(width / 2);
			int b = (int)Math.Round(height / 2);

			int a2 = a * a;
			int b2 = b * b;
			double coef = (double)b / a;

			int centX = (int)Math.Round(center.X);
			int centY = (int)Math.Round(center.Y);

			// мы рисуем по x до точки, в которой y' = -1
			int xend = (int)Math.Round((double)(a2 / (int)Math.Sqrt(b2 + a2)));

			// после этого мы рисуем по y
			int yend = 0;

			int y = 0;
			int x;

			for (x = 0; x < xend; x++)
			{
				y = (int)Math.Round(coef * (int)Math.Sqrt(a2 - x * x));

				PutPixel(centX - x, centY - y, g, color);
				PutPixel(centX - x, centY + y, g, color);
				PutPixel(centX + x, centY - y, g, color);
				PutPixel(centX + x, centY + y, g, color);
			}

			coef = 1 / coef;
			for (; y >= yend; y--)
			{
				x = (int)Math.Round(coef * (int)Math.Sqrt(b2 - y * y));

				PutPixel(centX - x, centY - y, g, color);
				PutPixel(centX - x, centY + y, g, color);
				PutPixel(centX + x, centY - y, g, color);
				PutPixel(centX + x, centY + y, g, color);
			}
		}

		public void Parametric(Graphics g, Color color)
		{
			int a = (int)Math.Round(width / 2);
			int aa = a * a;

			int b = (int)Math.Round(height / 2);
			int bb = b * b;

			int yend = (int)Math.Round((double)(bb / (int)Math.Sqrt(bb + aa)));
			int xend = 0;

			int centX = (int)Math.Round(center.X);
			int centY = (int)Math.Round(center.Y);

			int x = a;
			int y = 0;

			double t = 0;
			double step = (double)1 / b;
			while (y < yend)
			{
				PutPixel(centX - x, centY - y, g, color);
				PutPixel(centX - x, centY + y, g, color);
				PutPixel(centX + x, centY - y, g, color);
				PutPixel(centX + x, centY + y, g, color);

				t += step;
				x = (int)Math.Round(a * Math.Cos(t));
				y = (int)Math.Round(b * Math.Sin(t));
			}

			step = (double)1 / a;
			while (x >= xend)
			{
				PutPixel(centX - x, centY - y, g, color);
				PutPixel(centX - x, centY + y, g, color);
				PutPixel(centX + x, centY - y, g, color);
				PutPixel(centX + x, centY + y, g, color);

				t += step;
				x = (int)Math.Round(a * Math.Cos(t));
				y = (int)Math.Round(b * Math.Sin(t));
			}
		}

		public void StandartDrow(Graphics g, Color color)
		{
			g.DrawEllipse(new Pen(color), center.X - width / 2, center.Y - height / 2, width, height);
		}

		private void PutPixel(int x, int y, Graphics g, Color color)
		{
			g.FillRectangle(new SolidBrush(color), x, y, 1, 1);
		}
	}
}
