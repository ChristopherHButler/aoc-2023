using System;
using aoc_2023.Services;

namespace aoc_2023.Puzzles;

public class Range
{
    public long sourceRangeStart { get; set; }
    public long sourceRangeEnd
    {
        get
        {
            return sourceRangeStart + rangeLength;
        }
    }
    public long destRangeStart { get; set; }
    public long destRangeEnd
    {
        get
        {
            return destRangeStart + rangeLength;
        }
    }
    public long rangeLength;
}

public class Map
{
    public string Title { get; set; } = "";
    public List<Range> ranges = new List<Range>();

    public Map(string title)
    {
        Title = title;
    }

    public long GetDestFromSrc(long src)
    {
        for (int i = 0; i < ranges.Count; i++)
        {
            // if less than ANY range, map 1:1
            if (src < ranges[0].sourceRangeStart)
                return src;

            var startPosition = ranges[i].sourceRangeStart;
            var rangeLimit = ranges[i].sourceRangeStart + ranges[i].rangeLength;
            var startRangeEnd = ranges[i].sourceRangeEnd;

            if (rangeLimit != startRangeEnd)
                throw new Exception("Something doesn't add up");

            if (src >= startPosition && src < rangeLimit)
            {
                var destOffset = src - startPosition;
                var value = ranges[i].destRangeStart + destOffset;
                return value;
            }
        }

        return src;
    }
}

public static class Maps
{
    public static Map SeedToSoil = new Map("seed-to-soil");
    public static Map SoilToFertilizer = new Map("soil-to-fertilizer");
    public static Map FertilizerToWater = new Map("fertilizer-to-water");
    public static Map WaterToLight = new Map("water-to-light");
    public static Map LightToTemperature = new Map("light-to-temperature");
    public static Map TemperatureToHumidity = new Map("temperature-to-humidity");
    public static Map HumidityToLocation = new Map("humidity-to-location");
}

public class SeedLink
{
    public Dictionary<string, long> path = new Dictionary<string, long>();
}

public class SeedPair
{
    public long Start { get; set; }
    public long Range { get; set; }
}

public class Dec05
{
    
    public static void SolvePt1(string? date, bool useTestData = false)
    {
        List<long> Seeds = new List<long>();
        List<SeedLink> SeedLinks = new List<SeedLink>();

        // create a data file reader and read the file.
        var dfr = new DataFileReader(date: date, useTestData: useTestData, part: 1);
        dfr.ReadFile();

        GetSeeds(line: dfr.Lines[0], seeds: Seeds);

        CreateAllMaps(lines: dfr.Lines);

        MapSeedsToLocation(seeds: Seeds, paths: SeedLinks);

        var lowest = GetLowestLocationNumber(paths: SeedLinks);

        var outputString = useTestData ? "Part 1 Test [using test data]" : "Part 1 Test [using puzzle data]";
        Console.WriteLine($"{outputString}: {lowest}");

    }

    public static void SolvePt2(string? date, bool useTestData = false)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        List<SeedPair> SeedPairs = new List<SeedPair>();
        List<SeedLink> SeedLinks = new List<SeedLink>();

        // create a data file reader and read the file.
        var dfr = new DataFileReader(date: date, useTestData: useTestData, part: 1);
        dfr.ReadFile();

        GetPt2SeedPairs(line: dfr.Lines[0], pairs: SeedPairs);

        CreateAllMaps(lines: dfr.Lines);

        var lowest = MapSeedPairsToLocation(pairs: SeedPairs, paths: SeedLinks);

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;

