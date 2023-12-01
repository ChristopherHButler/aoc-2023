﻿using aoc_2023.Puzzles;

namespace aoc_2023;

class Program
{
    static void Main(string[] args)
    {
        var date = DateTime.Now.ToString("dd-MM-yyyy"); // "01-12-2023";
        var useTestData = false;

        Console.WriteLine($"AOC: {date}");
        Console.WriteLine("---------------");
        Console.WriteLine("");

        Dec01.SolvePt1(date, useTestData);
        Dec01.SolvePt2(date, useTestData);

        Console.ReadLine();
    }
}

