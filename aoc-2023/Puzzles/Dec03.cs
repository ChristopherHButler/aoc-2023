using aoc_2023.Services;

namespace aoc_2023.Puzzles;

public class Cell
{
    public bool IncludeNumber { get; set; } = false;
    public int Value { get; set; }
    // start position of number
    public int[] StartPosition = new int[2];
    public int[] EndPosition = new int[2];
    public List<int[]> Positions = new List<int[]>();

    public void SetStartPosition(int i, int j)
    {
        StartPosition[0] = i;
        StartPosition[1] = j;
    }

    public void SetEndPosition(int i, int j)
    {
        EndPosition[0] = i;
        EndPosition[1] = j;
    }
}

public class SpecialChar
{
    public char Character { get; set; }
    public int[] Position = new int[2];

    public void SetPosition(int i, int j)
    {
        Position[0] = i;
        Position[1] = j;
    }
}

public class Gear : SpecialChar
{
    public List<Cell> Parts = new List<Cell>();
    public int GearRatio { get; set; } = 0;

    public void AddPart(Cell cell)
    {
        bool exists = false;

        for (int i = 0; i < Parts.Count; i++)
        {
            var currPart = Parts[i];
            if (currPart.Value == cell.Value && currPart.StartPosition[0] == cell.StartPosition[0] && currPart.StartPosition[1] == cell.StartPosition[1])
            {
                exists = true;
            }
        }

        if (!exists)
        {
            Parts.Add(cell);
        }

    }

}



public static class Dec03
{
    public static char[,] Matrix;

    public static void SolvePt1(string? date, bool useTestData = false)
    {
        List<Cell> Numbers = new List<Cell>();
        List<SpecialChar> SpecialCharacters = new List<SpecialChar>();

        int total = 0;

        Matrix = CreateMatrix(date, useTestData);

        FindAllNumbersAndSpecialCharacters(Numbers, SpecialCharacters);

        for (int i = 0; i < Numbers.Count; i++)
        {
            ScanPerimeterOfNumber(Numbers[i], SpecialCharacters);
        }

        for (int i = 0; i < Numbers.Count; i++)
        {
            if (Numbers[i].IncludeNumber)
            {
                total += Numbers[i].Value;
            }
        }

        var outputString = useTestData ? "Part 1 Total [using test data]" : "Part 1 Total [using puzzle data]";
        Console.WriteLine($"{outputString}: {total}");

        // PrintMatrix();

    }

    public static void SolvePt2(string? date, bool useTestData = false)
    {
        List<Cell> Numbers = new List<Cell>();
        List<Gear> Gears = new List<Gear>();

        int total = 0;

        Matrix = CreateMatrix(date, useTestData);

        FindAllNumbersAndGears(Numbers, Gears);

        ScanPerimeterOfGear(Numbers, Gears);
        

        for (int i = 0; i < Gears.Count; i++)
        {
            if (Gears[i].Parts.Count == 2)
            {
                total += (Gears[i].Parts[0].Value * Gears[i].Parts[1].Value);
            }
        }

        var outputString = useTestData ? "Part 2 Total [using test data]" : "Part 2 Total [using puzzle data]";
        Console.WriteLine($"{outputString}: {total}");

        // PrintMatrix();

    }

    private static char[,] CreateMatrix(string date, bool useTestData)
    {
        // create a data file reader and read the file.
        var dfr = new DataFileReader(date: date, useTestData: useTestData, part: 1);
        dfr.ReadFile();

        var lines = dfr.Lines;

        var numRows = lines.Count;
        var numCols = dfr.Lines[0].ToCharArray().Count();

        char[ , ] matrix = new char[numRows, numCols];

        for (int i = 0; i < dfr.Lines.Count; i++)
        {
            char[] col = dfr.Lines[i].ToCharArray();

            for (int j = 0; j < col.Length; j++)
            {
                matrix[i, j] = col[j];
            }
        }

        return matrix;
    }

    private static void FindAllNumbersAndSpecialCharacters(List<Cell> numbers, List<SpecialChar> scs)
    {
        char[] SpecialChars = "%*#$@&/-+=".ToCharArray();

        for (int i = 0; i < GetMatrixNumRows(); i++)
        {
            for (int j = 0; j < GetMatrixNumCols(); j++)
            {
                if (char.IsDigit(Matrix[i, j]))
                {
                    Cell cell = new Cell();
                    cell.SetStartPosition(i, j);
                    cell.Value = FindFullNumber(cell);

                    numbers.Add(cell);

                    // skip to end of number
                    j = cell.EndPosition[1];
                }
                else if (SpecialChars.Contains(Matrix[i, j]))
                {
                    SpecialChar sc = new SpecialChar();
                    sc.Character = Matrix[i, j];
                    sc.SetPosition(i, j);

                    scs.Add(sc);
                }
            }
        }
    }

    private static void FindAllNumbersAndGears(List<Cell> numbers, List<Gear> gears)
    {
        char[] SpecialChars = "*".ToCharArray();

        for (int i = 0; i < GetMatrixNumRows(); i++)
        {
            for (int j = 0; j < GetMatrixNumCols(); j++)
            {
                if (char.IsDigit(Matrix[i, j]))
                {
                    Cell cell = new Cell();
                    cell.SetStartPosition(i, j);
                    cell.Value = FindFullNumber(cell);

                    numbers.Add(cell);

                    // skip to end of number
                    j = cell.EndPosition[1];
                }
                else if (SpecialChars.Contains(Matrix[i, j]))
                {
                    Gear sc = new Gear();
                    sc.Character = Matrix[i, j];
                    sc.SetPosition(i, j);

                    gears.Add(sc);
                }
            }
        }
    }

