using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab5
{
    struct ProcTask
    {
        // Описываем одну процессорную задачу
        public ProcTask(int _Id, int _Ticks)
        {
            this.Id = _Id;
            this.Ticks = _Ticks;
            this.IdProc = -1;
        }

        // Работа над процессом
        public void Work(int ProcWorkTick)
        {
            // Расходуем такты процесса
            this.Ticks -= ProcWorkTick;
            // Выводим информацию
            if (this.Ticks > 0)
            {
                //Console.WriteLine("Процесс <" + this.Id +
                //        "> выполняется процессором: <" + this.IdProc +
                //        ">. Тактов осталось: <" + this.Ticks + ">.");
            }
            else
            {
                //Console.WriteLine("Process <" + this.Id +
                //        "> COMPLETE!");
            }
        }

        public int Id; // ID Процесса
        public int Ticks; // Требуемое кол-во тактов
        public int IdProc; // ID Процессора
    }


    class ProcSingleMemory
    {
        // Конструктор
        public ProcSingleMemory(int proccount, int tickspercycle, int taskcount)
        {
            // Кол-во задач
            this.taskCount = taskcount;
            // Кол-во процессоров в системе
            this.procCount = proccount;
            // Кол-во тактов за цикл
            this.procTicksPerCycle = tickspercycle;
            // Переменные с результатом работы
            this.ResultMultiMemory = 0;
            this.ResultSingleMemory = 0;
        }


        int procTicksPerCycle; // Количество тактов за цикл
        int procCount; // Кол-во процессоров
        int taskCount; // Кол-во задач
        int ResultSingleMemory; // Результат вычисления однопоточной релизации
        int ResultMultiMemory; // Результат вычисления многопоточной релизации


        // Генерируем очередь на обработку
        public Queue<ProcTask> GenerateQueue(int TaskCount)
        {
            Random random = new Random();
            Queue<ProcTask> queue = new Queue<ProcTask>();

            for (int i = 0; i < TaskCount; i++)
            {
                ProcTask proctask = new ProcTask(i, random.Next(3, 12));
                queue.Enqueue(proctask);
            }

            return queue;
        }

        // Запуск процесса выполнения задач с однопоточной реализацией
        public void WorkSingleMemory()
        {
            // Инициализируем очередь на обработку
            Queue<ProcTask> procQueue = this.GenerateQueue(this.taskCount);

            bool[] procBusy = new bool[this.procCount]; // Занятость процессоров
            for (int i = 0; i < this.procCount; i++) // Инициализируем процессоры
                procBusy[i] = false; // Помечаем процессоры как свободные

            while (procQueue.Count != 0)
            {
                ProcTask curTask = procQueue.Dequeue(); // Текущая задача

                // Если задача не присвоена процессору
                if (curTask.IdProc == -1)
                {
                    bool emptyProc = false;
                    // Проверим все процессоры на занятость
                    for (int curProc = 0; curProc < procCount; curProc++)
                    {
                        // Если процессор не занят
                        if (procBusy[curProc] == false)
                        {
                            // Процессор занят задачей
                            procBusy[curProc] = true;

                            // Присваиваем процессор задаче
                            curTask.IdProc = curProc;

                            // Заносим информацию о том, что выполняется работа
                            this.ResultSingleMemory++;

                            // Если задача выполняется дольше кванта времени
                            if (curTask.Ticks > this.procTicksPerCycle)
                            {
                                // Отнимаем выполненные такты процесса
                                curTask.Work(this.procTicksPerCycle);
                                // Добавляем в очередь
                                procQueue.Enqueue(curTask);
                            }
                            // Если задача выполняется меньше кванта времени
                            else
                            {
                                // Обнуляем задачу
                                curTask.Work(this.procTicksPerCycle);
                                // Процессор теперь свободен
                                procBusy[curProc] = false;
                            }
                            curProc = this.procCount;
                            emptyProc = true;
                        }

                    }
                    if (emptyProc == false)
                    {
                        procQueue.Enqueue(curTask);
                    }
                }
                else
                {
                    // Заносим информацию о том, что выполняется работа
                    this.ResultSingleMemory++;

                    // Если задача выполняется дольше кванта времени
                    if (curTask.Ticks > this.procTicksPerCycle)
                    {
                        // Отнимаем выполненные такты процесса
                        curTask.Work(this.procTicksPerCycle);
                        // Добавляем в очередь
                        procQueue.Enqueue(curTask);
                    }
                    // Если задача выполняется меньше кванта времени
                    else
                    {
                        // Обнуляем задачу
                        curTask.Work(this.procTicksPerCycle);
                        // Процессор теперь свободен
                        procBusy[curTask.IdProc] = false;
                    }
                }
            }

            Console.WriteLine("Результат работы в однопоточном режиме: ");
            Console.WriteLine("Всего процессорных циклов: {0}", this.ResultSingleMemory);
            Console.WriteLine("Всего тактов затрачено: {0}", this.ResultSingleMemory * this.procTicksPerCycle);
        }

        // Запуск процесса выполнения задач с многопоточной реализацией
        private Queue<ProcTask> WorkMultiMemoryProcess(Queue<ProcTask> queue, int procNumber)
        {
            // Заносим информацию о том, что выполняется работа
            this.ResultMultiMemory++;

            // Текущая задача
            ProcTask curTask = queue.Dequeue();

            // Если задача выполняется дольше кванта времени
            if (curTask.Ticks > this.procTicksPerCycle)
            {
                // Передаём данные о процессоре
                curTask.IdProc = procNumber;
                // Отнимаем выполненные такты процесса
                curTask.Work(this.procTicksPerCycle);
                // Добавляем в очередь
                queue.Enqueue(curTask);
            }
            // Если задача выполняется меньше кванта времени
            else
            {
                // Обнуляем задачу
                curTask.Work(this.procTicksPerCycle);
            }
            return queue;
        }

        // Проверка на то, что все потоки пусты
        private bool WorkMultimemory_QueueIsEmpty(Queue<ProcTask>[] procMultiQueue)
        {
            bool queueIsEmpty = true;
            for (int curProc = 0; curProc < this.procCount; curProc++)
            {
                if (procMultiQueue[curProc].Count > 0)
                {
                    queueIsEmpty = false;
                }
            }
            return queueIsEmpty;
        }

        // Система управления процесса выполнения задач с многопоточным процессором
        public void WorkMultiMemory()
        {
            // Инициализируем очередь на обработку
            Queue<ProcTask> procQueue = this.GenerateQueue(this.taskCount);
            // Объявляем очереди на каждый процессор
            Queue<ProcTask>[] procMultiQueue = new Queue<ProcTask>[this.procCount];

            // Инициализация очередей
            for (int curProc = 0; curProc < this.procCount; curProc++)
                procMultiQueue[curProc] = new Queue<ProcTask>();

            int currentProcessor = 0;
            int currentQueue = 0;

            // Заполнение очередей
            int taskCount = procQueue.Count();
            for (int curTask = 0; curTask < taskCount; curTask++)
            {
                if (currentQueue >= this.procCount)
                    currentQueue = 0;
                procMultiQueue[currentQueue++].Enqueue(procQueue.Dequeue());
            }

            // Работа над очередями
            currentQueue = 0;
            while (!this.WorkMultimemory_QueueIsEmpty(procMultiQueue))
            {
                if (currentQueue >= this.procCount)
                {
                    currentQueue = 0;
                    currentProcessor = 0;
                }
                if (procMultiQueue[currentQueue].Count() > 0)
                {
                    procMultiQueue[currentQueue] = this.WorkMultiMemoryProcess(
                        procMultiQueue[currentQueue], currentProcessor++);
                }
                currentQueue++;
            }
            Console.WriteLine("Результат работы в многопоточном режиме: ");
            Console.WriteLine("Всего процессорных циклов: {0}", this.ResultMultiMemory);
            Console.WriteLine("Всего тактов затрачено: {0}", this.ResultMultiMemory * this.procTicksPerCycle);
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            ProcSingleMemory procSingleMemory = new ProcSingleMemory(4, 4, 20);
            procSingleMemory.WorkSingleMemory();
            procSingleMemory.WorkMultiMemory();

            Console.ReadKey();
        }
    }
}
