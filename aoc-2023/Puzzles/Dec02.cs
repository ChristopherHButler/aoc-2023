using System;
using aoc_2023.Services;

namespace aoc_2023.Puzzles;

// Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green

public class Cube
{
    public string Color { get; set; }
    public int Num { get; set; }
}

public class Subset
{
    public bool Possible = false;
    public List<Cube> Cubes = new List<Cube>();
}

public class Game
{
    public int Id { get; set; }
    public bool Possible = false;

    public int RedMax = 1;
    public int GreenMax = 1;
    public int BlueMax = 1;
    public int PowerOfCubes
    {
        get
        {
            return RedMax * GreenMax * BlueMax;
        }
    }

    public List<Subset> Subsets = new List<Subset>();
}

public static class Dec02
{
    public static void SolvePt1(string? date, bool useTestData = false)
    {
        // 12 red cubes, 13 green cubes, and 14 blue cubes
        List<Cube> totals = new List<Cube>
        {
            new Cube { Color = "red", Num = 12 },
            new Cube { Color = "green", Num = 13 },
            new Cube { Color = "blue", Num = 14 },
        };

        int idsTotal = 0;

        // create a data file reader and read the file.
        var dfr = new DataFileReader(date: date, useTestData: useTestData, part: 1);
        dfr.ReadFile();

        List<Game> Games = new List<Game>();

        for (int i = 0; i < dfr.Lines.Count; i++)
        {
            var game = ParseGameFromLine(dfr.Lines[i], totals);

            Games.Add(game);
        }

        for (int i = 0; i < Games.Count; i++)
        {
            if (Games[i].Possible)
            {
                Console.WriteLine($"game[{Games[i].Id}] possible");
                idsTotal += Games[i].Id; 
            }
        }

        var outputString = useTestData ? "Part 1 Total [using test data]" : "Part 1 Total [using puzzle data]";
        Console.WriteLine($"{outputString}: {idsTotal}");

    }

    public static void SolvePt2(string? date, bool useTestData = false)
    {
        // 12 red cubes, 13 green cubes, and 14 blue cubes
        List<Cube> totals = new List<Cube>
        {
            new Cube { Color = "red", Num = 12 },
            new Cube { Color = "green", Num = 13 },
            new Cube { Color = "blue", Num = 14 },
        };

        int powerOfCubesSum = 0;

        // create a data file reader and read the file.
        var dfr = new DataFileReader(date: date, useTestData: useTestData, part: 1);
        dfr.ReadFile();

        List<Game> Games = new List<Game>();

        for (int i = 0; i < dfr.Lines.Count; i++)
        {
            var game = ParseGameFromLine(dfr.Lines[i], totals);

            Games.Add(game);
        }

        for (int i = 0; i < Games.Count; i++)
        {
            powerOfCubesSum += Games[i].PowerOfCubes;
        }

        var outputString = useTestData ? "Part 2 Total [using test data]" : "Part 2 Total [using puzzle data]";
        Console.WriteLine($"{outputString}: {powerOfCubesSum}");
    }

    private static Game ParseGameFromLine(string line, List<Cube> totals)
    {
        string[] tmp = line.Split(':');
        // int id = Convert.ToInt32(line[5].ToString());
        int id = Convert.ToInt32(tmp[0].Split(' ')[1]);

        List<Subset> subsets = new List<Subset>();

        string[] stringSubsets = tmp[1].Split(';');

        int redMax = 1;
        int greenMax = 1;
        int blueMax = 1;

        for (int i = 0; i < stringSubsets.Length; i++)
        {
            var subset = ParseSubset(stringSubsets[i], totals);
            subsets.Add(subset);
        }
        bool gamePossible = true;
        foreach (Subset subset in subsets)
        {
            if (!subset.Possible) gamePossible = false;
        }

        for (int i = 0; i < subsets.Count; i++)
        {
            var redCube = subsets[i].Cubes.Find(cube => cube.Color == "red");
            var greenCube = subsets[i].Cubes.Find(cube => cube.Color == "green");
            var blueCube = subsets[i].Cubes.Find(cube => cube.Color == "blue");
            if (redCube?.Num > redMax) redMax = redCube.Num;
            if (greenCube?.Num > greenMax) greenMax = greenCube.Num;
            if (blueCube?.Num > blueMax) blueMax = blueCube.Num;

        }

        Game game = new Game
        {
            Id = id,
            Possible = gamePossible,
            Subsets = subsets,
            RedMax = redMax,
            GreenMax = greenMax,
            BlueMax = blueMax,
        };

        return game;
    }

    private static Subset ParseSubset(string subset, List<Cube> totals)
    {
        bool possible = true;
        List<Cube> cubes = new List<Cube>();
        string[] stringCubes = subset.Split(',');

        for (int i = 0; i < stringCubes.Length; i++)
        {
            var parts = stringCubes[i].Trim().Split(' ');

            int num = Convert.ToInt32(parts[0].Trim());
            string color = parts[1].Trim();

            cubes.Add(new Cube { Num = num, Color = color });
        }

        for (int i = 0; i < cubes.Count; i++)
        {
            var current = cubes[i];
            Cube target = totals.Find(cube => cube.Color == current.Color);

            if (current.Num > target.Num)
            {
                possible = false;
            }
        }

        Subset sebset = new Subset()
        {
            Possible = possible,
            Cubes = cubes
        };

        return sebset;
    }

}

