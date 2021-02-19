using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
    struct ProcTask
    {
        // Конструктор. Инициализация процесса
        public ProcTask(int _Id, int _Ticks, bool _isBigProc)
        {
            this.Id = _Id;
            this.Ticks = _Ticks;
            this.isBigProc = _isBigProc;
        }

        // Выполнение процесса
        public void Work(int ProcWorkTick)
        {
            if(this.Ticks > 0)
            {
                // Расходуем такт процесса
                this.Ticks -= ProcWorkTick;
                // Выводим информацию
                if(this.Ticks > 0)
                    Console.WriteLine("Process <" + this.Id +
                        "> in proccess. <" + this.Ticks + "> Ticks left");
                else
                    Console.WriteLine("Process <" + this.Id +
                        "> COMPLETE!");
            }
        }
        // ID Процесса
        public int Id;
        // Требуемое кол-во тактов
        public int Ticks;
        // Большой процесс или маленький
        public bool isBigProc;
    }

    class Proc
    {
        public Proc(int kl)
        {
            this.KL = kl;
            A = new Queue<ProcTask>();
            GenerateQueue();
        }

        // Очередь на обработку
        Queue<ProcTask> A;
        // Количество тактов за цикл
        int KL;
        // Храним кол-во пройденных циклов
        int procCycles = 0;
        // Храним пропущенные такты "в холостую"
        int procPassTimes = 0;
        // Храним время на маленькие задачи
        int procSmallTimes = 0;
        // Храним количество маленьких задач
        int procSmallCount = 0;

        // Генерируем очередь на обработку
        public void GenerateQueue()
        {
            Random rnd = new Random();
            // Создаём 10 задач с разным
            // кол-вом требуемых тактов
            for(int i = 0; i < 10; i++)
            {
                if (rnd.Next(0, 100) < 50)
                {
                    // 50% - Что будет большая задача
                    ProcTask _proc = new ProcTask(i, 6, true);
                    A.Enqueue(_proc);
                }
                else
                {
                    // 50% - что будет маленькая задача
                    ProcTask _proc = new ProcTask(i, rnd.Next(1, this.KL), false);
                    A.Enqueue(_proc);
                    this.procSmallCount++;
                }
            }
        }

        public void Work()
        {
            while(this.A.Count != 0)
            {
                ProcTask task = this.A.Dequeue();
                if(task.Ticks > this.KL)
                {
                    // Если задача выполняется больше такта
                    // Дадим ей поработать
                    task.Work(this.KL);
                    // И отложим на потом
                    this.A.Enqueue(task);
                }
                else
                {
                    // Если задача выполняется меньше такта
                    if(task.Ticks <= this.KL)
                    {
                        // Время пребывания процесса в очереди
                        this.procSmallTimes += this.procCycles;
                        // Кол-во тактов "в холостую"
                        this.procPassTimes += (KL-task.Ticks);
                        // Берём процесс в работу
                        task.Work(this.KL);
                        
                    }
                }
                this.procCycles++;
            }
            if(this.procSmallCount > 0)
            {
                Console.WriteLine("\nСреднее время пребывания короткой заявки в системе = " + 
                    (double)this.procSmallTimes/ this.procSmallCount);
                Console.WriteLine("степень загрузки процессора (вероятность занятого состояния) = " +
                    (100 * this.procPassTimes) / (KL*this.procCycles) + " %");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Proc proc = new Proc(2);
            proc.Work();
            Console.ReadKey();
        }
    }
}
