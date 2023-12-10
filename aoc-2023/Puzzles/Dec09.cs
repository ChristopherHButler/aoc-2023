using System;
using aoc_2023.Services;

namespace aoc_2023.Puzzles;

public class Sequence
{
    public List<int> Numbers = new List<int>();
    public int Difference = 0;


    public int GetLastNumber()
    {
        return Numbers.Last();
    }
}

public class History
{
    public List<int> Values = new List<int>();
    public List<Sequence> Sequences = new List<Sequence>();
    public int NextValue { get; set; }
    public int PreviousValue { get; set; }
}

public class Dec09
{
    public static void SolvePt1(string? date, bool useTestData = false)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        List<History> Histories = new List<History>();
       
        // create a data file reader and read the file.
        var dfr = new DataFileReader(date: date, useTestData: useTestData, part: 1);
        dfr.ReadFile();

        Histories = ParseHistories(lines: dfr.Lines);

        CreateSequencesForAllHistories(histories: Histories);

        int total = PredictNextValueForAllHistories(histories: Histories);

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;

        var outputString = useTestData ? "Part 1 Test [using test data]" : "Part 1 Test [using puzzle data]";
        Console.WriteLine($"{outputString}: {total} [elapsed time in ms: {elapsedMs}]");
    }

    public static void SolvePt2(string? date, bool useTestData = false)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        List<History> Histories = new List<History>();

        // create a data file reader and read the file.
        var dfr = new DataFileReader(date: date, useTestData: useTestData, part: 2);
        dfr.ReadFile();

        Histories = ParseHistories(lines: dfr.Lines);

        CreateSequencesForAllHistories(histories: Histories);

        int total = PredictPreviousValueForAllHistories(histories: Histories);

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;

        var outputString = useTestData ? "Part 2 Test [using test data]" : "Part 2 Test [using puzzle data]";
        Console.WriteLine($"{outputString}: {total} [elapsed time in ms: {elapsedMs}]");
    }

    #region Helpers

    private static List<History> ParseHistories(List<string> lines)
    {
        List<History> histories = new List<History>();
        for (int i = 0; i < lines.Count; i++)
        {
            var values = lines[i].Split(' ');
            History tmp = new History();
            for (int j = 0; j < values.Length; j++)
            {
                tmp.Values.Add(Convert.ToInt32(values[j].Trim()));
            }
            histories.Add(tmp);
        }
        return histories;
    }

    private static void CreateSequencesForAllHistories(List<History> histories)
    {
        for (int i = 0; i < histories.Count; i++)
        {
            CreateSequencesForHistory(history: histories[i]);
        }
    }

    private static void CreateSequencesForHistory(History history)
    {
        // create inital sequence
        history.Sequences.Add(CreateSequence(values: history.Values));

        int index = 0;

        while (!history.Sequences[index].Numbers.All(n => n == 0))
        {
            // create another sequence
            history.Sequences.Add(CreateSequence(values: history.Sequences[index++].Numbers));
        }
        history.Sequences.Reverse();
    }

    private static Sequence CreateSequence(List<int> values)
    {
        Sequence seq = new Sequence();
        var diff = 0;
        for (int i = 1; i < values.Count; i++)
        {
            diff = FindDifference(values[i - 1], values[i]);
            seq.Numbers.Add(diff);
        }
        seq.Difference = diff;
        return seq;
    }

    private static int FindDifference(int n1, int n2)
    {
        return (n2 - n1);// Math.Abs(n2 - n1);
    }

    private static int PredictNextValueForAllHistories(List<History> histories)
    {
        int total = 0;
        for (int i = 0; i < histories.Count; i++)
        {
            total += PredictNextValueForHistory(history: histories[i]);
        }
        return total;
    }

    private static int PredictNextValueForHistory(History history)
    {
        for (int i = 0; i < history.Sequences.Count; i++)
        {
            var currSeq = history.Sequences[i];
            if (i == 0) currSeq.Numbers.Add(currSeq.Difference); // 0
            else currSeq.Numbers.Add((currSeq.Difference + history.Sequences[i-1].GetLastNumber()));
        }
        history.Sequences.Reverse();

        var diff = history.Sequences[0].Numbers.Last();

        history.NextValue = history.Values.Last() + diff;
        history.Values.Add(history.NextValue);
        return history.NextValue;
    }

    private static int PredictPreviousValueForAllHistories(List<History> histories)
    {
        int total = 0;
        for (int i = 0; i < histories.Count; i++)
        {
            total += PredictPreviousValueForHistory(history: histories[i]);
        }
        return total;
    }

    private static int PredictPreviousValueForHistory(History history)
    {
        for (int i = 0; i < history.Sequences.Count; i++)
        {
            var currSeq = history.Sequences[i];
            if (i == 0) currSeq.Numbers.Insert(0, currSeq.Difference); // 0 - still 0

            else
            {
                var firstInPrevSeq = history.Sequences[i - 1].Numbers[0];
                currSeq.Numbers.Insert(0, (currSeq.Numbers[0] - firstInPrevSeq));
            }
        }
        history.Sequences.Reverse();

        var diff = history.Sequences[0].Numbers[0];

        history.PreviousValue = history.Values[0] - diff;
        history.Values.Insert(0, history.PreviousValue);
        return history.PreviousValue;
    }

    #endregion
}

