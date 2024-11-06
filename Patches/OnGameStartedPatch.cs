using System.Reflection;
using UnityEngine;
using EFT;
using EFT.Interactive;
using SPT.Reflection.Patching;
using ZoneExtractor.Models;
using ZoneExtractor.Utils;

namespace ZoneExtractor.Patches
{
    internal class OnGameStartedPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(GameWorld).GetMethod(nameof(GameWorld.OnGameStarted));
        }

        [PatchPostfix]
        private static void Postfix(GameWorld __instance)
        {
            // add location
            var locationId = __instance.LocationId;

            ZoneParser.AddLocation(locationId);

            // add zones
            var zoneInstances = Object.FindObjectsOfType<TriggerWithId>();

            foreach (var zoneInstance in zoneInstances)
            {
                var zone = new Zone()
                {
                    Type = zoneInstance.GetType().ToString(),
                    Position = zoneInstance.transform.position.ToString()
                };

                ZoneParser.AddZone(locationId, zoneInstance.Id, zone);
            }

            // re-write file
            ZoneParser.WriteJson();
        }
    }
}