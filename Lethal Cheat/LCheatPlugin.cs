using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Rumi.LCNetworks;

namespace Rumi.LethalCheat
{
    [BepInPlugin(modGuid, modName, modVersion)]
    [BepInDependency("Rumi.LCNetworkHandler")]
    [BepInDependency("Rumi.BrigadierForLethalCompany")]
    public sealed class LCheatPlugin : BaseUnityPlugin
    {
        public const string modGuid = "Rumi.LethalCheat";
        public const string modName = "LethalCheat";
        public const string modVersion = "1.0.1";

        internal static ManualLogSource? logger { get; private set; } = null;

        internal static Harmony harmony { get; } = new Harmony(modGuid);



        void Awake()
        {
            logger = Logger;

            Debug.Log("Start loading plugin...");

            LCNHPlugin.NetcodePatcher();

            Debug.Log($"Plugin {modName} is loaded!");
        }
    }
}
