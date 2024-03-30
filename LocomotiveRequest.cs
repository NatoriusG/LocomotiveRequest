/* Derail Valley */
using DV;
using DV.ThingTypes;
using DV.Utils;
/* Unity Mod Manager */
using UnityModManagerNet;
using HarmonyLib;
/* System */
using System.Collections.Generic;
using System.Linq;

namespace LocomotiveRequest;

/// <summary>
/// Core implementation of mod functionality. A mix of custom types, utility functions, and core logic.
/// </summary>
internal static class LocomotiveRequest
{
    /* Internal Types */

    internal enum LogLevel
    {
        Error,
        Warning,
        Debug
    }

    /// <summary>
    /// Struct to tie together the following data:
    /// * An index uniquely representing the locomotive
    /// * Printable locomotive name
    /// * TrainCarLivery entries representating locomotive car type(s)
    /// </summary>
    internal readonly struct Locomotive
    {
        /* Declaration of locomotives */
        // Making these statically available allows for enum-like access of locomotive objects.
        internal static readonly Locomotive DE2 = new(0);
        internal static readonly Locomotive DM3 = new(1);
        internal static readonly Locomotive DH4 = new(2);
        internal static readonly Locomotive DE6 = new(3);
        internal static readonly Locomotive S060 = new(4);
        internal static readonly Locomotive S282 = new(5);
        internal static readonly Locomotive DE6_Slug = new(6);
        internal static readonly Locomotive[] Locomotives =
            [DE2, DM3, DH4, DE6, S060, S282, DE6_Slug];

        /* Fields */
        private readonly int selectionIndex;
        internal readonly string Name // nicely formatted names
        {
            get => selectionIndex switch
                {
                    0 => "DE2",
                    1 => "DM3",
                    2 => "DH4",
                    3 => "DE6",
                    4 => "S060",
                    5 => "S282",
                    6 => "DE6 Slug",
                    _ => ""
                };
        }
        internal readonly List<TrainCarLivery> Liveries { get => LiveryMap[this]; }

        /* Methods */
        private Locomotive(int selectionIndex) =>
            this.selectionIndex = selectionIndex;

        // Allows using this object to index into an array
        public static implicit operator int(Locomotive locomotive) =>
            locomotive.selectionIndex;
    }



    /* Private fields */
    // A note on the default values below:
    // The livery map is initialized to null to make it easy to check if it has been populated or not.
    // For the available locomotives list, previous populations of the list are irrelevant since it might change as
    // licenses are unlocked.

    internal static UnityModManager.ModEntry.ModLogger? loggerUMM = null;
    internal static LogLevel logLevel = LogLevel.Debug;
    private static List<Locomotive> availableLocomotives = [];
    private static Dictionary<Locomotive, List<TrainCarLivery>>? liveryMap = null;



    /* Properties */

    /// <summary>
    /// Locomotives the player is currently allowed to spawn.
    /// </summary>
    internal static List<Locomotive> AvailableLocomotives
    {
        get
        {
            // Update each time this information is requested.
            // This ensures license changes will propogate quickly.
            UpdateAvailableLocomotives();
            return availableLocomotives;
        }
    }

    /// <summary>
    /// A mapping from each locomotive to its TrainCarLivery type representation.
    /// Used by the CarSpawner spawning method.
    /// </summary>
    internal static Dictionary<Locomotive, List<TrainCarLivery>> LiveryMap
    {
        get
        {
            if (liveryMap == null)
                CreateLiveryMap();
            return liveryMap!;
        }
    }



    /* Logging utility methods */

    private static void Log(string output, string color) =>
        loggerUMM?.Log($"<color={color}>{output}</color>");

    internal static void LogDebug(string output, string color = "white")
    {
        if ((int)logLevel > 1)
            Log(output, color);
    }

    internal static void LogWarning(string output, string color = "yellow")
    {
        if ((int)logLevel > 0)
            Log(output, color);
    }

    internal static void LogError(string output, string color = "red")
    {
        if ((int)logLevel > -1)
            Log(output, color);
    }



    /* Functions to populate properties */

    /// <summary>
    /// Update which locomotives the player is allowed to spawn.
    /// May change as licenses are unlocked.
    /// </summary>
    internal static void UpdateAvailableLocomotives()
    {
        // TODO: check licenses to get which locos can be spawned
        // also could require buying it, or paying a delivery fee?

        LogDebug("Updating available locomotives:");

        availableLocomotives =
        [
            Locomotive.DE2,
            Locomotive.DM3,
            Locomotive.DH4,
            Locomotive.DE6,
            Locomotive.S060,
            Locomotive.S282,
            Locomotive.DE6_Slug
        ];

        foreach (var entry in Locomotive.Locomotives)
        {
            var availableIndicator = availableLocomotives.Contains(entry) ?
                "<color=blue>o</color>" : "<color=yellow>o</color>";
            LogDebug($"> {entry.Name}: {availableIndicator}");
        }
    }

