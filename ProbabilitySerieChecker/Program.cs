using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace ProbabilitySerieChecker
{
	class Program
	{
		static void Main(string[] args)
		{

			int n = 2;
			int k = 4;

			Console.WriteLine(getResult(n, k, (a => oneOverRToK(a, 2))));
		}


		static Rational oneOverRToK(Rational r, int K)
		{
			Rational result = r;
			for (int i = 1; i < K; i++)
			{
				result = result * r;
			}

			result.invert();
			return result;
		}

		static Rational getProbArgument(int i, int n, int k, Func<Rational, Rational> transformFunction)
		{
			Rational result = new Rational(i + k * n - 1, n - i);
			return transformFunction(result);
		}

		static Rational getProduct(int n, int k, Func<Rational, Rational> transformFunction)
		{
			Rational result = new Rational(1);
			for (int i = 1; i < n; i++)
			{
				Rational factor = new Rational(1) - getProbArgument(i, n, k, transformFunction);
				result = result * factor;
			}
			return result;
		}

		static Rational getResult(int n, int k, Func<Rational, Rational> transformFunction)
		{
			return new Rational(1) - getProduct(n, k, transformFunction);
		}
	}
}
