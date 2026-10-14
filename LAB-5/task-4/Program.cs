
using System;
using System.Collections.Generic;

class IntStack
{
    private int[] items;
    private int count;

    public int Count => count;
    public int Capacity => items.Length;

    public IntStack(int capacity)
    {
        if (capacity < 1)
        {
            throw new ArgumentException("Місткість має бути більшою за нуль.");
        }

        items = new int[capacity];
        count = 0;
    }

    public void Push(int x)
    {
        if (count == items.Length)
        {
            int[] newItems = new int[items.Length * 2];
            Array.Copy(items, newItems, items.Length);
            items = newItems;
        }

        items[count] = x;
        count++;
    }

    public int Pop()
    {
        if (count == 0)
        {
            throw new InvalidOperationException("Стек порожній");
        }

        count--;
        return items[count];
    }

    public int Peek()
    {
        if (count == 0)
        {
            throw new InvalidOperationException("Стек порожній");
        }

        return items[count - 1];
    }
}

class Program
{
    static bool AreBracketsBalanced(string text)
    {
        Stack<char> stack = new Stack<char>();
        bool insideQuotes = false;

        foreach (char symbol in text)
        {
            if (symbol == '"')
            {
                insideQuotes = !insideQuotes;
                continue;
            }

            if (insideQuotes)
            {
                continue;
            }

            if (symbol == '(' || symbol == '[' || symbol == '{')
            {
                stack.Push(symbol);
            }
            else if (symbol == ')' || symbol == ']' || symbol == '}')
            {
                if (stack.Count == 0)
                {
                    return false;
                }

                char opening = stack.Pop();

                if ((symbol == ')' && opening != '(') ||
                    (symbol == ']' && opening != '[') ||
                    (symbol == '}' && opening != '{'))
                {
                    return false;
                }
            }
        }

        return stack.Count == 0;
    }

    static bool AreBracketsBalancedCustom(string text)
    {
        IntStack stack = new IntStack(16 % 4 + 2);
        bool insideQuotes = false;

        foreach (char symbol in text)
        {
            if (symbol == '"')
            {
                insideQuotes = !insideQuotes;
                continue;
            }

            if (insideQuotes)
            {
                continue;
            }

            if (symbol == '(' || symbol == '[' || symbol == '{')
            {
                stack.Push(symbol);
            }
            else if (symbol == ')' || symbol == ']' || symbol == '}')
            {
                if (stack.Count == 0)
                {
                    return false;
                }

                char opening = (char)stack.Pop();

                if ((symbol == ')' && opening != '(') ||
                    (symbol == ']' && opening != '[') ||
                    (symbol == '}' && opening != '{'))
                {
                    return false;
                }
            }
        }

        return stack.Count == 0;
    }

    static void TestCustomStack()
    {
        Console.WriteLine("ПЕРЕВІРКА ВЛАСНОГО СТЕКА");
        Console.WriteLine();

        IntStack stack = new IntStack(16 % 4 + 2);
        int pushCount = 16 % 4 + 3;

        for (int i = 1; i <= pushCount; i++)
        {
            stack.Push(i * 10);
            Console.WriteLine(
                $"Push {i}: Count = {stack.Count}, Capacity = {stack.Capacity}");
        }

        Console.WriteLine($"Верхівка стека: {stack.Peek()}");
        Console.WriteLine($"Вилучений елемент: {stack.Pop()}");
        Console.WriteLine();
    }

    static void TestBrackets()
    {
        Console.WriteLine("ПЕРЕВІРКА ДУЖОК");
        Console.WriteLine();

        string[] tests =
        {
            "(a[b]{c})",
            "([)]",
            ")(",
            "((",
            "",
            "\"(\"",
            "abc",
            "\"text ) [ }\""
        };

        foreach (string test in tests)
        {
            bool standardResult = AreBracketsBalanced(test);
            bool customResult = AreBracketsBalancedCustom(test);

            Console.WriteLine(
                $"\"{test}\": стандартний стек = {standardResult}, " +
                $"власний стек = {customResult}");
        }
    }

    static void TestEmptyStack()
    {
        Console.WriteLine();
        Console.WriteLine("ПЕРЕВІРКА ПОРОЖНЬОГО СТЕКА");

        try
        {
            IntStack stack = new IntStack(2);
            stack.Pop();
        }
        catch (InvalidOperationException exception)
        {
            Console.WriteLine($"Помилка: {exception.Message}");
        }
    }

    static void Main()
    {
        TestCustomStack();
        TestBrackets();
        TestEmptyStack();
    }
}
