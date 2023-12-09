using System;
using aoc_2023.Services;

namespace aoc_2023.Puzzles;

public static class Dec08
{



    public static void SolvePt1(string? date, bool useTestData = false)
    {


        // create a data file reader and read the file.
        var dfr = new DataFileReader(date: date, useTestData: useTestData, part: 1);
        dfr.ReadFile();


        int total = 1;

        var outputString = useTestData ? "Part 1 Test [using test data]" : "Part 1 Test [using puzzle data]";
        Console.WriteLine($"{outputString}: {total}");
    }

    public static void SolvePt2(string? date, bool useTestData = false)
    {


        // create a data file reader and read the file.
        var dfr = new DataFileReader(date: date, useTestData: useTestData, part: 2);
        dfr.ReadFile();


        int total = 1;

        var outputString = useTestData ? "Part 1 Test [using test data]" : "Part 1 Test [using puzzle data]";
        Console.WriteLine($"{outputString}: {total}");
    }


    #region Helpers




    #endregion

}