    private static int FindFullNumber(Cell cell)
    {
        List<char> chars = new List<char>();
        int i = cell.StartPosition[0];
        int j = cell.StartPosition[1];

        do
        {
            cell.Positions.Add(new[] { i, j });
            chars.Add(Matrix[i, j]);
            if (j <= GetMatrixNumCols()) j++;

        } while (j < GetMatrixNumCols() && char.IsDigit(Matrix[i, j]));

        cell.SetEndPosition(i, --j);
        var value = Convert.ToInt32(new string(chars.ToArray()));
        return value;
    }

    private static void ScanPerimeterOfNumber(Cell cell, List<SpecialChar> scs)
    {
        // loop over every cell in the number
        for (int i = 0; i < cell.Positions.Count; i++)
        {
            // check every cell around it to see if scs contains those coordinates
            for (int j = 0; j < scs.Count; j++)
            {
                var cellDigit = cell.Positions[i];
                int[] upperLeft = { cellDigit[0] - 1, cellDigit[1] - 1 };
                int[] top = { cellDigit[0] - 1, cellDigit[1] };
                int[] upperRight = { cellDigit[0] - 1, cellDigit[1] + 1 };
                int[] left = { cellDigit[0], cellDigit[1] - 1 };
                int[] right = { cellDigit[0], cellDigit[1] + 1 };
                int[] lowerLeft = { cellDigit[0] + 1, cellDigit[1] - 1 };
                int[] bottom = { cellDigit[0] + 1, cellDigit[1] };
                int[] lowerRight = { cellDigit[0] + 1, cellDigit[1] + 1 };

                if ((scs[j].Position[0] == upperLeft[0] && scs[j].Position[1] == upperLeft[1]) ||
                    (scs[j].Position[0] == top[0] && scs[j].Position[1] == top[1]) ||
                    (scs[j].Position[0] == upperRight[0] && scs[j].Position[1] == upperRight[1]) ||
                    (scs[j].Position[0] == left[0] && scs[j].Position[1] == left[1]) ||
                    (scs[j].Position[0] == right[0] && scs[j].Position[1] == right[1]) ||
                    (scs[j].Position[0] == lowerLeft[0] && scs[j].Position[1] == lowerLeft[1]) ||
                    (scs[j].Position[0] == bottom[0] && scs[j].Position[1] == bottom[1]) ||
                    (scs[j].Position[0] == lowerRight[0] && scs[j].Position[1] == lowerRight[1]))
                {
                    cell.IncludeNumber = true;
                    return;
                }
            }
        }

        cell.IncludeNumber = false;
    }

    private static void ScanPerimeterOfGear(List<Cell> numbers, List<Gear> gears)
    {
        // loop over every gear
        for (int i = 0; i < gears.Count; i++)
        {
            var gear = gears[i];

            int[] upperLeft = { gear.Position[0] - 1, gear.Position[1] - 1 };
            int[] top = { gear.Position[0] - 1, gear.Position[1] };
            int[] upperRight = { gear.Position[0] - 1, gear.Position[1] + 1 };
            int[] left = { gear.Position[0], gear.Position[1] - 1 };
            int[] right = { gear.Position[0], gear.Position[1] + 1 };
            int[] lowerLeft = { gear.Position[0] + 1, gear.Position[1] - 1 };
            int[] bottom = { gear.Position[0] + 1, gear.Position[1] };
            int[] lowerRight = { gear.Position[0] + 1, gear.Position[1] + 1 };

            List<int[]> adjs = new List<int[]>
            {
                upperLeft,
                top,
                upperRight,
                left,
                right,
                lowerLeft,
                bottom,
                lowerRight
            };

            // check every cell around the gear to see if two (and only two) numbers overlap
            for (int m = 0; m < adjs.Count; m++)
            {
                var currAdj = adjs[m];

                // every number
                for (int n = 0; n < numbers.Count; n++)
                {
                    var currNum = numbers[n];

                    // every position of every number
                    for (int p = 0; p < numbers[n].Positions.Count; p++)
                    {
                        var currPos = currNum.Positions[p];

                        if (currAdj[0] == currPos[0] && currAdj[1] == currPos[1])
                        {
                            // gears[i].Parts.Add(currNum);
                            gears[i].AddPart(currNum);
                            break;
                        }

                    }
                }
            }      

        }
        
    }

    private static char GetValueAtPosition(int[] pos)
    {
        return Matrix[pos[0], pos[1]];
    }

    private static int GetMatrixNumRows()
    {
        return Matrix.GetLength(0);
    }

    private static int GetMatrixNumCols()
    {
        return Matrix.GetLength(1);
    }

    private static void PrintMatrix()
    {
        for (int i = 0; i < GetMatrixNumRows(); i++)
        {
            for (int j = 0; j < GetMatrixNumCols(); j++)
            {
                Console.Write($" [{Matrix[i, j]}] ");
            }
            Console.WriteLine("");
        }
    }

}

