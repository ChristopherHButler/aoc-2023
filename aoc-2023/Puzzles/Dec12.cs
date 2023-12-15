using System;
using aoc_2023.Services;

namespace aoc_2023.Puzzles;




public class Dec12
{
    public static void SolvePt1(string? date, bool useTestData = false)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        // create a data file reader and read the file.
        var dfr = new DataFileReader(date: date, useTestData: useTestData, part: 1);
        dfr.ReadFile();


        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;

        var outputString = useTestData ? "Part 1 [using test data]" : "Part 1 [using puzzle data]";
        Console.WriteLine($"{outputString}: {0} [elapsed time in ms: {elapsedMs}]");
    }
}

