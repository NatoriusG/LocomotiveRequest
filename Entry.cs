using CommsRadioAPI;
using DVModApi;
using UnityEngine;
using UnityModManagerNet;

namespace LocomotiveRequest;

/// <summary>
/// Entry class for Unity Mod Manager to use.
/// </summary>
static class Entry
{
    static bool Load(UnityModManager.ModEntry modEntry)
    {
        // Bind UMM methods
        // TODO: investigate override CommsRadioAPI to allow disabling without restarting the game

        // Set up logging
        LocomotiveRequest.loggerUMM = modEntry.Logger;

        // Add the comms radio screens once the game loads
        DVModAPI.Setup(modEntry, FunctionType.OnGameLoad, () =>
            CommsRadioMode.Create
            (
                new RadioLocomotiveRequestEntry(),
                laserColor: null,
                insertBefore: mode => mode == ControllerAPI.GetVanillaMode(VanillaMode.SummonCrewVehicle)
            ));

        // Inform UMM loading is done
        return true;
    }

}
