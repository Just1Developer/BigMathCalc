using System.Numerics;
using System.Text;

namespace ConsoleApp1;

/// <summary>
/// Very neat class for rational numbers. Does not round to precision. Has no limit. Might use up some space though.
/// </summary>
public class RationalNumber
{
    public BigInteger Numerator { get; set; }      // Zähler
    private BigInteger _denominator;
    public BigInteger Denominator     // Nenner
    {
        get => _denominator;
        set
        {
            if (value == BigInteger.Zero)
                throw new DivideByZeroException();
            _denominator = value;
        }
    }
    public bool IsNegative { get; private set; }
    
    public int Sign
    {
        get => IsNegative ? -1 : 1;
    }
    
    public BigInteger Value
    {
        get => Numerator / Denominator;
    }

    #region Constructors
    
    private RationalNumber(BigInteger numerator, BigInteger denominator, bool isNegative)
    {
        this.Numerator = numerator;
        this.Denominator = denominator;
        this.IsNegative = isNegative;
        Simplify();
    }

    private RationalNumber(BigInteger numerator, BigInteger denominator, decimal sign)
        : this(numerator, denominator, sign < 0) { }

    public RationalNumber(int numerator, int denominator = 1)
    {
        this.Numerator = Math.Abs(numerator);
        this.Denominator = denominator;
        this.IsNegative = numerator < 0;
        Simplify();
    }

    public RationalNumber(double value)
    {
        double val = Math.Abs(value / 1.0);
        Numerator = (long)val;
        long exp = 0;
        value = Math.Abs(value);
        value %= 1.0;

        while (value % 1.0 > 0)
        {
            value *= 10;
            exp--;
        }

        Denominator = (long) Math.Pow(10, exp);
        Numerator *= (long) Math.Pow(10, exp);
        Numerator += (long) value;
        Simplify();
    }

    public RationalNumber(decimal value) : this((double)value)
    {
    }

    public RationalNumber(float value) : this((double)value)
    {
    }

    public RationalNumber(int value) : this((double)value)
    {
    }

    public RationalNumber(long value) : this((double)value)
    {
    }

    public RationalNumber(string value)
    {
        if (value.StartsWith('-'))
        {
            IsNegative = true;
            value = value.Substring(1);
        }

        if (value.Contains("/"))
        {
            var split = value.Split('/');
            string nom = split[0], denom = split[1];
            var nomnom = ParseDecimal(nom);
            var nomdom = ParseDecimal(denom);

            Numerator = nomnom.Item1 * nomdom.Item2;
            Denominator = nomnom.Item2 * nomdom.Item1;
            Simplify();
            return;
        }

        var values = ParseDecimal(value);
        Numerator = values.Item1;
        Denominator = values.Item2;
        
        Simplify();
    }

    private static (BigInteger, BigInteger) ParseDecimal(string value)
    {
        int commaIndex = value.IndexOf('.');
        if (commaIndex == -1) commaIndex = value.IndexOf(',');

        BigInteger numerator = BigInteger.Zero, denominator = BigInteger.Zero;

        bool stageDecimals = commaIndex != -1, stageTrailingZeros = !stageDecimals;
        BigInteger currentDecimalValue = 1;

        long powerOfTen = 0;

        for (int i = value.Length - 1; i >= 0; i--)
        {
            if (i == commaIndex)
            {
                stageDecimals = false;
                continue;
            }

            if (value[i] < '0' || value[i] > '9') throw new FormatException($"\"{value}\" is not a valid decimal.");

            if (stageDecimals)
            {
                numerator += (value[i] - '0') * currentDecimalValue;
                currentDecimalValue *= 10;
                powerOfTen--;
                continue;
            }

            stageTrailingZeros = stageTrailingZeros && value[i] == '0';

            if (stageTrailingZeros)
            {
                powerOfTen++;
                continue;
            }
            
            numerator += (value[i] - '0') * currentDecimalValue;
            currentDecimalValue *= 10;
        }

        if (powerOfTen < 0) denominator = (long) Math.Pow(10, -powerOfTen);
        else
        {
            denominator = 1;
            numerator *= (long) Math.Pow(10, powerOfTen);
        }

        return (numerator, denominator);
    }
    
    #endregion

    
    private void Contract()
    {
        // Removes trailing zeros
        while (Numerator % 10 == 0 && Numerator > 0 && Denominator % 10 == 0 && Denominator > 0)
        {
            Numerator /= 10;
            Denominator /= 10;
        }
    }

    /**
     * Kürzen der Zahl. Returns itself.
     */
    public RationalNumber Simplify()
    {
        BigInteger divisor = BigInteger.GreatestCommonDivisor(Numerator, Denominator);
        this.Numerator /= divisor;
        this.Denominator /= divisor;
        return this;
    }
    
