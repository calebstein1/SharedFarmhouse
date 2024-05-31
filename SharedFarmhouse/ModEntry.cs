using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace SharedFarmhouse;

internal sealed class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        var harmony = new Harmony(ModManifest.UniqueID);

        harmony.Patch(
            original: AccessTools.Method(typeof(FarmerTeam), nameof(FarmerTeam.DeleteFarmhand)),
            prefix: new HarmonyMethod(typeof(ModEntry), nameof(DeleteFarmhandPrefix))
        );
    }

    private static bool DeleteFarmhandPrefix()
    {
        return false;
    }
}