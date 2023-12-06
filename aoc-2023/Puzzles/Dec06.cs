using System;
using aoc_2023.Services;

namespace aoc_2023.Puzzles;

public class Race
{
    public long Time { get; set; }
    public long RecordDistance { get; set; }
    public List<RaceOption> RaceOptions = new List<RaceOption>();

    public int WinningOptions
    {
        get
        {
            return RaceOptions.Count(o => o.IsWinningOption);
        }
    }
}

public class RaceOption
{
    public long HoldTime { get; set; }
    public long TimeToRace { get; set; }
    public long Speed { get; set; }
    public long DistanceTraveled { get; set; }
    public bool IsWinningOption { get; set; } = false;
}



public class Dec06
{
    public static void SolvePt1(string? date, bool useTestData = false)
    {
        List<Race> Races = new List<Race>();

        // create a data file reader and read the file.
        var dfr = new DataFileReader(date: date, useTestData: useTestData, part: 1);
        dfr.ReadFile();

        CreateRaces(lines: dfr.Lines, races: Races);

        ComputeRaceOptions(races: Races);

        int total = (int)GetWinningProduct(races: Races);

        var outputString = useTestData ? "Part 1 Test [using test data]" : "Part 1 Test [using puzzle data]";
        Console.WriteLine($"{outputString}: {total}");

    }

    public static void SolvePt2(string? date, bool useTestData = false)
    {
        List<Race> Races = new List<Race>();

        // create a data file reader and read the file.
        var dfr = new DataFileReader(date: date, useTestData: useTestData, part: 2);
        dfr.ReadFile();

        CreateRaces2(lines: dfr.Lines, races: Races);

        ComputeRaceOptions(races: Races);

        long total = GetWinningProduct(races: Races);

        var outputString = useTestData ? "Part 2 Test [using test data]" : "Part 2 Test [using puzzle data]";
        Console.WriteLine($"{outputString}: {total}");

    }

    public static void CreateRaces(List<string> lines, List<Race> races)
    {
        // Time:     7  15   30
        // Distance: 9  40  200
        var times = Array.ConvertAll(
            lines[0].Split(':')[1].Trim().Split(' ')
                .Where(x => !string.IsNullOrEmpty(x)).ToArray(),
            s => Convert.ToInt64(s)
        );
        var dists = Array.ConvertAll(
            lines[1].Split(':')[1].Trim().Split(' ')
                .Where(x => !string.IsNullOrEmpty(x)).ToArray(),
            s => Convert.ToInt64(s)
        );

        if (times.Length != dists.Length)
            throw new ArgumentOutOfRangeException("Times and Distances do not match");

        for (int i = 0; i < times.Length; i++)
        {
            races.Add(new Race { Time = times[i], RecordDistance = dists[i] });
        }
    }

    public static void CreateRaces2(List<string> lines, List<Race> races)
    {
        var time = Convert.ToInt64(lines[0].Split(':')[1].Replace(" ", "").Trim());
        var dist = Convert.ToInt64(lines[1].Split(':')[1].Replace(" ", "").Trim());

        races.Add(new Race { Time = time, RecordDistance = dist });
    }

    public static void ComputeRaceOptions(List<Race> races)
    {
        for (int i = 0; i < races.Count; i++)
        {
            for (long j = 0; j < races[i].Time; j++)
            {
                // create a race option for every ms
                races[i].RaceOptions.Add(new RaceOption
                {
                    HoldTime = j,
                    TimeToRace = races[i].Time - j,
                    Speed = j,
                    DistanceTraveled = (races[i].Time - j) * j,
                    IsWinningOption = ((races[i].Time - j) * j) > races[i].RecordDistance ? true : false,
                });
            }
        }

    }

    public static long GetWinningProduct(List<Race> races)
    {
        long total = 1;

        foreach (var race in races)
        {
            total *= race.WinningOptions;
        }
        return total;
    }

}

