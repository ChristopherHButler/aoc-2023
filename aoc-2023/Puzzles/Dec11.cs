using System;
using aoc_2023.Entities;
using aoc_2023.Services;

namespace aoc_2023.Puzzles;


public class Galaxy
{
    public Pos Position = new Pos();

    public Galaxy(Pos p)
    {
        Position.X = p.X;
        Position.Y = p.Y;
    }
}

public class Size
{
    public int Rows = 0;
    public int Cols = 0;
}

public class Universe
{
    // size of the universe
    public Size Size = new Size();

    public List<Galaxy> Galaxies = new List<Galaxy>();

    public long Sum { get; set; } = 0;

    public Universe(List<string> lines)
    {
        CreateUniverse(lines);
    }

    private void CreateUniverse(List<string> lines)
    {
        var numRows = lines.Count;
        var numCols = lines[0].ToCharArray().Count();

        // set the size of the universe
        Size = new Size { Rows = numRows, Cols = numCols };

        for (int i = 0; i < numRows; i++)
        {
            string[] col = lines[i].ToCharArray().Select(c => c.ToString()).ToArray();

            for (int j = 0; j < col.Length; j++)
            {
                if (col[j] == "#")
                {
                    var g = new Galaxy(new Pos { X = i, Y = j });
                    Galaxies.Add(g);
                }
            }
        }
    }

    public void ExpandUniverse()
    {
        // get empty rows and cols
        var emptyRows = GetEmptyRows();
        var emptyCols = GetEmptyColumns();

        // update the size of the galaxy
        Size.Rows += emptyRows.Count();
        Size.Cols += emptyCols.Count();

        // update the positions of galaxies
        for (int i = 0; i < emptyRows.Count(); i++)
        {
            AddEmptyRowsAfter(emptyRows[i]+i);
        }
        for (int i = 0; i < emptyCols.Count(); i++)
        {
            AddEmptyColumnsAfter(emptyCols[i]+i);
        }
        Sum = FindShortestPathsForAllGalaxies();
    }

    public void ExpandOlderUniverse()
    {
        // get empty rows and cols
        var emptyRows = GetEmptyRows();
        var emptyCols = GetEmptyColumns();

        // update the size of the galaxy
        Size.Rows += emptyRows.Count();
        Size.Cols += emptyCols.Count();

        var expansionFactor = 999999;

        // update the positions of galaxies
        for (int i = 0; i < emptyRows.Count(); i++)
        {
            AddEmptyRowsAfter(index: (emptyRows[i]+(i * expansionFactor)), rowCount: expansionFactor);
        }
        for (int i = 0; i < emptyCols.Count(); i++)
        {
            AddEmptyColumnsAfter(index: (emptyCols[i]+(i * expansionFactor)), colCount: expansionFactor);
        }
        Sum = FindShortestPathsForAllGalaxies();
    }

    public long FindShortestPathsForAllGalaxies()
    {
        Dictionary<string, long> pairs = new Dictionary<string, long>();
        for (int i = 0; i < Galaxies.Count()-1; i++)
        {
            for (int j = i+1; j < Galaxies.Count(); j++)
            {                
                var sum = FindShortestPathBetweenTwoGalaxies(Galaxies[i], Galaxies[j]);
                var key = $"{Galaxies[i].Position.X}.{Galaxies[i].Position.Y}:{Galaxies[j].Position.X}.{Galaxies[j].Position.Y}"; // $"{i+1}{j+1}";
                pairs.Add(key, sum);
            }
        }
        
        return pairs.Sum(x => x.Value);
    }

    public int FindShortestPathBetweenTwoGalaxies(Galaxy a, Galaxy b)
    {
        return Math.Abs(a.Position.X - b.Position.X) + Math.Abs(a.Position.Y - b.Position.Y);
    }

    #region Helpers

    private List<int> GetEmptyRows()
    {
        List<int> emptyRows = new List<int>();

        // so to find an empty row you need to scan the list of galaxies and
        for (int i = 0; i < Size.Rows; i++)
        {
            var items = Galaxies.Where(x => x.Position.X == i);
            if (items.Count() == 0)
            {
                emptyRows.Add(i);
                continue;
            }
        }
        return emptyRows;
    }

    private List<int> GetEmptyColumns()
    {
        List<int> emptyCols = new List<int>();
        for (int i = 0; i < Size.Cols; i++)
        {
            var items = Galaxies.Where(x => x.Position.Y == i);
            if (items.Count() == 0)
            {
                emptyCols.Add(i);
                continue;
            }
        }
        return emptyCols;
    }

    private void AddEmptyRowsAfter(int index, int rowCount = 1)
    {
        for (int i = 0; i < Galaxies.Count(); i++)
        {
            if (Galaxies[i].Position.X > index)
            {
                Galaxies[i].Position.X += rowCount;
            }
        }
    }

    private void AddEmptyColumnsAfter(int index, int colCount = 1)
    {
        for (int i = 0; i < Galaxies.Count(); i++)
        {
            if (Galaxies[i].Position.Y > index)
            {
                Galaxies[i].Position.Y += colCount;
            }
        }
    }

    public void PrintUniverse(bool displayNumber = false)
    {
        Console.WriteLine($"Universe: [{Size.Rows} x {Size.Cols}]");
        Console.WriteLine("");
        int count = 1;
        for (int i = 0; i < Size.Rows; i++)
        {
            for (int j = 0; j < Size.Cols; j++)
            {
                if (Galaxies.Any(g => g.Position.X == i && g.Position.Y == j))
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    if (displayNumber) Console.Write($"{count++}");
                    else Console.Write($"#");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write($".");
                }
            }
            Console.WriteLine("");
        }
        Console.WriteLine("");
        Console.WriteLine("");
    }

    #endregion

}

public class Dec11
{
    public static void SolvePt1(string? date, bool useTestData = false)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        // create a data file reader and read the file.
        var dfr = new DataFileReader(date: date, useTestData: useTestData, part: 1);
        dfr.ReadFile();

        // and then god created the universe...
        Universe universe = new Universe(dfr.Lines);
        // universe.PrintUniverse(displayNumber: true);

        // expand the universe
        universe.ExpandUniverse();
        // universe.PrintUniverse(displayNumber: true);  

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;

        var outputString = useTestData ? "Part 1 Test [using test data]" : "Part 1 Test [using puzzle data]";
        Console.WriteLine($"{outputString}: {universe.Sum} [elapsed time in ms: {elapsedMs}]");
    }

    public static void SolvePt2(string? date, bool useTestData = false)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        // create a data file reader and read the file.
        var dfr = new DataFileReader(date: date, useTestData: useTestData, part: 1);
        dfr.ReadFile();

        // and then god created the universe...
        Universe universe = new Universe(dfr.Lines);
        // universe.PrintUniverse(displayNumber: true);

        // expand the universe
        universe.ExpandOlderUniverse();
        // universe.PrintUniverse(displayNumber: true);  

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;

        var outputString = useTestData ? "Part 2 Test [using test data]" : "Part 2 Test [using puzzle data]";
        Console.WriteLine($"{outputString}: {universe.Sum} [elapsed time in ms: {elapsedMs}]");
    }

}