    /**
     * Returns Value as double if possible.
     * Returns 0.0 if the value is too large for a double to hold.
     */
    public double TryGetAsDouble()
    {
        try
        {
            double value = (long)Numerator;
            value /= (long)Denominator;
            return value * Sign;
        }
        catch (OverflowException)
        {
            return 0;
        }
    }

    #region Operator Overloading

    public static RationalNumber operator +(RationalNumber number, RationalNumber other)
    {
        int signThis = number.Sign, otherSign = other.Sign;

        BigInteger numeratorThis = number.Numerator * other.Denominator;         // Expand by other denominator
        BigInteger numeratorOther = other.Numerator * number.Denominator;        // Expand by other denominator
        BigInteger newDenominator = number.Denominator * other.Denominator;

        BigInteger newValue = numeratorThis * signThis + numeratorOther * otherSign;
        bool isNegative = newValue.Sign == -1;
        if (isNegative) newValue *= -1;

        return new RationalNumber(newValue, newDenominator, isNegative).Simplify();
    }

    public static RationalNumber operator -(RationalNumber number, RationalNumber other)
    {
        return number + Negative(other);
    }

    public static RationalNumber operator *(RationalNumber number, RationalNumber other)
    {
        BigInteger newNumerator = number.Numerator * other.Numerator;
        BigInteger newDenominator = number.Denominator * other.Denominator;

        bool isNegative = number.IsNegative ^ other.IsNegative;
        
        return new RationalNumber(newNumerator, newDenominator, isNegative).Simplify();
    }

    public static RationalNumber operator /(RationalNumber number, RationalNumber other)
    {
        return number * Reciprocal(other);
    }

    public static bool operator >(RationalNumber number, RationalNumber other)
    {
        // Get to same denominator and compare numerators
        BigInteger numeratorThis = number.Numerator * other.Denominator;
        BigInteger numeratorOther = other.Numerator * number.Denominator;

        return numeratorThis > numeratorOther;
    }

    public static bool operator <(RationalNumber number, RationalNumber other)
    {
        // Get to same denominator and compare numerators
        BigInteger numeratorThis = number.Numerator * other.Denominator;
        BigInteger numeratorOther = other.Numerator * number.Denominator;

        return numeratorThis < numeratorOther;
    }

    public static bool operator ==(RationalNumber number, RationalNumber other)
    {
        return number.Numerator == other.Numerator
               && number.Denominator == other.Denominator
               && number.IsNegative == other.IsNegative;
    }

    public static bool operator !=(RationalNumber number, RationalNumber other)
    {
        return !(number == other);
    }

    public static bool operator <=(RationalNumber number, RationalNumber other)
    {
        return number < other || number == other;
    }

    public static bool operator >=(RationalNumber number, RationalNumber other)
    {
        return number > other || number == other;
    }

    public static RationalNumber Abs(RationalNumber number)
    {
        return new RationalNumber(number.Numerator, number.Denominator, false);
    }

    public static RationalNumber Negative(RationalNumber positive)
    {
        return new RationalNumber(positive.Numerator, positive.Denominator, true);
    }

    public static RationalNumber AdditiveInverse(RationalNumber number)
    {
        return new RationalNumber(number.Numerator, number.Denominator, !number.IsNegative);
    }

    /**
     * Kehrwert, n => 1/n
     */
    public static RationalNumber Reciprocal(RationalNumber number)
    {
        return new RationalNumber(number.Denominator, number.Numerator, number.IsNegative);
    }

    
    #region Other Type Operator Overloading
    
    
    public static RationalNumber operator +(RationalNumber number, decimal other)
    {
        return number + new RationalNumber(other);
    }

    public static RationalNumber operator -(RationalNumber number, decimal other)
    {
        return number - new RationalNumber(other);
    }

    public static RationalNumber operator *(RationalNumber number, decimal other)
    {
        return number * new RationalNumber(other);
    }

    public static RationalNumber operator /(RationalNumber number, decimal other)
    {
        return number / new RationalNumber(other);
    }

    public static RationalNumber operator ^(RationalNumber number, int exponent)
    {
        return new RationalNumber(BigInteger.Pow(number.Numerator, exponent), BigInteger.Pow(number.Denominator, exponent), (int) Math.Pow(number.Sign, exponent));
    }
    
    public static bool operator >(RationalNumber number, decimal other)
    {
        RationalNumber rationalOther = new RationalNumber(other);
        return number > rationalOther;
    }

    public static bool operator <(RationalNumber number, decimal other)
    {
        RationalNumber rationalOther = new RationalNumber(other);
        return number < rationalOther;
    }

