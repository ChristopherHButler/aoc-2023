using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.Linq;
using aoc_2023.Services;

namespace aoc_2023.Puzzles;

public class Dir
{
    string[] AllowedValues = new string[] { "L", "R" };

    public string Value { get; private set; }

    public Dir(string value)
    {
        SetValue(value);
    }

    private void SetValue(string value)
    {
        if (!AllowedValues.Any(o => o == value))
            throw new ArgumentException("Not a valid card");

        Value = value;
    }
}


public class Node
{
    public string Name { get; set; } = "";
    public string Left { get; set; } = "";
    public string Right { get; set; } = "";

    public Node(string name, string left, string right)
    {
        if (name.Length != 3 || left.Length != 3 || right.Length != 3)
            throw new ArgumentOutOfRangeException("All Nodes must be 3 chars long");

        Name = name;
        Left = left;
        Right = right;
    }
}


public static class Dec08
{

    public static void SolvePt1(string? date, bool useTestData = false)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        List<Dir> Directions = new List<Dir>();
        List<Node> Nodes = new List<Node>();

        // create a data file reader and read the file.
        var dfr = new DataFileReader(date: date, useTestData: useTestData, part: 1);
        dfr.ReadFile();

        Directions = ParseDirections(lines: dfr.Lines);
        Nodes = ParseNodes(lines: dfr.Lines);

        int steps = Navigate(dirs: Directions, nodes: Nodes);

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;

        var outputString = useTestData ? "Part 1 Test [using test data]" : "Part 1 Test [using puzzle data]";
        Console.WriteLine($"{outputString}: {steps} [elapsed time in ms: {elapsedMs}]");
    }

    public static void SolvePt2(string? date, bool useTestData = false)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        List<Dir> Directions = new List<Dir>();
        List<Node> Nodes = new List<Node>();
        List<Node> Starters = new List<Node>();

        // create a data file reader and read the file.
        var dfr = new DataFileReader(date: date, useTestData: useTestData, part: 2);
        dfr.ReadFile();

        Directions = ParseDirections(lines: dfr.Lines);
        Nodes = ParseNodes(lines: dfr.Lines);
        Starters = ParseStartingNodes(nodes: Nodes);

        long steps = Navigate2(dirs: Directions, nodes: Nodes, starters: Starters);

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;

        var outputString = useTestData ? "Part 1 Test [using test data]" : "Part 1 Test [using puzzle data]";
        Console.WriteLine($"{outputString}: {steps} [elapsed time in ms: {elapsedMs}]");
    }


    #region Helpers

    private static List<Dir> ParseDirections(List<string> lines)
    {
        List<Dir> dirs = new List<Dir>();
        for (int i = 0; i < lines.Count; i++)
        {
            var chars = lines[i].ToCharArray().Select(c => c.ToString()).ToList();
            for (int j = 0; j < chars.Count; j++)
            {
                var dir = new Dir(value: chars[j]);
                dirs.Add(dir);
            }
            
            if (string.IsNullOrEmpty(lines[i])) break;
        }
        return dirs;
    }

    private static List<Node> ParseNodes(List<string> lines)
    {
        List<Node> nodes = new List<Node>();
        for (int i = 2; i < lines.Count; i++)
        {
            // SGR = (JLL, VRV)
            var parts = lines[i].Split('=');
            var dirs = parts[1].Replace("(", "").Replace(")", "").Split(",");
            var node = new Node(name: parts[0].Trim(), left: dirs[0].Trim(), right: dirs[1].Trim());
            nodes.Add(node);
        }
        return nodes;
    }

    private static List<Node> ParseStartingNodes(List<Node> nodes)
    {
        List<Node> starters = new List<Node>();
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].Name[2] == 'A') starters.Add(nodes[i]);
        }
        return starters;
    }

    private static int Navigate(List<Dir> dirs, List<Node> nodes)
    {
        int index = 0;
        int count = 0;
        // look up first direction
        var currDir = dirs[0];
        // var currStep;
        var currNode = nodes.Find(n => n.Name == "AAA");
        // check which node to visit next
        var next = "";

        while (currNode.Name != "ZZZ")
        {
            // reset the directions if you go over
            if (index == dirs.Count) index = 0;

            // look up first direction
            currDir = dirs[index];

            Console.Write($"d: [{currDir.Value}] | CN: [{currNode.Name}] ->");

            if (currDir == null)
                throw new ArgumentOutOfRangeException("Could not find that direction. currDir is null");

            // check which node to visit next
            next = currDir.Value == "L" ? currNode.Left : currNode.Right;

            if (next.Length != 3)
                throw new ArgumentOutOfRangeException($"{next} is not a valid node.");

            // look up that node
            currNode = nodes.Find(n => n.Name == next);

            if (currNode == null)
                throw new ArgumentOutOfRangeException("Could not find that node. currNode is null");

            Console.Write($"[{currNode.Name}]");
            Console.WriteLine("");

            // record the step
            count++;

            // and repeat
            index++;
        }
        return count;
    }

    private static long Navigate2(List<Dir> dirs, List<Node> nodes, List<Node> starters)
    {
        List<long> counts = new List<long>();
        
        // for each starting node
        for (int i = 0; i < starters.Count; i++)
        {
            var currNode = starters[i];

            var pathCount = DoWork(currNode, dirs, nodes);
            counts.Add(pathCount);
        }
        return FindLCM(counts);
    }

    private static long DoWork(Node currNode, List<Dir> dirs, List<Node> nodes)
    {
        int index = 0; // direction index. Need to loop when All advance
        long count = 0;
        var currDir = dirs[0];
        var next = "";

        while (currNode.Name[2] != 'Z')
        {
            // reset the directions if you go over
            if (index == dirs.Count) index = 0;

            // look up first direction
            currDir = dirs[index];

            Console.Write($"s: [{currNode.Name}] | d: [{currDir.Value}] | CN: [{currNode.Name}] ->");

            if (currDir == null)
                throw new ArgumentOutOfRangeException("Could not find that direction. currDir is null");

            // check which node to visit next
            next = currDir.Value == "L" ? currNode.Left : currNode.Right;

            if (next.Length != 3)
                throw new ArgumentOutOfRangeException($"{next} is not a valid node.");

            // look up that node
            currNode = nodes.Find(n => n.Name == next);

            if (currNode == null)
                throw new ArgumentOutOfRangeException("Could not find that node. currNode is null");

            Console.Write($"[{currNode.Name}]");
            Console.WriteLine("");

            // record the step
            count++;

            // and repeat
            index++;
        }
        return count;
    }

    private static long FindLCM(IEnumerable<long> numbers)
    {
        return numbers.Aggregate((long)1, (current, number) => current / FindGCD(current, number) * number);
    }

    private static long FindGCD(long a, long b)
    {
        while (b != 0)
        {
            a %= b;
            (a, b) = (b, a);
        }
        return a;
    }

    #endregion

}