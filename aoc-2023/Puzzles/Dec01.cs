using aoc_2023.Services;

namespace aoc_2023.Puzzles;

public static class Dec01
{
    public static void SolvePt1(string? date, bool useTestData = false)
    {
        // create a data file reader and read the file.
        var dfr = new DataFileReader(date: date, useTestData: useTestData, part: 1);
        dfr.ReadFile();

        // Console.WriteLine($"Lines in File: {dfr.Lines.Count}");

        int totalOfCalibrationValues = 0;

        for (int i = 0; i < dfr.Lines.Count; i++)
        {
            // Console.WriteLine($"dfr.lines[{i}]: {dfr.Lines[i]}");
            totalOfCalibrationValues += parseCalibrationValueFromLine(dfr.Lines[i]);
            // Console.WriteLine($"Line: {i}");
        }
        var outputString = useTestData ? "Part 1 Total [using test data]" : "Part 1 Total [using puzzle data]";
        Console.WriteLine($"{outputString}: {totalOfCalibrationValues}");
    }

    public static void SolvePt2(string? date, bool useTestData = false)
    {
        // create a data file reader and read the file.
        var dfr = new DataFileReader(date: date, useTestData: useTestData, part: 2);
        dfr.ReadFile();

        // Console.WriteLine($"Lines in File: {dfr.Lines.Count}");

        int totalOfCalibrationValues = 0;

        for (int i = 0; i < dfr.Lines.Count; i++)
        {
            // Console.WriteLine($"dfr.lines[{i}]: {dfr.Lines[i]}");
            totalOfCalibrationValues += parseCalibrationValueFromLine2(dfr.Lines[i]);
            // Console.WriteLine($"Line: {i}");
        }

        var outputString = useTestData ? "Part 2 Total [using test data]" : "Part 2 Total [using puzzle data]";
        Console.WriteLine($"{outputString}: {totalOfCalibrationValues}");
    }

    private static int parseCalibrationValueFromLine(string line)
    {
        char[] _line = line.ToCharArray();

        bool firstFound = false;

        char first = 'z';
        char last = 'z';

        for (int i = 0; i < _line.Length; i++)
        {
            if (isCharDigit(line[i]))
            {
                if (!firstFound)
                {
                    firstFound = true;
                    first = _line[i];
                }
                else
                {
                    last = _line[i];
                }
            }
        }

        if (last == 'z') last = first;

        string tmp = charsToString(first, last);

        var value = Int32.Parse(tmp);

        // Console.WriteLine($"Value: {value}");

        if (value.GetType() != typeof(int))
        {
            Console.WriteLine($"Line failed");
            throw new ArgumentOutOfRangeException();
        }

        //if (numCount > 2)
        //{
        //    Console.WriteLine($"numCount: {numCount}");
        //    Console.WriteLine($"line: {line}");
        //    Console.WriteLine($"value: {value}");
        //}

        return value;
    }

    private static int parseCalibrationValueFromLine2(string line)
    {
        char[] _line = line.ToCharArray();

        bool firstFound = false;

        char first = 'z';
        char last = 'z';

        for (int i = 0; i < _line.Length; i++)
        {
            int num = getNumberAtIndex(i, line);
            if (num != 0)
            {
                if (!firstFound)
                {
                    firstFound = true;
                    first = Convert.ToChar(num.ToString());
                }
                else
                {
                    last = Convert.ToChar(num.ToString());
                }
            }

            if (isCharDigit(line[i]))
            {
                if (!firstFound)
                {
                    firstFound = true;
                    first = _line[i];
                }
                else
                {
                    last = _line[i];
                }
            }
            
        }

        if (last == 'z') last = first;

        string tmp = charsToString(first, last);

        var value = Int32.Parse(tmp);

        //if (value.GetType() != typeof(int))
        //{
        //    Console.WriteLine($"Line failed");
        //    throw new ArgumentOutOfRangeException();
        //}

        //if (numCount > 2)
        //{
        //    Console.WriteLine($"numCount: {numCount}");
        //    Console.WriteLine($"line: {line}");
        //    Console.WriteLine($"value: {value}");
        //}

        return value;


    }

    private static bool isCharDigit(char c)
    {
        return Char.IsDigit(c);
    }

    private static string charsToString(char a, char b)
    {
        char[] chars = { a, b };
        return new string(chars);
    }

    private static int getNumberAtIndex(int index, string line)
    {
        List<string> numbers = new List<string>
        {
            "one", "two", "three", "four", "five", "six", "seven", "eight", "nine"
        };

        int num = 0;

        for (int i = 0; i < numbers.Count; i++)
        {
            if (line.IndexOf(numbers[i], index) == index)
            {
                num = i + 1;
                return num;
            }
        }
        return 0;
    }
}
