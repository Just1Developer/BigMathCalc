// See https://aka.ms/new-console-template for more information

using ConsoleApp1;

Console.WriteLine("Hello, World!");

/*
var num1 = new LosslessDecimal(1, 1);
var num2 = new LosslessDecimal(2, 0);
var num3 = num1 + num2;
var num4 = new LosslessDecimal("-0.12345");
var num5 = num4 + num2;
var num6 = num4 + num2 + num1;

Console.WriteLine($"num1: {num1}, num2: {num2}, num3: {num3}, num1 value: {num1.TryGetAsDouble()}, num2 value: {num2.TryGetAsDouble()}, num3 value: {num3.TryGetAsDouble()}");
Console.WriteLine($"num4: {num4}, num4 value: {num4.TryGetAsDouble()}");
Console.WriteLine($"num5: {num5}, num5 value: {num5.TryGetAsDouble()}");
Console.WriteLine($"num6: {num6}, num6 value: {num6.TryGetAsDouble()}");

var x = new LosslessDecimal("0.1") + new LosslessDecimal("0.2");
Console.WriteLine($"x: {x}, x value: {x.TryGetAsDouble()}");
x *= 3;
Console.WriteLine($"x: {x}, x value: {x.TryGetAsDouble()}");
x *= num4;
Console.WriteLine($"x: {x}, x value: {x.TryGetAsDouble()}");
var num7 = new LosslessDecimal("0.0000000242");
Console.WriteLine($"num7: {num7}, num7 value: {num7.TryGetAsDouble()}");
num7 *= x;
Console.WriteLine($"num7: {num7}, num7 value: {num7.TryGetAsDouble()}");

Console.WriteLine($"Kehrwert von 2 = 1/2 = 0.5: " + LosslessDecimal.Reciprocal(new LosslessDecimal("2")));
//*/

//*
var num1 = new RationalNumber(10);
var num2 = new RationalNumber(2);
var num3 = num1 + num2;
var num4 = new RationalNumber("-0.12345");
var num5 = num4 + num2;
var num6 = num4 + num2 + num1;

Console.WriteLine($"num1: {num1}, num2: {num2}, num3: {num3}, num1 value: {num1.TryGetAsDouble()}, num2 value: {num2.TryGetAsDouble()}, num3 value: {num3.TryGetAsDouble()}");
Console.WriteLine($"num4: {num4}, num4 value: {num4.TryGetAsDouble()}");
Console.WriteLine($"num5: {num5}, num5 value: {num5.TryGetAsDouble()}");
Console.WriteLine($"num6: {num6}, num6 value: {num6.TryGetAsDouble()}");

var x = new RationalNumber("0.1") + new RationalNumber("0.2");
Console.WriteLine($"x: {x}, x value: {x.TryGetAsDouble()}");
x *= 3;
Console.WriteLine($"x: {x}, x value: {x.TryGetAsDouble()}");
x *= num4;
Console.WriteLine($"x: {x}, x value: {x.TryGetAsDouble()}");
var num7 = new RationalNumber("0.0000000242");
Console.WriteLine($"num7: {num7}, num7 value: {num7.TryGetAsDouble()}");
num7 *= x;
Console.WriteLine($"num7: {num7}, num7 value: {num7.TryGetAsDouble()}");

Console.WriteLine($"Kehrwert von 2 = 1/2 = 0.5: " + RationalNumber.Reciprocal(new RationalNumber("2")));

Console.WriteLine("Maximums:");

var _double = new RationalNumber(double.MaxValue);
Console.WriteLine($"_double: {_double}, _double value: {_double.TryGetAsDouble()}");
_double *= 2;
Console.WriteLine($"_double * 2: {_double}, _double value * 2: {_double.TryGetAsDouble()}");

var _long = new RationalNumber(long.MaxValue);
Console.WriteLine($"_long: {_long}, _long value: {_long.TryGetAsDouble()}");
_long *= 2;
Console.WriteLine($"_long * 2: {_long}, _long value * 2: {_long.TryGetAsDouble()}");

var onethird = new RationalNumber("1/3");
Console.WriteLine($"1/3: {onethird}");
Console.WriteLine($"1/3^2: {onethird ^ 2}, Equals 1/3 * 1/3: {(onethird ^ 2) == (onethird * onethird)}");
//*/