using System.Collections.Generic;
using System.IO;
using System.Text;
using ZoneExtractor.Models;

namespace ZoneExtractor.Utils
{
    public static class ZoneParser
    {
        //                                 locId
        private static readonly Dictionary<string, Location> _locations;

        static ZoneParser()
        {
            _locations = new Dictionary<string, Location>();
        }

        public static void AddLocation(string locationId)
        {
            var location = new Location()
            {
                Zones = new Dictionary<string, Zone>()
            };

            if (!_locations.ContainsKey(locationId))
            {
                _locations.Add(locationId, location);
            }
            else
            {
                _locations[locationId] = location;
            }
        }

        public static void AddZone(string locationId, string zoneId, Zone zone)
        {
            var zones = _locations[locationId].Zones;

            if (!zones.ContainsKey(zoneId))
            {
                zones.Add(zoneId, zone);
            }
            else
            {
                zones[zoneId] = zone;
            }
        }

        public static string GetJson()
        {
            var sb = new StringBuilder();

            sb.AppendLine("{");

            foreach (var locationKvp in _locations)
            {
                sb.AppendLine($"    \"{locationKvp.Key}\":");
                sb.AppendLine("    {");

                foreach (var zoneKvp in locationKvp.Value.Zones)
                {
                    var zone = zoneKvp.Value;
                    sb.AppendLine($"        \"{zoneKvp.Key}\": {{ \"type\": \"{zone.Type}\", \"position\": \"{zone.Position}\" }},");
                }

                sb.AppendLine("    },");
            }

            sb.Append("}");

            return sb.ToString();
        }

        public static void WriteJson()
        {
            var json = GetJson();
            File.WriteAllText("questpoints.jsonc", json);
        }
    }
}