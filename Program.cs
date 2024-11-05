using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

struct Result
{
    //                locid
    public Dictionary<string, Location> Locations;
}

struct Location
{
    //                zoneid
    public Dictionary<string, Zone> Zones;
}

// members is lowercase for parsing
struct Zone
{
    public string position;
    public string type;
}

struct IndexMap
{
    public int Start;
    public int End;
}

class Program
{
    static void Main()
    {
        // read input
        var input = File.ReadAllLines("./zonesdump");
        
        // parse input
        var result = GetLocations(input);

        // save to file
        var json = JsonConvert.SerializeObject(result, Formatting.Indented);
        File.WriteAllText("./questpoints.jsonc", json);
    }

    private static Result GetLocations(string[] lines)
    {
        var result = new Result()
        {
            Locations = new Dictionary<string, Location>()
        };

        // parse input
        var locationBarriers = GetIndexBarriers(lines);

        foreach (var locationKvp in locationBarriers)
        {
            // add location to result
            var location = new Location()
            {
                Zones = new Dictionary<string, Zone>()
            };

            result.Locations.Add(locationKvp.Key, location);

            // add zones to result
            for (var i = locationKvp.Value.Start; i < locationKvp.Value.End; i++)
            {
                var line = lines[i];

                // DEBUG
                Console.WriteLine(line);

                // get zone
                var zoneName = GetZoneName(line);

                var zone = new Zone();
                zone.position = GetPosition(line);
                zone.type = GetType(line);

                // add result
                if (location.Zones.ContainsKey(zoneName))
                {
                    // edit
                    location.Zones[zoneName] = zone;
                }
                else
                {
                    // add
                    location.Zones.Add(zoneName, zone);
                }
            }
        }

        return result;
    }

    private static Dictionary<string, IndexMap>GetIndexBarriers(string[] lines)
    {
        var result = new Dictionary<string, IndexMap>();
        var locationName = string.Empty;
        var barrier = new IndexMap();

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];

            if (line.StartsWith("///"))
            {
                locationName = line
                    .Replace("///", string.Empty)
                    .ToLowerInvariant();

                barrier.Start = i + 1;
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                barrier.End = i - 1; 
                result.Add(locationName, barrier);

                // reset
                barrier = new IndexMap();
            }
        }

        return result;
    }

    private static string GetZoneName(string line)
    {
        return line
            .Replace("[Info   :Quests Extended] ZoneId: ", string.Empty)
            .Split(" ")[0];
    }

    private static string GetPosition(string line)
    {
        var start = line.IndexOf('(') + 1;
        var end = line.IndexOf(')');

        return line.Substring(start, end - start);
    }

    private static string GetType(string line)
    {
        var text = "Type: ";
        var start = line.IndexOf(text) + text.Length;

        return line.Substring(start, line.Length - start);
    }
}