
using System;
using System.Collections.Generic;

class Request
{
    public int Number { get; set; }
    public int Weight { get; set; }
    public int ArrivalTime { get; set; }

    public Request(int number, int weight, int arrivalTime)
    {
        Number = number;
        Weight = weight;
        ArrivalTime = arrivalTime;
    }
}

class Program
{
    static void SimulateService()
    {
        int N = 26;
        int K = 16;
        int requestCount = 8 + K % 5;

        Random random = new Random(N);
        Queue<Request> queue = new Queue<Request>();

        int arrivalTime = 0;

        for (int i = 1; i <= requestCount; i++)
        {
            if (i > 1)
            {
                arrivalTime += random.Next(1, 5);
            }

            int weight = random.Next(1, 10);

            Request request = new Request(i, weight, arrivalTime);
            queue.Enqueue(request);
        }

        int currentTime = 0;
        int totalIdleTime = 0;
        int maxQueueLength = 0;

        Console.WriteLine("МОДЕЛЬ ОБСЛУГОВУВАННЯ ЗАЯВОК");
        Console.WriteLine();

        while (queue.Count > 0)
        {
            if (queue.Count > maxQueueLength)
            {
                maxQueueLength = queue.Count;
            }

            if (queue.Count == 0)
            {
                break;
            }

            Request request = queue.Dequeue();

            if (currentTime < request.ArrivalTime)
            {
                totalIdleTime += request.ArrivalTime - currentTime;
                currentTime = request.ArrivalTime;
            }

            int startTime = currentTime;
            int endTime = startTime + request.Weight;

            Console.WriteLine(
                $"Заявка {request.Number}: " +
                $"вага = {request.Weight}, " +
                $"надходження = {request.ArrivalTime} хв, " +
                $"початок = {startTime} хв, " +
                $"завершення = {endTime} хв");

            currentTime = endTime;
        }

        Console.WriteLine();
        Console.WriteLine($"Кількість заявок: {requestCount}");
        Console.WriteLine($"Загальний час простою: {totalIdleTime} хв");
        Console.WriteLine($"Максимальна довжина черги: {maxQueueLength}");
    }

    static void Main()
    {
        SimulateService();
    }
}
