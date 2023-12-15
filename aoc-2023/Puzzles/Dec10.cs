using System.Drawing;
using System.Reflection.Emit;
using aoc_2023.Entities;
using aoc_2023.Services;

namespace aoc_2023.Puzzles;





public class Tile
{
    private string[] AllowedValues = new string[] { "|", "-", "L", "J", "7", "F", ".", "S" };

    private string[] AllowedRegionValues = new string[] { "O", "P", "I", "-" };

    public Pos Position = new Pos();
    public string Value { get; private set; } = "";
    public bool Initialized { get; set; } = false;
    public string ForwardDirection { get; set; } = "";
    public string ReverseDirection { get; set; } = "";
    public int ForwardDistance { get; set; } = 0;
    public int ReverseDistance { get; set; } = 0;
    public string Region { get; private set; } = "-";

    public Tile()
    {
        Value = ".";
        Position.X = 0;
        Position.Y = 0;
        Initialized = false;
    }

    public Tile(string value, int x, int y)
    {
        SetValue(value);
        Position.X = x;
        Position.Y = y;
        Initialized = true;
    }

    public bool Equals(Tile other)
    {
        // not tru equals but good enough
        return
            this.Position.X == other.Position.X &&
            this.Position.Y == other.Position.Y &&
            this.Value == other.Value;
    }

    private void SetValue(string value)
    {
        if (!AllowedValues.Any(o => o == value) || string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"Not a valid tile value: {value}");

        Value = value;
    }

    public void SetRegion(string value)
    {
        if (!AllowedRegionValues.Any(o => o == value) || string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Not a valid tile");

        Region = value;
    }

    public string[] GetPossibleMoves()
    {
        return Value switch
        {
            "|" => new string[] { "U", "D" },
            "-" => new string[] { "L", "R" },
            "L" => new string[] { "U", "R" },
            "J" => new string[] { "U", "L" },
            "7" => new string[] { "D", "L" },
            "F" => new string[] { "D", "R" },
            "S" => new string[] { "U", "D", "L", "R" },
            "." => throw new ArgumentException("Not a valid card value"),
            _ => throw new ArgumentException("Not a valid card value"),
        };
    }
}

public class Path
{
    public List<Tile> Tiles = new List<Tile>();
    public Tile Start = new Tile();
}


public class Dec10
{
    static Path Path = new Path();

    public static void SolvePt1(string? date, bool useTestData = false)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        // create a data file reader and read the file.
        var dfr = new DataFileReader(date: date, useTestData: useTestData, part: 1);
        dfr.ReadFile();

        string[,] Map = CreateMap(lines: dfr.Lines);

        PrintMap(map: Map, posToHighlight: Path.Start.Position);

        CreatePath(map: Map);

        int total = ComputeFurthestPos();

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;

        var outputString = useTestData ? "Part 2 Test [using test data]" : "Part 2 Test [using puzzle data]";
        Console.WriteLine($"{outputString}: {total} [elapsed time in ms: {elapsedMs}]");
    }

    public static void SolvePt2(string? date, bool useTestData = false)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        // create a data file reader and read the file.
        var dfr = new DataFileReader(date: date, useTestData: useTestData, part: 2);
        dfr.ReadFile();

        List<Tile> TileMap = CreateTileMap(lines: dfr.Lines);

        string[,] Map = CreateMap(lines: dfr.Lines);
        // PrintMap(map: Map, posToHighlight: Path.Start.Position);

        CreatePath(map: Map);

        int total = MapPathContents(map: Map, tileMap: TileMap);

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;