    public static bool operator ==(RationalNumber number, decimal other)
    {
        RationalNumber rationalOther = new RationalNumber(other);
        return number.Numerator == rationalOther.Numerator
               && number.Denominator == rationalOther.Denominator
               && number.IsNegative == rationalOther.IsNegative;
    }

    public static bool operator !=(RationalNumber number, decimal other)
    {
        return !(number == other);
    }

    public static bool operator <=(RationalNumber number, decimal other)
    {
        RationalNumber rationalOther = new RationalNumber(other);
        return number < rationalOther || number == rationalOther;
    }

    public static bool operator >=(RationalNumber number, decimal other)
    {
        RationalNumber rationalOther = new RationalNumber(other);
        return number > rationalOther || number == rationalOther;
    }
    
    // ---------------------------------------------------------------------------
    
    public static RationalNumber operator +(decimal number, RationalNumber other)
    {
        return new RationalNumber(number) + other;
    }

    public static RationalNumber operator -(decimal number, RationalNumber other)
    {
        return new RationalNumber(number) - other;
    }

    public static RationalNumber operator *(decimal number, RationalNumber other)
    {
        return new RationalNumber(number) * other;
    }

    public static RationalNumber operator /(decimal number, RationalNumber other)
    {
        return new RationalNumber(number) / other;
    }
    
    public static bool operator >(decimal number, RationalNumber other)
    {
        RationalNumber rationalNumber = new RationalNumber(number);
        return rationalNumber > other;
    }

    public static bool operator <(decimal number, RationalNumber other)
    {
        RationalNumber rationalNumber = new RationalNumber(number);
        return rationalNumber < other;
    }

    public static bool operator ==(decimal number, RationalNumber other)
    {
        RationalNumber rationalNumber = new RationalNumber(number);
        return rationalNumber.Numerator == other.Numerator
               && rationalNumber.Denominator == other.Denominator
               && rationalNumber.IsNegative == other.IsNegative;
    }

    public static bool operator !=(decimal number, RationalNumber other)
    {
        return !(number == other);
    }

    public static bool operator <=(decimal number, RationalNumber other)
    {
        RationalNumber rationalNumber = new RationalNumber(number);
        return rationalNumber < other || rationalNumber == other;
    }

    public static bool operator >=(decimal number, RationalNumber other)
    {
        RationalNumber rationalNumber = new RationalNumber(number);
        return rationalNumber > other || rationalNumber == other;
    }

    
    #endregion

    
    /**
     * Returns the bigger rational number.
     * If the numbers are equal, returns the first number.
     * Returns the objects, not a copy.
     */
    public static RationalNumber Max(RationalNumber number, RationalNumber other)
    {
        return number >= other ? number : other;
    }

    /**
     * Returns the smaller rational number.
     * If the numbers are equal, returns the first number.
     * Returns the objects, not a copy.
     */
    public static RationalNumber Min(RationalNumber number, RationalNumber other)
    {
        return number <= other ? number : other;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null) return false;
        if (obj.GetType() == typeof(RationalNumber))
        {
            return (RationalNumber) obj == this;
        }
        if (obj is BigInteger)
        {
            return (BigInteger) obj == Value;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    #endregion

    public override String ToString()
    {
        try
        {
            return $"({(IsNegative ? "-" : "")}{Numerator}/{Denominator}, = {ToSingleString()})";
        }
        catch (Exception)
        {
            // If an exception occured when trying to cast the Denominator to an int. Shouldn't occur but who knows.
            return $"({(IsNegative ? "-" : "")}{Numerator}/{Denominator}, = {{Denominator Integer Overflow}})";
        }
    }

    private String ToSingleString()
    {
        StringBuilder result = new StringBuilder();
        BigInteger integerPart = Numerator / Denominator;
        BigInteger remainder = Numerator % Denominator;

        result.Append(integerPart.ToString());  // Add the integer part

        if (remainder != 0)
        {
            result.Append(".");  // Add the decimal point

            // For keeping track of repeating decimals
            Dictionary<BigInteger, int> seenRemainders = new Dictionary<BigInteger, int>();

            while (remainder != 0)
            {
                if (seenRemainders.TryGetValue(remainder, out int pos))
                {
                    // Insert brackets to indicate repeating decimals
                    result.Insert(pos, "[");
                    result.Append("]");
                    break;
                }

                seenRemainders[remainder] = result.Length;

                remainder *= 10;
                BigInteger decimalPart = remainder / Denominator;
                result.Append(decimalPart.ToString());
                remainder %= Denominator;
            }
        }

        if (IsNegative) result.Insert(0, '-');
        
        return result.ToString();
    }
}