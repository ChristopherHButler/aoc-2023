using System;
using System.Text;
using aoc_2023.Services;

namespace aoc_2023.Puzzles;

public class Pattern
{
    private string[] AllowedValues = new string[] { "V", "H", "-" };

    public List<string> Rows = new List<string>();
    public List<string> Cols = new List<string>();

    public string AxisOfReflection = "-";

    public void SetAxisOfReflection(string value)
    {
        if (!AllowedValues.Any(o => o == value) || string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"Not a valid tile value: {value}");

        AxisOfReflection = value;
    }
}

public class Puzzle
{
    public List<Pattern> Patterns = new List<Pattern>();

    public Puzzle(List<string> lines)
    {
        InitializePatterns(lines);
    }

    private void InitializePatterns(List<string> lines)
    {
        Pattern tmp = new Pattern();

        // map the rows
        for (int i = 0; i < lines.Count; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
            {
                Patterns.Add(tmp);
                tmp = new Pattern();
                continue;
            }
            tmp.Rows.Add(lines[i]);
        }

        Patterns.Add(tmp);

        // map the columns
        for (int i = 0; i < Patterns.Count(); i++)
        {
            // string[] col = lines[i].ToCharArray().Select(c => c.ToString()).ToArray();
            var numCols = Patterns[i].Rows[0].ToCharArray().Count();
            for (int j = 0; j < numCols; j++)
            {
                var col = "";
                // loop each column
                for (int k = 0; k < Patterns[i].Rows.Count(); k++)
                {
                    col += Patterns[i].Rows[k].ToCharArray().Select(c => c.ToString()).ToArray()[j];
                }
                Patterns[i].Cols.Add(col);
            }
        }
        
    }

    #region Part 1
    public int FindAllAxisOfReflection()
    {
        int summary = 0;
        foreach (var pattern in Patterns)
        {
            var (type, value) = FindPatternAxisOfReflection(pattern);
            summary += ComputePatternSummary(type, value);
        }
        return summary;
    }

    private int ComputePatternSummary(string type, int value)
    {
        if (type == "v") return value;
        else if (type == "h") return 100 * value;
        throw new ArgumentOutOfRangeException("this should never happen...");
    }

    private (string type, int value) FindPatternAxisOfReflection(Pattern pattern)
    {
        var hRef = DetectReflection(pattern.Rows);

        if (hRef != -1) return ("h", hRef);

        var vRef = DetectReflection(pattern.Cols);

        if (vRef != -1) return ("v", vRef);

        return ("-", -1);

    }

    private int DetectReflection(List<string> items)
    {
        for (int i = 0; i < items.Count()-1; i++)
        {
            for (int j = i + 1; j < items.Count(); j++)
            {
                if (items[i].Equals(items[j]))
                {
                    if (VerifyReflection(index: i, items)) return i+1;
                }
            }
        }
        return -1;
    }

    private bool VerifyReflection(int index, List<string> items)
    {
        int i = index;
        int j = index + 1;

        while (true)
        {
            if (i < 0 || j == items.Count) return true;
            if (!items[i--].Equals(items[j++])) return false;
        }
    }
    #endregion

    #region Part 2
    public int FindAllSmudgedAxisOfReflection()
    {
        int summary = 0;
        foreach (var pattern in Patterns)
        {
            var (origType, origValue) = FindPatternAxisOfReflection(pattern);
            var (type, value) = FindSmudgedPatternAxisOfReflection(pattern, origType, origValue);
            summary += ComputePatternSummary(type, value);
        }
        return summary;
    }

    private (string type, int value) FindSmudgedPatternAxisOfReflection(Pattern pattern, string origType, int origValue)
    {
        var hRef = DetectSmudgedReflection(pattern.Rows, "h", origType, origValue);

        if (hRef != -1) return ("h", hRef);

        var vRef = DetectSmudgedReflection(pattern.Cols, "v", origType, origValue);

        if (vRef != -1) return ("v", vRef);

        return ("-", -1);
    }

    private int DetectSmudgedReflection(List<string> items, string type, string origType, int origValue)
    {
        for (int i = 0; i < items.Count() - 1; i++)
        {
            for (int j = i + 1; j < items.Count(); j++)
            {
                if (items[i].Equals(items[j]) || (SmudgeEquals(items[i], items[j])))
                {
                    if (type == origType && (i + 1) == origValue) continue;
                    if (VerifySmudgedReflection(index: i, items, type, origType, origValue)) return i + 1;
                }
            }
        }
        return -1;
    }

    private bool SmudgeEquals(string str1, string str2)
    {
        // fi they are equal, exit early
        if (str1.Equals(str2)) return false;

        int counter = 0;
        for (int i = 0; i < str1.Length; i++)
        {
            if (str1[i] != str2[i]) counter++;
            if (counter > 1) return false;
        }
        if (counter == 1) return true;
        return false;
    }

    private bool VerifySmudgedReflection(int index, List<string> items, string type, string origType, int origValue)
    {
        if (type == origType && index == origValue) return false;

        int i = index;
        int j = index + 1;
        int counter = 0;

        while (true)
        {
            if (i < 0 || j >= items.Count) return true;
            
            if (!(items[i].Equals(items[j]) || SmudgeEquals(items[i], items[j]))) return false;
            if (SmudgeEquals(items[i], items[j])) counter++;
            if (counter > 1) return false;
            i--;
            j++;
        }
    }
    #endregion

}





public class Dec13
{
    public static void SolvePt1(string? date, bool useTestData = false)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        // create a data file reader and read the file.
        var dfr = new DataFileReader(date: date, useTestData: useTestData, part: 1);
        dfr.ReadFile();

        Puzzle puzzle = new Puzzle(lines: dfr.Lines);
        int total = puzzle.FindAllAxisOfReflection();


        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;

        var outputString = useTestData ? "Part 1 [using test data]" : "Part 1 [using puzzle data]";
        Console.WriteLine($"{outputString}: {total} [elapsed time in ms: {elapsedMs}]");
    }

    public static void SolvePt2(string? date, bool useTestData = false)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        // create a data file reader and read the file.
        var dfr = new DataFileReader(date: date, useTestData: useTestData, part: 2);
        dfr.ReadFile();

        Puzzle puzzle = new Puzzle(lines: dfr.Lines);
        int total = puzzle.FindAllSmudgedAxisOfReflection();


        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;

        var outputString = useTestData ? "Part 2 [using test data]" : "Part 2 [using puzzle data]";
        Console.WriteLine($"{outputString}: {total} [elapsed time in ms: {elapsedMs}]");
    }
}

