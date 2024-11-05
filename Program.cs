using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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

class Zone
{
    public string Position;
    public string Type;

    public Zone(string line)
    {
        Position = GetPosition(line);
        Type = GetType(line);
    }

    static string GetPosition(string line)
    {
        var start = line.IndexOf('(') + 1;
        var end = line.IndexOf(')');

        return line.Substring(start, end - start);
    }

    static string GetType(string line)
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

        using (var fs = File.OpenRead("./Player.log"))
        {
            using (var sr = new StreamReader(fs))
            {
                while (!sr.EndOfStream)
                {
                    var line = await sr.ReadLineAsync();
                    ParseLine(line, ref result, ref location);
                }
            }
        }

        var json = FormatResult(result);

        await File.WriteAllTextAsync("./questpoints.jsonc", json);
    }

    static void ParseLine(string line, ref Result result, ref Location location)
    {
        if (line.StartsWith("[Info   :RequestHandler] [REQUEST]: /singleplayer/settings/bot/maxCap/"))
        {
            // handle location entry

            var locationName = GetLocationName(line);

            location = new Location()
            {
                Zones = new Dictionary<string, Zone>()
            };

            // add result
            result.Locations.Add(locationName, location);
        }

        if (line.StartsWith("[Info   :Quests Extended] ZoneId: "))
        {
            // handle zone entry

            var zoneName = GetZoneName(line);
            var zone = new Zone(line);

            // add or edit result
            if (!location.Zones.TryAdd(zoneName, zone))
            {
                location.Zones[zoneName] = zone;
            }
        }
    }

    static string GetLocationName(string line)
    {
        return line
            .Replace("[Info   :RequestHandler] [REQUEST]: /singleplayer/settings/bot/maxCap/", string.Empty)
            .Trim()
            .ToLowerInvariant();
    }

    static string GetZoneName(string line)
    {
        return line
            .Replace("[Info   :Quests Extended] ZoneId: ", string.Empty)
            .Split(" ")[0];
    }

    static string FormatResult(Result result)
    {
        var sb = new StringBuilder();

        sb.AppendLine("{");

        foreach (var locationKvp in result.Locations)
        {
            sb.AppendLine($"    \"{locationKvp.Key}\":");
            sb.AppendLine("    {");

            foreach (var zoneKvp in locationKvp.Value.Zones)
            {
                var zone = zoneKvp.Value;
                sb.AppendLine($"        \"{zoneKvp.Key}\": {{ \"position\": \"{zone.Position}\", \"type\": \"{zone.Type}\" }},");
            }

            sb.AppendLine("    },");
        }

        sb.Append("}");

        return sb.ToString();
    }
}
