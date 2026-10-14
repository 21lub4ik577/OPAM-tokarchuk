
using System;
using System.Collections.Generic;

class Program
{
    static bool AreBracketsBalanced(string text)
    {
        Stack<char> stack = new Stack<char>();
        bool insideQuotes = false;

        for (int i = 0; i < text.Length; i++)
        {
            char symbol = text[i];

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

    static void Main()
    {
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
            bool result = AreBracketsBalanced(test);

            Console.WriteLine(
                $"Рядок: \"{test}\" -> " +
                (result ? "Збалансований" : "Незбалансований"));
        }

        string extraTest = new string('(', 16) + new string(')', 15);

        Console.WriteLine(
            $"Тест K: {AreBracketsBalanced(extraTest)}");
    }
}
