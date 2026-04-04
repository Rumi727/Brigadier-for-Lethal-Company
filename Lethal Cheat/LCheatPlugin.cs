using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace Rumi.LethalCheat
{
    [BepInPlugin(modGuid, modName, modVersion)]
    [BepInDependency("Rumi.BrigadierForLethalCompany")]
    [BepInDependency(StaticNetcodeLib.StaticNetcodeLib.Guid, BepInDependency.DependencyFlags.HardDependency)]
    public sealed class LCheatPlugin : BaseUnityPlugin
    {
        public const string modGuid = "Rumi.LethalCheat";
        public const string modName = "LethalCheat";
        public const string modVersion = "2.0.0";

        internal static ManualLogSource? logger { get; private set; } = null;

        internal static Harmony harmony { get; } = new Harmony(modGuid);



        void Awake()
        {
            logger = Logger;

            Debug.Log("Start loading plugin...");

            Debug.Log($"Plugin {modName} is loaded!");
        }
    }
}
