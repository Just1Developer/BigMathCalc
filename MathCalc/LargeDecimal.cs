using System.Numerics;
using System.Text;

namespace ConsoleApp1;

public class LosslessDecimal
{
    private BigInteger mantisse;
    private BigInteger exponent;
    private bool negative;
    
    public int Sign
    {
        get => negative ? -1 : 1;
    }
    
    public BigInteger Value
    {
        get => mantisse * (BigInteger) Math.Pow(10, (long) exponent);
    }

    #region Constructors
    
    private LosslessDecimal(BigInteger positiveValue, BigInteger exponent, bool negative)
    {
        mantisse = positiveValue;
        this.exponent = exponent;
        this.negative = negative;
        Contract();
    }

    public LosslessDecimal(int value, int exponent = 0)
    {
        mantisse = Math.Abs(value);
        this.exponent = exponent;
        negative = value < 0;
    }

    public LosslessDecimal(double value)
    {
        double val = Math.Abs(value / 1.0);
        mantisse = (long)val;
        long exp = 0;
        value = Math.Abs(value);
        value %= 1.0;

        while (value % 1.0 > 0)
        {
            value *= 10;
            exp--;
        }

        exponent = exp;
        mantisse *= (long)Math.Pow(10, exp);
        mantisse += (long)value;
    }

    public LosslessDecimal(decimal value) : this((double)value)
    {
    }

    public LosslessDecimal(float value) : this((double)value)
    {
    }

    public LosslessDecimal(int value) : this((double)value)
    {
    }

    public LosslessDecimal(long value) : this((double)value)
    {
    }

    public LosslessDecimal(string value)
    {
        if (value.StartsWith('-'))
        {
            negative = true;
            value = value.Substring(1);
        }
        
        int commaIndex = value.IndexOf('.');
        if (commaIndex == -1) commaIndex = value.IndexOf(',');

        mantisse = 0;
        exponent = 0;

        bool stageDecimals = commaIndex != -1, stageTrailingZeros = !stageDecimals;
        BigInteger currentDecimalValue = 1;

        for (int i = value.Length - 1; i >= 0; i--)
        {
            if (i == commaIndex)
            {
                stageDecimals = false;
                exponent--;
                continue;
            }

            if (value[i] < '0' || value[i] > '9') throw new FormatException($"\"{value}\" is not a valid decimal.");

            if (stageDecimals)
            {
                mantisse += (value[i] - '0') * currentDecimalValue;
                currentDecimalValue *= 10;
                exponent--;
                continue;
            }

            exponent++;

            if (stageTrailingZeros)
            {
                if (value[i] == '0')
                    continue;

                mantisse += (value[i] - '0') * currentDecimalValue;
                currentDecimalValue *= 10;
                stageTrailingZeros = false;
                continue;
            }

            mantisse += (value[i] - '0') * currentDecimalValue;
            currentDecimalValue *= 10;
        }
    }
    
    #endregion

    
    private void Contract()
    {
        // Removes trailing zeros
        while (mantisse % 10 == 0 && mantisse > 0)
        {
            mantisse /= 10;
            exponent++;
        }
    }
    
    public double TryGetAsDouble()
    {
        double value = (long)this.mantisse;
        value *= Math.Pow(10, (long)exponent);
        return value;
    }

    #region Operator Overloading

    public static LosslessDecimal operator +(LosslessDecimal number, LosslessDecimal other)
    {
        int signThis = number.Sign, otherSign = other.Sign;
        
        BigInteger value = number.mantisse * signThis;
        BigInteger exponent = number.exponent;

        // 1. Get both exponents to lower
        // 2. Adjust mantisse of the higher one accordingly.
        // 3. Profit

        if (other.exponent < exponent)
        {
            // we must use other.exponent here, adjust the current value accordingly.

            BigInteger exponentDelta = exponent - other.exponent;
            value *= (long)Math.Pow(10, (double)exponentDelta);
            exponent = other.exponent;
            value += other.mantisse * otherSign;
        }
        else if (other.exponent > exponent)
        {
            // Use the exponent we already have, adjust the other value accordingly.
            BigInteger exponentDelta = other.exponent - exponent;
            value += other.mantisse * (long)Math.Pow(10, (long)exponentDelta) * otherSign;      // Todo is this right?
        }
        else
        {
            value += other.mantisse * otherSign;
        }

        bool negative = value.Sign < 0;
        if (negative) value *= -1;
        
        return new LosslessDecimal(value, exponent, negative);
    }

