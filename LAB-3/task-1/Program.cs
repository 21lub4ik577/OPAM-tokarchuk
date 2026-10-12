
using System;
using System.IO;

class DirectoryAnalyzer
{
    public static string? GetPath()
    {
        Console.WriteLine("Enter folder path:");
        string? path = Console.ReadLine();

        return path;
    }

    public static FileInfo? FindLargestFile(string path)
    {
        FileInfo? largestFile = null;

        try
        {
            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                FileInfo currentFile = new FileInfo(file);

                if (largestFile == null ||
                    currentFile.Length > largestFile.Length)
                {
                    largestFile = currentFile;
                }
            }

            string[] directories = Directory.GetDirectories(path);

            foreach (string directory in directories)
            {
                FileInfo? subfolderLargest = FindLargestFile(directory);

                if (subfolderLargest != null &&
                    (largestFile == null || subfolderLargest.Length > largestFile.Length))
                {
                    largestFile = subfolderLargest;
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine($"Access denied: {path}");
        }
        catch (DirectoryNotFoundException)
        {
            Console.WriteLine($"Directory not found: {path}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"I/O error in {path}: {ex.Message}");
        }

        return largestFile;
    }
}

class Program
{
    static void Main()
    {
        string? path = DirectoryAnalyzer.GetPath();

        if (string.IsNullOrWhiteSpace(path) ||
            !Directory.Exists(path))
        {
            Console.WriteLine("Folder does not exist.");
            return;
        }

        FileInfo? largestFile = DirectoryAnalyzer.FindLargestFile(path);

        if (largestFile == null)
        {
            Console.WriteLine("No files found.");
        }
        else
        {
            Console.WriteLine("\nLargest file:");
            Console.WriteLine($"Name: {largestFile.Name}");
            Console.WriteLine($"Size: {largestFile.Length} bytes");
            Console.WriteLine($"Full path: {largestFile.FullName}");
        }
    }
}
