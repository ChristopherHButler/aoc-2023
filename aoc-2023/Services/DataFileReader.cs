using System;
namespace aoc_2023.Services;


// This Data File Reader is meant to be used in a very specific way.
// if no parameter is suppied, it will create the file name it wants to use from the date.
// You can overwrite the filename by supplying it.
public class DataFileReader
{
    public string FullPath { get; set; }
    public List<string> Lines { get; set; } = new List<string>();

    /// <summary>
    /// This fucntion you are creating a data file each day with a specific name format: dd-MM-yyyy-data.txt
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="date"></param>
    /// <param name="debugMode"></param>
    /// <param name="runningTests"></param>
    public DataFileReader(string? filename = null, string? date = null, bool? useTestData = false, int? part = 0, bool debugMode = false)
    {
        if (!String.IsNullOrEmpty(filename))
        {
            FullPath = filename;
        }
        else
        {
            // This is tightly coupled to directory structure. I'm cool with that.
            string part1 = "part1-test";
            string part2 = "part2-test";

            var _now = String.IsNullOrEmpty(date) ? DateTime.Now.ToString("dd-MM-yyyy") : date;

            // This assumes you are creating a data file each day with a specific name format: dd-MM-yyyy-data.txt
            // If that is not the case, this will obviously crash and burn but I'm OK with that for this project.
            var filePath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"../../../Data"));

            string fullPath = "";

            if (useTestData == true)
            {
                if (part == 1) fullPath = $"{filePath}/{_now}-{part1}-data.txt";
                if (part == 2) fullPath = $"{filePath}/{_now}-{part2}-data.txt";
            }
            else
            {
                fullPath = $"{filePath}/{_now}-data.txt";
            }

            FullPath = fullPath;

            if (debugMode)
            {
                Console.WriteLine($"DataFileReader - ctor - fullPath: {FullPath}");
            }
        }
    }


    // could save this value during creation but this offers more fine grained logging control
    public void ReadFile(bool debugMode = false)
    {
        try
        {
            if (File.Exists(FullPath))
            {
                var strArr = File.ReadAllLines(FullPath);
                Lines = new List<string>(strArr);

                if (debugMode)
                {
                    Console.WriteLine($"DataFileReader - ReadFile - strArr.Length: {strArr.Length}");
                    Console.WriteLine($"DataFileReader - ReadFile - Lines.Count: {Lines.Count}");
                    Console.WriteLine($"DataFileReader - ReadFile - Lines[0]: {Lines[0]}");
                }

            }
            else
            {
                throw new FileNotFoundException();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error Reading File: {FullPath}");
            Console.WriteLine($"stack trace: {e}");
        }
    }


}


