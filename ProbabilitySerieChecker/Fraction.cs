using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace ProbabilitySerieChecker
{
	class Fraction
	{
		public long Numerator, Denominator;

		// Initialize the fraction from a string A/B.
		public Fraction(string txt)
		{
			string[] pieces = txt.Split('/');
			Numerator = long.Parse(pieces[0]);
			Denominator = long.Parse(pieces[1]);
			Simplify();
		}

		// Initialize the fraction from numerator and denominator.
		public Fraction(long numer, long denom)
		{
			Numerator = numer;
			Denominator = denom;
			Simplify();
		}

		// Return a * b.
		public static Fraction operator *(Fraction a, Fraction b)
		{
			// Swap numerators and denominators to simplify.
			Fraction result1 = new Fraction(a.Numerator, b.Denominator);
			Fraction result2 = new Fraction(b.Numerator, a.Denominator);

			return new Fraction(
				result1.Numerator * result2.Numerator,
				result1.Denominator * result2.Denominator);
		}

		// Return -a.
		public static Fraction operator -(Fraction a)
		{
			return new Fraction(-a.Numerator, a.Denominator);
		}

		// Return a + b.
		public static Fraction operator +(Fraction a, Fraction b)
		{
			// Get the denominators' greatest common divisor.
			long gcd_ab = MathStuff.GCD(a.Denominator, b.Denominator);

			long numer =
				a.Numerator * (b.Denominator / gcd_ab) +
				b.Numerator * (a.Denominator / gcd_ab);
			long denom =
				a.Denominator * (b.Denominator / gcd_ab);
			return new Fraction(numer, denom);
		}

		// Return a - b.
		public static Fraction operator -(Fraction a, Fraction b)
		{
			return a + -b;
		}

		// Return a / b.
		public static Fraction operator /(Fraction a, Fraction b)
		{
			return a * new Fraction(b.Denominator, b.Numerator);
		}

		// Simplify the fraction.
		private void Simplify()
		{
			// Simplify the sign.
			if (Denominator < 0)
			{
				Numerator = -Numerator;
				Denominator = -Denominator;
			}

			// Factor out the greatest common divisor of the
			// numerator and the denominator.
			long gcd_ab = MathStuff.GCD(Numerator, Denominator);
			Numerator = Numerator / gcd_ab;
			Denominator = Denominator / gcd_ab;
		}

		// Convert a to a double.
		public static implicit operator double(Fraction a)
		{
			return (double)a.Numerator / a.Denominator;
		}

		// Return the fraction's value as a string.
		public override string ToString()
		{
			return Numerator.ToString() + "/" + Denominator.ToString();
		}
	}

	public class Rational
	{
		private BigInteger _numerator = 0;
		private BigInteger _denominator = 1;

		// コンストラクタ
		public Rational(BigInteger int_value)
		{
			set(int_value, 1);
		}

		public Rational(BigInteger new_numerator, BigInteger new_denominator)
		{
			set(new_numerator, new_denominator);
		}

		// アクセサ
		public void set(BigInteger new_numerator, BigInteger new_denominator)
		{
			if (new_denominator == 0)
			{
				throw new ArithmeticException("Denominator must not be 0");
			}

			_numerator = new_numerator;
			_denominator = new_denominator;
		}

		public BigInteger numerator()
		{
			return _numerator;
		}

		public void numerator(BigInteger new_numerator)
		{
			_numerator = new_numerator;
		}

		public BigInteger denominator()
		{
			return _denominator;
		}

		public void denominator(BigInteger new_denominator)
		{
			if (new_denominator == 0)
			{
				throw new ArithmeticException("Denominator must not be 0");
			}
			_denominator = new_denominator;
		}

		// ---------- 補助関数 ----------

		// 最大公約数
		public static BigInteger gcd(BigInteger v1, BigInteger v2)
		{
			BigInteger tmp;

			// どちらかが0だったら即座に終了
			if (v1 == 0 || v2 == 0) return 0;

			// 正の値にしておく
			if (v1 < 0) v1 = -v1;
			if (v2 < 0) v2 = -v2;

			// v1の方を大きくしておく
			if (v2 > v1)
			{
				tmp = v1; v1 = v2; v2 = tmp;
			}

			for (; ; )
			{
				tmp = v1 % v2;
				if (tmp == 0) return v2;

				v1 = v2; v2 = tmp;
			}
		}

		// 通分する
		private void _fix_denominator(Rational other)
		{
			BigInteger tmp = _denominator;
			_numerator *= other._denominator;
			_denominator *= other._denominator;

			other._numerator *= tmp;
			other._denominator *= tmp;
		}

		// 正規化
		// ●分子・分母を約分する
		// ●負の符号が分母についている場合、分子にのみつけるようにする
		// ●値が0である場合は「0/1」にする
		private void _regularize()
		{
			int sign = _denominator.Sign;
			BigInteger divisor = sign * gcd(_numerator, _denominator);
			if (divisor == 0)
			{
				// 分子が0の場合
				_numerator = 0;
				_denominator = 1;
			}
			else
			{
				_numerator /= divisor;
				_denominator /= divisor;
			}
		}

		// ---------- 比較 ----------

		public static bool operator ==(Rational r1, Rational r2)
		{
			r1._regularize();
			r2._regularize();
			return (r1._numerator == r2._numerator && r1._denominator == r2._denominator);
		}

		public static bool operator !=(Rational r1, Rational r2)
		{
			return (r1 != r2);
		}

		public static bool operator <(Rational r1, Rational r2)
		{
			r1._fix_denominator(r2);
			return (r1.numerator() < r2.numerator());
		}

		public static bool operator >(Rational r1, Rational r2)
		{
			return (r2 < r1);
		}

		// 以下、他の関数やクラスに使ってもらう際の補助関数
		public override bool Equals(object obj)
		{
			if (obj.GetType() == this.GetType())
			{
				return (this == (Rational)obj);
			}

			return false;
		}

		public override int GetHashCode()
		{
			return (_numerator.GetHashCode() | (_denominator.GetHashCode() << 16));
		}

		// ---------- 型変換 ----------

		public static explicit operator double(Rational r)
		{
			return (double)r._numerator / (double)r._denominator;
		}

		public double eval()
		{
			return (double)this;
		}

		public static explicit operator float(Rational r)
		{
			return (float)r._numerator / (float)r._denominator;
		}

		// ---------- 加減乗除 ----------

		public static Rational operator +(Rational r) // 単項演算子
		{
			// 単に自分のコピーを作ればよい
			return new Rational(r._numerator, r._denominator);
		}

		public static Rational operator -(Rational r) // 単項演算子
		{
			return new Rational(-r._numerator, r._denominator);
		}

		public static Rational operator +(Rational r1, Rational r2) // 二項演算子
		{
			r1._fix_denominator(r2);
			return new Rational(r1._numerator + r2._numerator, r1._denominator);
		}

		public static Rational operator -(Rational r1, Rational r2) // 二項演算子
		{
			r1._fix_denominator(r2);
			return new Rational(r1._numerator - r2._numerator, r1._denominator);
		}

		public static Rational operator *(Rational r1, Rational r2)
		{
			return new Rational(r1._numerator * r2._numerator, r1._denominator * r2._denominator);
		}

		public static Rational operator /(Rational r1, Rational r2)
		{
			if (r2._numerator == 0)
			{
				throw new DivideByZeroException();
			}
			return new Rational(r1._numerator * r2._denominator, r1._denominator * r2._numerator);
		}

		// ユーティリティ
		public override string ToString()
		{
			_regularize();
			if (_denominator == 1) return _numerator.ToString();

			return string.Format("({0}/{1})", _numerator, _denominator);
		}

		/// <summary>
		/// Transforms this rational (a/b) to its inversion - ie. (b/a)
		/// </summary>
		public void invert()
		{
			BigInteger h = _numerator;
			_numerator = _denominator;
			_denominator = h;
		}

	}

}
