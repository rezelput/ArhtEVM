#include <stdio.h>
#include <math.h>
#include "getCPUTime.c"

const int N = 100000000;
const int x = 2;
const int AttemptCount = 16;

int main()
{
	// Массив со значенияи процессорного времени
	double *CPUTimeArray=(double*)malloc(AttemptCount*sizeof(double));
	// Значение суммы всех вычислений
	double CPUTimeSummary = 0;
	
	for(int attempt = 0; attempt < AttemptCount; attempt++)
	{
		// Переменные для хранения времени процессора
		double startTime, endTime;
		// Считываем начальное аремя работы процесса
		startTime = getCPUTime();
		
		// Вычисление согласно варианту
		double fx = 0;
		for (int i = 1; i <= N; i++)
			fx += (sin(i) + cos(x * pow(i, -1)));
		
		// Считываем конечное время работы процесса
		endTime = getCPUTime();
		// Вычисляем и записываем время работы участка кода
		CPUTimeArray[attempt] = (endTime - startTime);
		CPUTimeSummary += CPUTimeArray[attempt];
		
		printf("Attempt %d : F(x) = %lf\n", attempt, fx);
		fprintf( stderr, "Attempt %d : CPUTime = %lf\n\n", attempt, CPUTimeArray[attempt]);
	}

	double average = (CPUTimeSummary/AttemptCount);
	printf("Average = %lf\n", average);
	
	// Вычисляем дисперсию
	double dispersion = 0;
	for(int attempt = 0; attempt < AttemptCount; attempt++)
	{
		dispersion += (CPUTimeArray[attempt] - average) * (CPUTimeArray[attempt] - average);
	}
	dispersion /= (AttemptCount - 1);

	
	printf("Dispersion = %lf\n", dispersion);
	
	return 0;
}