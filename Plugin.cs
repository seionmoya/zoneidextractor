using System.IO;
using BepInEx;
using ZoneExtractor.Patches;
using ZoneExtractor.Utils;

namespace ZoneExtractor
{
    [BepInPlugin("com.seion.zoneextractor", "zoneextractor", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            new OnGameStartedPatch().Enable();
        }

        private void OnApplicationQuit()
        {
            var json = ZoneParser.GetJson();
            File.WriteAllText("questpoints.jsonc", json);
        }
    }
}