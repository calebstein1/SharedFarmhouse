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

        harmony.Patch(
            original: AccessTools.Method(typeof(GameServer), nameof(GameServer.IsFarmhandAvailable)),
            postfix: new HarmonyMethod(typeof(ModEntry), nameof(IsFarmhandAvailablePostfix))
        );
    }

    private static bool DeleteFarmhandPrefix()
    {
        return false;
    }

    private static void IsFarmhandAvailablePostfix(ref bool __result)
    {
        __result = true;
    }
}