    /// <summary>
    /// Create a mapping from a given locomotive to its TrainCarLivery list type representation.
    /// Should only need to be done once per session.
    /// </summary>
    internal static void CreateLiveryMap()
    {
        LogDebug("Creating livery map...");

        liveryMap = [];
        var liveries = Globals.G.Types.Liveries;

        liveryMap.Add(Locomotive.DE2,
            [
                (
                    from livery in liveries
                    where livery.id == "LocoDE2"
                    select livery
                ).First()
            ]
        );

        liveryMap.Add(Locomotive.DM3,
            [
                (
                    from livery in liveries
                    where livery.id == "LocoDM3"
                    select livery
                ).First()
            ]
        );

        liveryMap.Add(Locomotive.DH4,
            [
                (
                    from livery in liveries
                    where livery.id == "LocoDH4"
                    select livery
                ).First()
            ]
        );

        liveryMap.Add(Locomotive.DE6,
            [
                (
                    from livery in liveries
                    where livery.id == "LocoDE6"
                    select livery
                ).First()
            ]
        );

        liveryMap.Add(Locomotive.S060,
            [
                (
                    from livery in liveries
                    where livery.id == "LocoS060"
                    select livery
                ).First()
            ]
        );

        liveryMap.Add(Locomotive.S282,
            [
                (
                    from livery in liveries
                    where livery.id == "LocoS282A"
                    select livery
                ).First(),
                (
                    from livery in liveries
                    where livery.id == "LocoS282A"
                    select livery
                ).First()
            ]
        );

        liveryMap.Add(Locomotive.DE6_Slug,
            [
                (
                    from livery in liveries
                    where livery.id == "LocoDE6Slug"
                    select livery
                ).First()
            ]
        );

        LogDebug("Livery map created.");
    }



    /* Functions interacting with game objects */

    /// <summary>
    /// Attempt to spawn a locomotive as requested by the player.
    /// </summary>
    /// <param name="requestedLocomotive">The locomotive to be spawned</param>
    /// <param name="result">A message to the player representing the spawn attempt's result</param>
    /// <returns>Whether the locomotive was spawned successfully</returns>
    internal static bool TrySpawnRequestedLocomotive(Locomotive requestedLocomotive, out string result)
    {
        LogDebug("Received request to spawn locomotive.");
        LogDebug($"Requested type: {requestedLocomotive}");

        LogDebug("Identifying locomotive spawners in range...");
        var spawnersInRange =
            from station in StationController.allStations
            from spawner in station.GetComponentsInChildren<StationLocoSpawner>()
            where PlayerInRangeOfStation(station)
            select spawner;
        LogDebug("Spawners in range:");
        foreach (var spawner in spawnersInRange)
            LogDebug($"> {spawner.name}");

        LogDebug("Identifying unocuppied tracks...");
        var availableTracks = (
            from spawner in spawnersInRange
            select spawner.locoSpawnTrack into track
            where TrackAvailable(track)
            select track
        ).ToArray();
        LogDebug("Available tracks:");
        foreach (var track in availableTracks)
            LogDebug($"> {track.name}");

        if (availableTracks.Length == 0)
        {
            LogWarning("Spawning failed due to no available locomotive spawn locations.");
            result = "FAILED:\nNo available tracks! Try moving other locomotives.";
            return false;
        }

        // TODO: Expand track selection to use locomotive parking tracks which don't have associated spawners
        // TODO: Investigate (perhaps optionally) allowing locomotive to simply spawn on the nearest track
        LogDebug("Picking track at random...");
        var selectedTrackIndex = UnityEngine.Random.Range(0, availableTracks.Length - 1);
        var selectedTrack = availableTracks[selectedTrackIndex];
        LogDebug($"Selected track at index {selectedTrackIndex} (\'{selectedTrack.name})");

        // TODO: Investigate possible exceptions
        LogDebug("Attempting to spawn locomotive...");
        SingletonBehaviour<CarSpawner>.Instance.SpawnCarTypesOnTrack(
            trainCarTypes: LiveryMap[requestedLocomotive],
            carsOrientationReversed: null,
            railTrack: selectedTrack,
            preventAutoCoupleOnLastCars: false,
            applyHandbrakeOnLastCars: true
        );
        LogDebug("Locomotive spawned.");

        result = "Locomotive delivered.";
        return true;
    }

    /// <summary>
    /// Determine if the player is allowed to spawn locomotives at a given station.
    /// Uses the job generation zone for consistency's sake.
    /// </summary>
    /// <param name="station">The station to check</param>
    /// <returns>Whether the player is in range</returns>
    private static bool PlayerInRangeOfStation(StationController station) =>
        Traverse.Create(station).Field<bool>("playerEnteredJobGenerationZone").Value;

    /// <summary>
    /// Determine if any cars (including locomotives) are already present on a given track and would block spawning a
    /// new locomotive.
    /// </summary>
    /// <param name="track">The track to check</param>
    /// <returns>Whether the track is unoccupied</returns>
    private static bool TrackAvailable(RailTrack track) =>
        track.logicTrack.GetCarsFullyOnTrack().Count == 0;
}
