using HarmonyLib;
using Rumi.BrigadierForLethalCompany.Components;
using Rumi.BrigadierForLethalCompany.Networking;
using UnityEngine.EventSystems;

namespace Rumi.BrigadierForLethalCompany
{
    /// <summary>
    /// Responsible for general patches for BFLC mods<br/>
    /// <br/><br/>
    /// BFLC 모드의 일반적인 패치를 담당합니다<br/>
    /// 채팅에서 명령어를 입력할 수 있게 <see cref="HUDManager.Start"/>, <see cref="HUDManager.SubmitChat_performed"/> 메소드를 패치합니다
    /// </summary>
    public static class BFLCPatches
    {
        /// <summary><see cref="HUDManager.Start"/> 메소드를 패치합니다</summary>
        public static void Patch()
        {
            Debug.Log($"{nameof(HUDManager)} Patch...");

            try
            {
                BFLCPlugin.harmony.PatchAll(typeof(BFLCPatches));
                Debug.Log($"{nameof(HUDManager)} Patched!");
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                Debug.Log($"{nameof(HUDManager)} Patch Fail!");
            }
        }

        [HarmonyPatch(typeof(HUDManager), nameof(HUDManager.Start))]
        [HarmonyPostfix]
        static void HUDManager_Start_Postfix(HUDManager __instance)
        {
            CommandIntelliSense.Create(__instance);
            __instance.chatTextField.characterLimit = 0;
        }

        [HarmonyPatch(typeof(HUDManager), nameof(HUDManager.SubmitChat_performed))]
        [HarmonyPrefix]
        static bool HUDManager_SubmitChat_performed_Prefix(HUDManager __instance)
        {
            string text = __instance.chatTextField.text;
            if (!text.StartsWith("/"))
                return true;

            text = text.Remove(0, 1);

            BFLCNetworkHandler.ExecuteCommand(text);

            __instance.chatTextField.text = "";
            __instance.localPlayer.isTypingChat = false;
            __instance.typingIndicator.enabled = false;

            EventSystem.current.SetSelectedGameObject(null);
            return false;
        }
    }
}
