using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;
using xTile.Dimensions;

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

        harmony.Patch(
            original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction), new [] { typeof(string[]), typeof(Farmer), typeof(Location)}),
            transpiler: new HarmonyMethod(typeof(ModEntry), nameof(FixMailboxTranspiler))
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

    private static IEnumerable<CodeInstruction> FixMailboxTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var output = new List<CodeInstruction>();
        
        foreach (var code in instructions)
        {
            if (code.opcode == OpCodes.Callvirt && (code.operand as MethodInfo)?.Name == "get_IsOwnedByCurrentPlayer")
            {
                output.RemoveAt(output.Count - 1);
                output.Add(new CodeInstruction(OpCodes.Ldc_I4_1));
                continue;
            }
            output.Add(code);
        }

        return output.AsEnumerable();
    }
}