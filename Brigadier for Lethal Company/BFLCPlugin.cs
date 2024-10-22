using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Rumi.LCNetworks;

namespace Rumi.BrigadierForLethalCompany
{
    /// <summary>
    /// This is the main class of Brigadier For Lethal Company mod.
    /// <br/><br/>
    /// Brigadier For Lethal Company 모드의 메인 클래스입니다
    /// </summary>
    [BepInPlugin(modGuid, modName, modVersion)]
    [BepInDependency("Rumi.LCNetworkHandler")]
    public sealed class BFLCPlugin : BaseUnityPlugin
    {
        /// <summary>
        /// GUID of this mod
        /// <br/><br/>
        /// 이 모드의 GUID
        /// </summary>
        public const string modGuid = "Rumi.BrigadierForLethalCompany";

        /// <summary>
        /// Name of this mod
        /// <br/><br/>
        /// 이 모드의 이름
        /// </summary>
        public const string modName = "Brigadier for Lethal Company";

        /// <summary>
        /// Version of this mod
        /// <br/><br/>
        /// 이 모드의 버전
        /// </summary>
        public const string modVersion = "1.0.1";

        internal static ManualLogSource? logger { get; private set; } = null;

        internal static Harmony harmony { get; } = new Harmony(modGuid);



        void Awake()
        {
            logger = Logger;

            Debug.Log("Start loading plugin...");

            LCNHPlugin.NetcodePatcher();
            BFLCPatches.Patch();

            Debug.Log($"Plugin {modName} is loaded!");
        }
    }
}
