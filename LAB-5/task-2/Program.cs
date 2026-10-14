using System;
using System.Collections.Generic;

class Program
{
    static bool CalculateRPN(string expression, out double result)
    {
        Stack<double> stack = new Stack<double>();
        result = 0;

        string[] tokens = expression.Split(
            ' ',
            StringSplitOptions.RemoveEmptyEntries);

        foreach (string token in tokens)
        {
            if (double.TryParse(token, out double number))
            {
                stack.Push(number);
            }
            else
            {
                if (token != "+" && token != "-" && token != "*" && token != "/" && token != "^")
                {
                    return false;
                }

                if (stack.Count < 2)
                {
                    return false;
                }

                double right = stack.Pop();
                double left = stack.Pop();
                double value;

                switch (token)
                {
                    case "+":
                        value = left + right;
                        break;

                    case "-":
                        value = left - right;
                        break;

                    case "*":
                        value = left * right;
                        break;

                    case "/":
                        if (right == 0)
                        {
                            return false;
                        }

                        value = left / right;
                        break;

                    case "^":
                        value = Math.Pow(left, right);
                        break;

                    default:
                        return false;
                }

                stack.Push(value);
            }
        }

        if (stack.Count != 1)
        {
            return false;
        }

        result = stack.Pop();
        return true;
    }

    static void Main()
    {
        string[] expressions =
        {
            "3 4 + 2 *",
            "5 3 -",
            "8 2 /",
            "2 3 4 * +",
            "5 0 /",
            "2 3 ^ 2 3 ^ +",
            "7 +",
            "2 3",
            "2 abc +",
            ""
        };

        foreach (string expression in expressions)
        {
            if (CalculateRPN(expression, out double result))
            {
                Console.WriteLine($"{expression} = {result}");
            }
            else
            {
                Console.WriteLine(
                    $"'{expression}' -> Помилка у виразі");
            }
        }
    }
}