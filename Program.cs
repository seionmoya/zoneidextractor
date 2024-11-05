using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

class Result
{
    //                locid
    public Dictionary<string, Location> Locations;
}

class Location
{
    //                zoneid
    public Dictionary<string, Zone> Zones;
}

// members is lowercase for parsing
class Zone
{
    public string position;
    public string type;

    public Zone(string line)
    {
        position = GetPosition(line);
        type = GetType(line);
    }

    private string GetPosition(string line)
    {
        var start = line.IndexOf('(') + 1;
        var end = line.IndexOf(')');

        return line.Substring(start, end - start);
    }

    private string GetType(string line)
    {
        var text = "Type: ";
        var start = line.IndexOf(text) + text.Length;

        return line.Substring(start, line.Length - start);
    }
}

class Program
{
    static async Task Main()
    {
        var location = default(Location);
        var result = new Result()
        {
            Locations = new Dictionary<string, Location>()
        };

        // read file
        using (var fs = File.OpenRead("./zonesdump"))
        {
            using (var sr = new StreamReader(fs))
            {
                // read lines
                while (!sr.EndOfStream)
                {
                    var line = await sr.ReadLineAsync();
                    ParseLine(line, ref result, ref location);
                }
            }
        }

        // save to file
        var json = JsonConvert.SerializeObject(result, Formatting.Indented);

        File.WriteAllText("./questpoints.jsonc", json);
    }

    private static void ParseLine(string line, ref Result result, ref Location location)
    {
        if (line.StartsWith("///"))
        {
            // location identifier
            var locationName = line.Replace("///", string.Empty);

            location = new Location()
            {
                Zones = new Dictionary<string, Zone>()
            };

            result.Locations.Add(locationName, location);
        }

        if (line.StartsWith("[Info   :Quests Extended]"))
        {
            // zone block
            var zoneName = GetZoneName(line);
            var zone = new Zone(line);

            // add or edit result
            if (!location.Zones.TryAdd(zoneName, zone))
            {
                location.Zones[zoneName] = zone;
            }
        }
    }

    private static string GetZoneName(string line)
    {
        return line
            .Replace("[Info   :Quests Extended] ZoneId: ", string.Empty)
            .Split(" ")[0];
    }
}