        var outputString = useTestData ? "Part 2 Location [using test data]" : "Part 2 Location [using puzzle data]";
        Console.WriteLine($"{outputString}: {lowest} [elapsed time in ms: {elapsedMs}]");

    }


    private static void GetSeeds(string line, List<long> seeds)
    {
        var tmp = line.Split(':');
        seeds.AddRange(
          Array.ConvertAll(tmp[1].Trim().Split(' '), s => Convert.ToInt64(s))
        );
    }

    private static void GetPt2SeedPairs(string line, List<SeedPair> pairs)
    {
        var tmp = line.Split(':');
        // get the pairs (start and range)
        var values = tmp[1].Trim().Split(' ');

        for (var i = 0; i < values.Length; i += 2)
        {
            var pair = new SeedPair
            {
                Start = Convert.ToInt64(values[i]),
                Range = Convert.ToInt64(values[i+1]),
            };
            pairs.Add(pair);
        }

    }

    private static void CreateMap(Map map, List<string> lines)
    {
        int startLine = -1;
        List<Range> ranges = new List<Range>();

        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].Contains(map.Title)) startLine = i + 1;
            if (startLine == -1) continue;

            if (string.IsNullOrEmpty(lines[i])) break;
            
            if (i >= startLine)
            {
                var parts = lines[i].Split(' ');
                
                Range range = new Range
                {
                    sourceRangeStart = Convert.ToInt64(parts[1]),
                    destRangeStart = Convert.ToInt64(parts[0]),
                    rangeLength = Convert.ToInt64(parts[2])
                };
                ranges.Add(range);
            }
        }
        // order the ranges
        map.ranges.AddRange(ranges.OrderBy(x => x.sourceRangeStart));
    }

    private static void CreateAllMaps(List<string> lines)
    {
        CreateMap(map: Maps.SeedToSoil, lines);
        CreateMap(map: Maps.SoilToFertilizer, lines);
        CreateMap(map: Maps.FertilizerToWater, lines);
        CreateMap(map: Maps.WaterToLight, lines);
        CreateMap(map: Maps.LightToTemperature, lines);
        CreateMap(map: Maps.TemperatureToHumidity, lines);
        CreateMap(map: Maps.HumidityToLocation, lines);
    }

    private static void MapSeedsToLocation(List<long> seeds, List<SeedLink> paths)
    {
        // for each seed, map to a location
        for(int i = 0; i < seeds.Count; i++)
        {
            // link to each type until location;
            var soil = Maps.SeedToSoil.GetDestFromSrc(seeds[i]);
            var fert = Maps.SoilToFertilizer.GetDestFromSrc(soil);
            var water = Maps.FertilizerToWater.GetDestFromSrc(fert);
            var light = Maps.WaterToLight.GetDestFromSrc(water);
            var temp = Maps.LightToTemperature.GetDestFromSrc(light);
            var hum = Maps.TemperatureToHumidity.GetDestFromSrc(temp);
            var loc = Maps.HumidityToLocation.GetDestFromSrc(hum);

            SeedLink tmp = new SeedLink();
            tmp.path.Add("seed", seeds[i]);
            tmp.path.Add("soil", soil);
            tmp.path.Add("fert", fert);
            tmp.path.Add("water", water);
            tmp.path.Add("light", light);
            tmp.path.Add("temp", temp);
            tmp.path.Add("hum", hum);
            tmp.path.Add("loc", loc);

            paths.Add(tmp);
        }
    }

    private static long MapSeedPairsToLocation(List<SeedPair> pairs, List<SeedLink> paths)
    {
        long lowest = 0;
        for (int i = 0; i < pairs.Count; i++)
        {
            var startPosition = pairs[i].Start;
            var rangeLimit = pairs[i].Start + pairs[i].Range;

            // this is a "list" of seeds to check
            for (long j = startPosition; j < rangeLimit; j++)
            {
                // link to each type until location;
                var soil = Maps.SeedToSoil.GetDestFromSrc(j);
                var fert = Maps.SoilToFertilizer.GetDestFromSrc(soil);
                var water = Maps.FertilizerToWater.GetDestFromSrc(fert);
                var light = Maps.WaterToLight.GetDestFromSrc(water);
                var temp = Maps.LightToTemperature.GetDestFromSrc(light);
                var hum = Maps.TemperatureToHumidity.GetDestFromSrc(temp);
                var loc = Maps.HumidityToLocation.GetDestFromSrc(hum);

                if (lowest == 0) lowest = loc;
                if (loc < lowest && loc != -1) lowest = loc;
            }

        }
        return lowest;
    }

    private static long GetLowestLocationNumber(List<SeedLink> paths)
    {
        long lowest = 0;
        for (int i = 0; i < paths.Count; i++)
        {
            if (i == 0) lowest = paths[i].path["loc"];
            if (paths[i].path["loc"] < lowest && paths[i].path["loc"] != -1) lowest = paths[i].path["loc"];
        }
        return lowest;
    }
}

