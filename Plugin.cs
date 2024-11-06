using BepInEx;
using ZoneExtractor.Patches;

namespace ZoneExtractor
{
    [BepInPlugin("com.seion.zoneextractor", "zoneextractor", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            new OnGameStartedPatch().Enable();
        }
    }
}