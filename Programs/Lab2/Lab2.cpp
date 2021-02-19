#include <cstdio>
#include <cmath>
#include <ctime>
#include <random>
#include <malloc.h>
#include <Windows.h>
#include <xmmintrin.h>

/// Вычисление функции средствами C++ и SSE
/// 2) f(x,y,z) = -(1/x) + x*y*z - 5*z
/// Автор: Селезнёв А.Э.

#define ARRAY_SIZE 100000000

void CreateArrays(float* x, float* y, float* z);
void ComputeWithCpp(float *x, float *y, float *z, float* result);
void ComputeWithSSE(float* x, float* y, float* z, float* result);

// Точка входа в программу
int main()
{
	float* x = (float*)_aligned_malloc(sizeof(float) * ARRAY_SIZE, 16);
	float* y = (float*)_aligned_malloc(sizeof(float) * ARRAY_SIZE, 16);
	float* z = (float*)_aligned_malloc(sizeof(float) * ARRAY_SIZE, 16);
	float* result = (float*)_aligned_malloc(sizeof(float) * ARRAY_SIZE, 16);
	printf("COMPLETE: Allocate memory to array. OUTPUT ARRAY: %d\n", ARRAY_SIZE);
	
	DWORD startTime, endTime;

	//Получаем время начала работы
	startTime = GetTickCount();

	// Заполняем массивы значениями
	CreateArrays(x, y, z);

	//Получаем время окончания работы
	endTime = GetTickCount();

	printf("COMPLETE: Fill array with values. Time: %d ms\n", (endTime - startTime));

	//Получаем время начала работы
	startTime = GetTickCount();

	// Вычисление средствами Cpp
	ComputeWithCpp(x, y, z, result);

	//Получаем время окончания работы
	endTime = GetTickCount();

	printf("COMPLETE: Calculate with C++. Time: %d ms\n", (endTime-startTime));

	//Получаем время начала работы
	startTime = GetTickCount();

	// Вычисление средствами SSE
	ComputeWithSSE(x, y, z, result);

	//Получаем время окончания работы
	endTime = GetTickCount();

	printf("COMPLETE: Calculate with SSE. Time: %d ms\n", (endTime - startTime));
	
	_aligned_free(x);
	_aligned_free(y);
	_aligned_free(z);
	_aligned_free(result);
	
	system("pause");
	return 0;
}

// Заполняем массив значениями
void CreateArrays(float* x, float* y, float* z)
{
	std::random_device random_device;
	std::mt19937 generator(random_device());
	std::uniform_real_distribution<> distribution_double(-100.f, 100.f);

	for (int i = 0; i < ARRAY_SIZE; i++)
	{
		x[i] = (float)distribution_double(generator);
		y[i] = (float)distribution_double(generator);
		z[i] = (float)distribution_double(generator);
	}
}

// Вычисление средствами C++
/// 2) f(x,y,z) = -(1/x) + x*y*z - 5*z
void ComputeWithCpp(float* x, float* y, float* z, float* result)
{
	float* px = x, * py = y, * pz = z, * presult = result;
	for (int i = 0; i < ARRAY_SIZE; i++)
	{
		*presult = -(1 / *px) + (*px * *py * *pz) - (5 * *pz);
		px++;
		py++;
		pz++;
		presult++;
	}
}

// Вычисление средствами SSE
/// 2) f(x,y,z) = -(1/x) + x*y*z - 5*z
void ComputeWithSSE(float* x, float* y, float* z, float* result)
{
	int nLoop = ARRAY_SIZE / 4;
	__m128 m1_1;
	__m128 m1_2;

	__m128 m2_1;
	__m128 m2_2;

	__m128 m3;
	__m128 m1and2;

	__m128 m_const_1 = _mm_set_ps1(1.f);
	__m128 m_const_m1 = _mm_set_ps1(-1.f);
	__m128 m_const_5 = _mm_set_ps1(5.f);

	__m128* px = (__m128*) x;
	__m128* py = (__m128*) y;
	__m128* pz = (__m128*) z;
	__m128* presult = (__m128*) result;

	for (int i = 0; i < nLoop; i++)
	{
		m1_1 = _mm_div_ps(m_const_1, *px);
		m1_2 = _mm_mul_ps(m1_1, m_const_m1);

		m2_1 = _mm_mul_ps(*px, *py);
		m2_2 = _mm_mul_ps(m2_1, *pz);

		m3 = _mm_mul_ps(*pz, m_const_5);

		m1and2 = _mm_add_ps(m1_2, m2_2);
		*presult = _mm_sub_ps(m1_2, m3);

		px++;
		py++;
		pz++;
		presult++;
	}
}