    public static LosslessDecimal operator -(LosslessDecimal number, LosslessDecimal other)
    {
        return number + Negative(other);
    }

    public static LosslessDecimal operator *(LosslessDecimal number, LosslessDecimal other)
    {
        int signThis = number.Sign, otherSign = other.Sign;
        
        BigInteger value = number.mantisse * signThis;
        BigInteger exponent = number.exponent + other.exponent;

        // 1. Get both exponents to higher
        // 2. Multiply mantisses
        // 3. Profit

        value *= other.mantisse;
        
        bool negative = value.Sign < 0;
        if (negative) value *= -1;
        
        return new LosslessDecimal(value, exponent, negative);
    }

    public static LosslessDecimal operator /(LosslessDecimal number, LosslessDecimal other)
    {
        return number * Reciprocal(other);
    }

    public static LosslessDecimal operator +(LosslessDecimal number, decimal other)
    {
        return number + new LosslessDecimal(other);
    }

    public static LosslessDecimal operator -(LosslessDecimal number, decimal other)
    {
        return number - new LosslessDecimal(other);
    }

    public static LosslessDecimal operator *(LosslessDecimal number, decimal other)
    {
        return number * new LosslessDecimal(other);
    }

    public static LosslessDecimal operator /(LosslessDecimal number, decimal other)
    {
        return number / new LosslessDecimal(other);
    }

    public static bool operator >(LosslessDecimal number, LosslessDecimal other)
    {
        if (number.exponent > other.exponent) return true;
        if (number.exponent < other.exponent) return false;
        return number.mantisse > number.exponent;
    }

    public static bool operator <(LosslessDecimal number, LosslessDecimal other)
    {
        if (number.exponent < other.exponent) return true;
        if (number.exponent > other.exponent) return false;
        return number.mantisse < number.exponent;
    }

    public static bool operator ==(LosslessDecimal number, LosslessDecimal other)
    {
        return number.mantisse == other.mantisse && number.exponent == other.exponent;
    }

    public static bool operator !=(LosslessDecimal number, LosslessDecimal other)
    {
        return !(number == other);
    }

    public static bool operator <=(LosslessDecimal number, LosslessDecimal other)
    {
        return number < other || number == other;
    }

    public static bool operator >=(LosslessDecimal number, LosslessDecimal other)
    {
        return number > other || number == other;
    }

    public static LosslessDecimal Abs(LosslessDecimal number)
    {
        return new LosslessDecimal(number.mantisse, number.exponent, false);
    }

    public static LosslessDecimal Negative(LosslessDecimal positive)
    {
        return new LosslessDecimal(positive.mantisse, positive.exponent, true);
    }

    /**
     * Kehrwert, n => 1/n
     */
    public static LosslessDecimal Reciprocal(LosslessDecimal number)
    {
        // Could also just return 0.
        if (number.mantisse == 0) return new LosslessDecimal(0, -number.exponent, number.negative);
        
        BigInteger divisorHelp = (long) Math.Pow(10, number.mantisse.ToString().Length);  // Should be exactly enough
        BigInteger newMantisse = divisorHelp / number.mantisse;
        
        return new LosslessDecimal(newMantisse, -number.exponent, number.negative);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null) return false;
        if (obj.GetType() == typeof(LosslessDecimal))
        {
            return (LosslessDecimal) obj == this;
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
            return $"({(negative ? "-" : "")}{mantisse}, {exponent}, = {ToSingleString()})";
        }
        catch (Exception)
        {
            // If an exception occured when trying to cast the exponent to an int. Shouldn't occur but who knows.
            return $"({(negative ? "-" : "")}{mantisse}, {exponent}, = {{Exponent Integer Overflow}})";
        }
    }

    private String ToSingleString()
    {
        StringBuilder builder = new StringBuilder();
        string mStr = mantisse.ToString();
        
        builder.Append(mStr);
        // Append trailing zeros (if there are any) or insert the decimal point
        if (exponent < 0)
        {
            int index = mStr.Length + (int) exponent;
            while (index < 0)
            {
                builder.Insert(0, '0');
                index++;
            }
            // Insert decimal point
            builder.Insert(index, '.');
            if (index == 0) builder.Insert(0, '0');
        }
        else if (mantisse > 0)
        {
            for (int i = 0; i < exponent; i++)
                builder.Append('0');
        }
        
        if (negative) builder.Insert(0, '-');
        return builder.ToString();
    }
    
}