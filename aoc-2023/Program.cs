using aoc_2023.Puzzles;

namespace aoc_2023;

class Program
{
    static void Main(string[] args)
    {
        var date = "07-12-2023"; // DateTime.Now.ToString("dd-MM-yyyy"); // "01-12-2023";
        var useTestData = true;

        Console.WriteLine($"AOC: {date}");
        Console.WriteLine("---------------");
        Console.WriteLine("");

        // Dec01.SolvePt1(date, useTestData);
        // Dec01.SolvePt2(date, useTestData);

        // Dec02.SolvePt1(date, useTestData);
        // Dec02.SolvePt2(date, useTestData);

        // Dec03.SolvePt1(date, useTestData);
        // Dec03.SolvePt2(date, useTestData);

        // Dec04.SolvePt1(date, useTestData);
        // Dec04.SolvePt2(date, useTestData);

        // Dec05.SolvePt1(date, useTestData);
        // Dec05.SolvePt2(date, useTestData);

        // Dec06.SolvePt1(date, useTestData);
        // Dec06.SolvePt2(date, useTestData);

        // Dec07.SolvePt1(date, useTestData);
        Dec07.SolvePt2(date, useTestData);

        // 254464303 too low
        // 255491737 too low
        // 256519796 too low


        Console.ReadLine();
    }
}