        var outputString = useTestData ? "Part 2 Test [using test data]" : "Part 2 Test [using puzzle data]";
        Console.WriteLine($"{outputString}: {total} [elapsed time in ms: {elapsedMs}]");
    }

    #region Helpers

    private static string[,] CreateMap(List<string> lines)
    {
        var numRows = lines.Count;
        var numCols = lines[0].ToCharArray().Count();

        string[,] map = new string[numRows, numCols];

        for (int i = 0; i < numRows; i++)
        {
            string[] col = lines[i].ToCharArray().Select(c => c.ToString()).ToArray();

            for (int j = 0; j < col.Length; j++)
            {
                map[i, j] = col[j]; 
                if (map[i, j] == "S")
                {
                    var start = new Tile(value: map[i, j], x: Convert.ToInt32(i), y: Convert.ToInt32(j));
                    Path.Start = start;
                    Path.Tiles.Add(start);
                }
            }
        }
        return map;
    }

    private static List<Tile> CreateTileMap(List<string> lines)
    {
        List<Tile> tileMap = new List<Tile>();

        var numRows = lines.Count;
        var numCols = lines[0].ToCharArray().Count();

        for (int i = 0; i < numRows; i++)
        {
            string[] col = lines[i].ToCharArray().Select(c => c.ToString()).ToArray();

            for (int j = 0; j < col.Length; j++)
            {
                tileMap.Add(new Tile(value: col[j], x: Convert.ToInt32(i), y: Convert.ToInt32(j)));
            }
        }
        return tileMap;
    }

    private static void CreatePath(string[,] map)
    {
        // start at the start
        var currTile = Path.Start;

        do
        {
            // want to check the 'next' tile is not the 'previous' tile to make sure you are not
            // going back the way you came
            var prev = currTile.Value == "S" ? currTile : Path.Tiles[Path.Tiles.Count - 2];
            // for the distance, need the last position + 1
            var last = currTile.Value == "S" ? currTile : Path.Tiles.Last();

            // is each direction a possible move
            var canMoveUp = CanMoveUp(map, currTile, prev);
            var canMoveRight = CanMoveRight(map, currTile, prev);
            var canMoveDown = CanMoveDown(map, currTile, prev);
            var canMoveLeft = CanMoveLeft(map, currTile, prev);

            // Every pipe in the main loop connects to its two neighbors
            // (including S, which will have exactly two pipes connecting to it,
            // and which is assumed to connect back to those two pipes).

            // if we're at the start, try to move in a direction
            if (canMoveUp)
            {
                var uPos = GetUpPos(currTile);
                var mapVal = GetMapValue(map, uPos, currTile);

                Tile step = new Tile(value: mapVal, x: Convert.ToInt32(uPos.X), y: Convert.ToInt32(uPos.Y));
                step.ForwardDirection = "U";
                step.ForwardDistance = last.ForwardDistance + 1;
                Path.Tiles.Add(step);
                //forward.Add(step);
                currTile = step;
            }
            else if (canMoveRight)
            {
                var rPos = GetRightPos(currTile);
                var mapVal = GetMapValue(map, rPos, currTile);

                Tile step = new Tile(value: mapVal, x: Convert.ToInt32(rPos.X), y: Convert.ToInt32(rPos.Y));
                step.ForwardDirection = "R";
                step.ForwardDistance = last.ForwardDistance + 1;
                Path.Tiles.Add(step);
                //forward.Add(step);
                currTile = step;
            }
            else if (canMoveDown)
            {
                var dPos = GetDownPos(currTile);
                var mapVal = GetMapValue(map, dPos, currTile);

                Tile step = new Tile(value: mapVal, x: Convert.ToInt32(dPos.X), y: Convert.ToInt32(dPos.Y));
                step.ForwardDirection = "D";
                step.ForwardDistance = last.ForwardDistance + 1;
                Path.Tiles.Add(step);
                //forward.Add(step);
                currTile = step;
            }
            else if (canMoveLeft)
            {
                var lPos = GetLeftPos(currTile);
                var mapVal = GetMapValue(map, lPos, currTile);

                Tile step = new Tile(value: mapVal, x: Convert.ToInt32(lPos.X), y: Convert.ToInt32(lPos.Y));
                step.ForwardDirection = "L";
                step.ForwardDistance = last.ForwardDistance + 1;
                Path.Tiles.Add(step);
                //forward.Add(step);
                currTile = step;
            }

            // now move again....put the above code in a loop
            // PrintMap(map: map, posToHighlight: currTile.Position);

        } while (currTile.Value != "S");
    }

    private static int ComputeFurthestPos()
    {
        //forward.OrderBy(t => t.ForwardDistance);

        //var index = forward.Count / 2;

        //var prev = forward[index - 1];
        //var item = forward[index];
        //var next = forward[index + 1];

        //// reverse.OrderBy(t => t.ReverseDistance).GroupBy(i => new { i.Value, i.ReverseDistance });

        //int max = 0;

        //for (int i = 0; i < forward.Count; i++)
        //{
        //    for (int j = 0; j < reverse.Count; j++)
        //    {
        //        if (forward[i].Value == reverse[j].Value && forward[i].Value != "S" && forward[i].ForwardDistance == reverse[j].ReverseDistance)
        //        {
        //            if (forward[i].ForwardDistance > max) max = forward[i].ForwardDistance;
        //        }
        //    }
        //}
        int max = Path.Tiles.Count / 2;
        return max;
    }

    private static int MapPathContents(string[,] map, List<Tile> tileMap)
    {
        var numRows = map.GetLength(0);
        var numCols = map.GetLength(1);

        int total = 0;

        Console.ForegroundColor = ConsoleColor.DarkGray;

        for (int i = 0; i < numRows; i++)
        {
            // crossings for row
            List<Tile> crossings = new List<Tile>();
            for (int j = 0; j < numCols; j++)
            {
                var tmpTile = new Tile(value: map[i, j], x: Convert.ToInt32(i), y: Convert.ToInt32(j));
                var currTileIsOnPath = TileIsOnPath(tmpTile);
                string[] NorthFacingValues = new string[] { "|", "L", "J", };
                var isVerticalCrossing = NorthFacingValues.Any(t => t == map[i, j]);

                // Console.WriteLine($"[{i},{j}: {map[i,j]}]");

                if (currTileIsOnPath && isVerticalCrossing)
                {
                    crossings.Add(tmpTile);
                }
            }
            // put them in order
            crossings.OrderBy(t => t.Position.X);

            // reset crossings on every row
            for (int j = 0; j < numCols; j++)
            {
                // Console.Write($" [(x:{i}, y:{j}) {map[i, j]}]");
                
                var currTileIsOnPath = TileIsOnPath(new Tile(value: map[i, j], x: Convert.ToInt32(i), y: Convert.ToInt32(j)));

                // if on path, set outside
                if (currTileIsOnPath)
                {
                    SetTileRegion(tileMap: tileMap, pos: new Pos { X = i, Y = j }, value: map[i, j], loc: "O");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"[{map[i, j]}]");
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    continue;
                }

                if (TileIsInside(crossings, pos: new Pos { X = i, Y = j }, numRows, numCols))
                {
                    SetTileRegion(tileMap: tileMap, pos: new Pos { X = i, Y = j }, value: map[i, j], loc: "I");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write($"[I]");
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    continue;
                }
                else
                {
                    SetTileRegion(tileMap: tileMap, pos: new Pos { X = i, Y = j }, value: map[i, j], loc: "O");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"[O]");
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    continue;
                }                
            }
            Console.WriteLine("");
            Console.WriteLine("");
        }

        for (int i = 0; i < tileMap.Count; i++)
        {
            if (tileMap[i].Region == "I") total++;
        }
        return total;
    }

    #region check moves
    private static bool CanMoveUp(string[,] map, Tile currTile, Tile prev)
    {
        if (currTile.Position.X == 0) return false;

        // the positions for up, down, left and right
        var uPos = GetUpPos(currTile);
        // the tiles for up, down, left and right
        var upTileValue = GetMapValue(map, uPos, currTile);
        // possible values for each move
        string[] uVals = new string[] { "|", "7", "F", "S" };
        if (uVals.Contains(upTileValue) &&
            currTile.GetPossibleMoves().Contains("U") &&
            !NextIsPrev(next: uPos, prev: prev.Position)) return true;
        return false;
    }

    private static bool CanMoveRight(string[,] map, Tile currTile, Tile prev)
    {
        var numCols = map.GetLength(1);

        if (currTile.Position.Y == numCols - 1) return false;

        var rPos = GetRightPos(currTile);
        var rightTileValue = GetMapValue(map, rPos, currTile);
        string[] rVals = new string[] { "-", "7", "J", "S" };
        if (rVals.Contains(rightTileValue) &&
            currTile.GetPossibleMoves().Contains("R") &&
            !NextIsPrev(next: rPos, prev: prev.Position)) return true;
        return false;
    }

    private static bool CanMoveDown(string[,] map, Tile currTile, Tile prev)
    {
        var numRows = map.GetLength(0);

        if (currTile.Position.X == numRows - 1) return false;

        var dPos = GetDownPos(currTile);
        var downTileValue = GetMapValue(map, dPos, currTile);
        string[] dVals = new string[] { "|", "L", "J", "S" };
        if (dVals.Contains(downTileValue) &&
            currTile.GetPossibleMoves().Contains("D") &&
            !NextIsPrev(next: dPos, prev: prev.Position)) return true;
        return false;
    }

    private static bool CanMoveLeft(string[,] map, Tile currTile, Tile prev)
    {
        if (currTile.Position.Y == 0) return false;
        var lPos = GetLeftPos(currTile);
        var leftTileValue = GetMapValue(map, lPos, currTile);
        string[] lVals = new string[] { "-", "F", "L", "S" };
        if (lVals.Contains(leftTileValue) &&
            currTile.GetPossibleMoves().Contains("L") &&
            !NextIsPrev(next: lPos, prev: prev.Position)) return true;
        return false;
    }
    #endregion

    #region Get Positions
    private static Pos GetUpPos(Tile currTile)
    {
        return new Pos { X = currTile.Position.X - 1, Y = currTile.Position.Y };
    }

    private static Pos GetRightPos(Tile currTile)
    {
        return new Pos { X = currTile.Position.X, Y = currTile.Position.Y + 1 };
    }

    private static Pos GetDownPos(Tile currTile)
    {
        return new Pos { X = currTile.Position.X + 1, Y = currTile.Position.Y };
    }

    private static Pos GetLeftPos(Tile currTile)
    {
        return new Pos { X = currTile.Position.X, Y = currTile.Position.Y - 1 };
    }

    private static string GetMapValue(string[,] map, Pos pos, Tile currTile)
    {
        try
        {
            return map[pos.X, pos.Y];
        }
        catch (Exception e)
        {
            throw new IndexOutOfRangeException($"{currTile}, message: {e}");
        }
        
    }
    #endregion

    private static bool NextIsPrev(Pos next, Pos prev)
    {
        if (next.X == prev.X && next.Y == prev.Y) return true;
        return false;
    }

    private static bool TileIsOnPath(Tile tile)
    {
        return Path.Tiles.Any(t => t.Position.X == tile.Position.X && t.Position.Y == tile.Position.Y && t.Value == tile.Value);
    }

    private static bool TileIsInside(List<Tile> crossings, Pos pos, int numRows, int numCols)
    {
        if (crossings.Count == 0) return false;

        for (int i = 1; i < crossings.Count; i++)
        {
            if (pos.Y > crossings[i-1].Position.Y && (i == crossings.Count || pos.Y < crossings[i].Position.Y) && pos.Y != numCols-1)
            {
                // it should be between two crossings here. Check the left crossing
                if (i % 2 == 1) return true;
            }
        }
        return false;
    }

    private static void SetTileRegion(List<Tile> tileMap, Pos pos, string value, string loc)
    {
        var tile = tileMap.First(t => t.Position.X == pos.X && t.Position.Y == pos.Y && t.Value == value);
        tile.SetRegion(loc);
    }

    private static void PrintMap(string[,] map, Pos posToHighlight)
    {
        var numRows = map.GetLength(0);
        var numCols = map.GetLength(1);

        var target = map[posToHighlight.X, posToHighlight.Y];

        Console.ForegroundColor = ConsoleColor.DarkGray;

        for (int i = 0; i < numRows; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                Console.Write($" [(x:{i}, y:{j}) ");

                if (posToHighlight != null && i == posToHighlight.X && j == posToHighlight.Y)
                {
                    HighlightConsole($"{target}");
                }
                else Console.Write($"{map[i, j]}");
                
                Console.Write($"] ");
            }
            Console.WriteLine("");
            Console.WriteLine("");
        }
    }

    private static void HighlightConsole(string content)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(content);
        Console.ForegroundColor = ConsoleColor.DarkGray;
    }

    #endregion

}

