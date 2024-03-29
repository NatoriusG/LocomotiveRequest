using UnityEngine;
using UnityModManagerNet;
using DV.ThingTypes;
using DV.Utils;
using DV.Logic.Job;
using DVModApi;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using System;
using DV;

namespace LocomotiveRequest
{
    static class Main
    {
        public static bool enabled;
        public static UnityModManager.ModEntry mod = null!;
        private static bool gameLoaded = false;
        private static float elapsedTime = 0f;

        private static Dictionary<RequestableLocomotives, List<TrainCarLivery>>? liveries = null;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            mod = modEntry;
            mod.OnToggle = OnToggle;
            mod.OnUpdate = OnUpdate;
            DVModAPI.Setup(mod, FunctionType.OnGameLoad, () => gameLoaded = true);
            return true;
        }

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            // TODO: toggle logic
            enabled = value;
            return true;
        }

        static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {
            if (gameLoaded)
            {
                elapsedTime += dt;
                if (elapsedTime >= 5f)
                {
                    //var randomIndex = UnityEngine.Random.Range(0, 6);
                    //TrySpawnRequestedLocomotive((RequestableLocomotives)randomIndex);
                    elapsedTime = 0;
                }
            }
        }

        static void Log(string output)
        {
            mod.Logger.Log($"<color=green>{output}</color>");
        }

        static void PopulateLiveryDictionary()
        {
            liveries = [];

            var allLiveries = Globals.G.Types.Liveries;
            Log($"Populating livery dictionary. Liveries found: {allLiveries.Count}");

            var DE2Livery = (
                from livery in allLiveries
                where livery.id == "LocoDE2"
                select livery
            ).FirstOrDefault();
            liveries.Add(RequestableLocomotives.DE2, [DE2Livery]);

            var DM3Livery = (
                from livery in allLiveries
                where livery.id == "LocoDM3"
                select livery
            ).FirstOrDefault();
            liveries.Add(RequestableLocomotives.DM3, [DM3Livery]);

            var DH4Livery = (
                from livery in allLiveries
                where livery.id == "LocoDH4"
                select livery
            ).FirstOrDefault();
            liveries.Add(RequestableLocomotives.DH4, [DH4Livery]);

            var DE6Livery = (
                from livery in allLiveries
                where livery.id == "LocoDE6"
                select livery
            ).FirstOrDefault();
            liveries.Add(RequestableLocomotives.DE6, [DE6Livery]);

            var DE6SlugLivery = (
                from livery in allLiveries
                where livery.id == "LocoDE6Slug"
                select livery
            ).FirstOrDefault();
            liveries.Add(RequestableLocomotives.DE6Slug, [DE6SlugLivery]);

            var S060Livery = (
                from livery in allLiveries
                where livery.id == "LocoS060"
                select livery
            ).FirstOrDefault();
            liveries.Add(RequestableLocomotives.S060, [S060Livery]);

            var S282ALivery = (
                from livery in allLiveries
                where livery.id == "LocoS282A"
                select livery
            ).FirstOrDefault();
            var S282BLivery = (
                from livery in allLiveries
                where livery.id == "LocoS282B"
                select livery
            ).FirstOrDefault();
            liveries.Add(RequestableLocomotives.S282, [S282ALivery, S282BLivery]);
        }

        static void TrySpawnRequestedLocomotive(RequestableLocomotives requested)
        {
            Log($"Received request to spawn locomotive '{requested}'");

            var locoSpawnersInRange =
                from station in StationController.allStations
                from spawner in station.GetComponentsInChildren<StationLocoSpawner>()
                where Traverse.Create(station).Field<bool>("playerEnteredJobGenerationZone").Value
                select spawner;

            var availableLocoSpawners =
                from spawner in locoSpawnersInRange
                where spawner.GetLocoTypesCurrentlyOnSpawnTrack().Count() == 0
                select spawner;

            if (availableLocoSpawners.Count() == 0)
            {
                Log("No available locomotive spawn locations.");
                return;
            }

            var spawnerIndex = UnityEngine.Random.Range(0, availableLocoSpawners.Count() - 1);
            Log($"Spawning from spawner index {spawnerIndex}");

            if (liveries == null)
                PopulateLiveryDictionary();

            SingletonBehaviour<CarSpawner>.Instance.SpawnCarTypesOnTrack(
                liveries![requested],
                null,
                availableLocoSpawners.ToArray()[spawnerIndex].locoSpawnTrack,
                false,
                true
            );
        }
    }

    enum RequestableLocomotives
    {
        DE2,
        DM3,
        DH4,
        DE6,
        DE6Slug,
        S060,
        S282
    }
}
