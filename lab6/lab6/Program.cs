using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

namespace lab6
{
    class Program
    {
        // Функция
        static double function(double x)
        {
           // return (Math.Pow(x, 2) + Math.Sin((0.48 * (x + 2.0)))) / (Math.Pow(Math.E, (Math.Pow(x, 2) + 0.38))); // Функция
            return 1 / (Math.Sqrt(x) * Math.Pow(Math.E, 0.9 * x));
            //return Math.Pow(x, 2);
        }

        //n - точность (кол-во интервалов разбиения)
        //a и b - границы отрезка, на котором происходит интегрирование
        static double Trapezoid(double n, double a, double b)
        {
            double width = (b - a) / n;
            double increment = 0;

            for (int step = 0; step < n; step++)
            {
                double x1 = a + step * width;
                double x2 = a + (step + 1) * width;

                increment += 0.5 * (x2 - x1) * (function(x1) + function(x2));
            }

            return increment;
        }

        //n - точность (кол-во интервалов разбиения)
        //a и b - границы отрезка, на котором происходит интегрирование
        static double Rectangle(double n, double a, double b)
        {
            double x, step, increment = 0, y;
            step = (b - a) / n;

            for (x = (a + step / 2); x < b; x += step)
            {
                y = function(x);
                increment += y * step;
            }

            return increment;
        }

        static void Main(string[] args)
        {
            Stopwatch time = new Stopwatch();

            time.Start();
            double result_Rect = Rectangle(120, 0.5, 2.0);
            time.Stop();
            Console.WriteLine("Время вычисления методом Прямоугольника: <{0}>", time.Elapsed);
            time.Restart();
            double result_Trap = Trapezoid(120, 0.5, 2.0);
            time.Stop();
            Console.WriteLine("Время вычисления методом Трапеции: <{0}>", time.Elapsed);

            Console.ReadKey();
        }
    }
}
