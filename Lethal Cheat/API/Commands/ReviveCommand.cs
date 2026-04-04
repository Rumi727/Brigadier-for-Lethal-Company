using Brigadier.NET.Builder;
using GameNetcodeStuff;
using Rumi.BrigadierForLethalCompany;
using Rumi.BrigadierForLethalCompany.API;
using StaticNetcodeLib;
using Unity.Netcode;
using UnityEngine;

namespace Rumi.LethalCheat.API.Commands
{
    [StaticNetcode]
    public sealed class ReviveCommand : ServerCommand
    {
        public const string resultText = "Revived {targets}";

        ReviveCommand() { }

        public override void Register()
        {
            //revive
            //revive <Entity:targets>
            dispatcher.Register(x =>
                x.Literal("revive")
                    .Executes(x =>
                    {
                        if (x.Source.sender is PlayerControllerB player)
                        {
                            RevivePlayer(player);
                            x.Source.SendCommandResult(resultText.Replace("{targets}", x.Source.sender.GetEntityName()));

                            return 1;
                        }
                        else
                            return 0;
                    })
                    .Then(x => 
                        x.Argument("targets", LethalArguments.Selector(true))
                            .Executes(x =>
                            {
                                var targets = LethalArguments.GetSelector(x, "targets").GetEntitys(x.Source);
                                int count = 0;

                                foreach (var entity in targets)
                                {
                                    if (entity is not PlayerControllerB player)
                                        continue;

                                    try
                                    {
                                        RevivePlayer(player);
                                        count++;
                                    }
                                    catch (System.Exception e)
                                    {
                                        Debug.LogError(e);
                                    }
                                }

                                x.Source.SendCommandResult(resultText.Replace("{targets}", targets.GetEntityName(count)));

                                return count;
                            })
                    )
            );
        }

        /// <summary>
        /// 선택한 플레이어를 부활시킵니다.
        /// </summary>
        /// <param name="entity">부활시킬 <see cref="PlayerControllerB"/> 오브젝트</param>
        public static void RevivePlayer(PlayerControllerB player)
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
                return;

            InternalRevivePlayerClientRpc(player);
        }

        [ClientRpc]
        static void InternalRevivePlayerClientRpc(NetworkBehaviourReference entityRef)
        {
            if (!entityRef.TryGet(out PlayerControllerB targetPlayer) || !targetPlayer.isPlayerDead)
                return;

            StartOfRound.Instance.allPlayersDead = false;

            for (int i = 0; i < StartOfRound.Instance.allPlayerScripts.Length; i++)
            {
                if (targetPlayer != StartOfRound.Instance.allPlayerScripts[i])
                    continue;

                Debug.Log("Reviving players A");
                targetPlayer.ResetPlayerBloodObjects(targetPlayer.isPlayerDead);
                if (!targetPlayer.isPlayerDead && !targetPlayer.isPlayerControlled)
                    continue;

                targetPlayer.isClimbingLadder = false;
                if (!targetPlayer.inSpecialInteractAnimation || targetPlayer.currentTriggerInAnimationWith == null || !targetPlayer.currentTriggerInAnimationWith.GetComponentInChildren<MoveToExitSpecialAnimation>())
                {
                    targetPlayer.clampLooking = false;
                    targetPlayer.gameplayCamera.transform.localEulerAngles = new Vector3(targetPlayer.gameplayCamera.transform.localEulerAngles.x, 0f, targetPlayer.gameplayCamera.transform.localEulerAngles.z);
                    targetPlayer.inVehicleAnimation = false;
                }

                targetPlayer.overridePoisonValue = false;
                targetPlayer.disableMoveInput = false;
                targetPlayer.ResetZAndXRotation();
                targetPlayer.thisController.enabled = true;
                targetPlayer.health = 100;
                targetPlayer.hasBeenCriticallyInjured = false;
                targetPlayer.disableLookInput = false;
                targetPlayer.disableInteract = false;
                targetPlayer.nightVisionRadar.enabled = false;
                Debug.Log("Reviving players B");
                targetPlayer.isPlayerDead = false;

                targetPlayer.enemyWaitingForBodyRagdoll = null;
                targetPlayer.isPlayerControlled = true;
                targetPlayer.isInElevator = true;
                targetPlayer.isInHangarShipRoom = true;
                targetPlayer.isInsideFactory = false;
                targetPlayer.parentedToElevatorLastFrame = false;
                targetPlayer.overrideGameOverSpectatePivot = null;
                StartOfRound.Instance.SetPlayerObjectExtrapolate(enable: false);
                targetPlayer.TeleportPlayer(StartOfRound.Instance.GetPlayerSpawnPosition(i));
                targetPlayer.setPositionOfDeadPlayer = false;
                targetPlayer.DisablePlayerModel(targetPlayer.gameObject, enable: true, disableLocalArms: true);
                targetPlayer.helmetLight.enabled = false;
                Debug.Log("Reviving players C");
                targetPlayer.Crouch(crouch: false);
                targetPlayer.criticallyInjured = false;
                if (targetPlayer.playerBodyAnimator != null)
                    targetPlayer.playerBodyAnimator.SetBool("Limp", false);

                targetPlayer.bleedingHeavily = false;
                targetPlayer.activatingItem = false;
                targetPlayer.twoHanded = false;
                targetPlayer.inShockingMinigame = false;
                targetPlayer.inSpecialInteractAnimation = false;
                targetPlayer.freeRotationInInteractAnimation = false;
                targetPlayer.disableSyncInAnimation = false;
                targetPlayer.inAnimationWithEnemy = null;
                targetPlayer.holdingWalkieTalkie = false;
                targetPlayer.speakingToWalkieTalkie = false;
                Debug.Log("Reviving players D");
                targetPlayer.isSinking = false;
                targetPlayer.isUnderwater = false;
                targetPlayer.sinkingValue = 0f;
                targetPlayer.statusEffectAudio.Stop();
                targetPlayer.DisableJetpackControlsLocally();
                targetPlayer.health = 100;
                Debug.Log("Reviving players E");
                targetPlayer.mapRadarDotAnimator.SetBool("dead", false);
                targetPlayer.externalForceAutoFade = Vector3.zero;
                targetPlayer.carryWeight = 1f;

                if (targetPlayer.IsOwner)
                {
                    HUDManager.Instance.SetCracksOnVisor(100f);
                    HUDManager.Instance.gasHelmetAnimator.SetBool("gasEmitting", false);
                    targetPlayer.hasBegunSpectating = false;
                    HUDManager.Instance.RemoveSpectateUI();
                    HUDManager.Instance.gameOverAnimator.SetTrigger("revive");
                    targetPlayer.hinderedMultiplier = 1f;
                    targetPlayer.isMovementHindered = 0;
                    targetPlayer.sourcesCausingSinking = 0;
                    StartOfRound.Instance.SendChangedWeightEvent();
                    Debug.Log("Reviving players E2");
                    targetPlayer.reverbPreset = StartOfRound.Instance.shipReverb;
                }

                Debug.Log("Reviving players F");
                HUDManager.Instance.spitOnCameraAlpha = 1f;
                SoundManager.Instance.earsRingingTimer = 0f;
                SoundManager.Instance.alternateEarsRinging = false;
                HUDManager.Instance.cadaverFilter = 0f;
                targetPlayer.voiceMuffledByEnemy = false;
                SoundManager.Instance.playerVoicePitchTargets[i] = 1f;
                SoundManager.Instance.SetPlayerPitch(1f, i);
                if (targetPlayer.currentVoiceChatIngameSettings == null)
                    StartOfRound.Instance.RefreshPlayerVoicePlaybackObjects();

                if (targetPlayer.currentVoiceChatIngameSettings != null)
                {
                    if (targetPlayer.currentVoiceChatIngameSettings.voiceAudio == null)
                        targetPlayer.currentVoiceChatIngameSettings.InitializeComponents();

                    if (targetPlayer.currentVoiceChatIngameSettings.voiceAudio == null)
                        return;

                    targetPlayer.currentVoiceChatIngameSettings.voiceAudio.GetComponent<OccludeAudio>().overridingLowPass = false;
                }

                Debug.Log("Reviving players G");
            }

            if (targetPlayer == GameNetworkManager.Instance.localPlayerController)
            {
                targetPlayer.bleedingHeavily = false;
                targetPlayer.criticallyInjured = false;
                if (targetPlayer.playerBodyAnimator != null)
                    targetPlayer.playerBodyAnimator.SetBool("Limp", false);

                targetPlayer.health = 100;
                HUDManager.Instance.UpdateHealthUI(100, false);

                targetPlayer.spectatedPlayerScript = null;
                HUDManager.Instance.audioListenerLowPass.enabled = false;

                Debug.Log("Reviving players H");
                StartOfRound.Instance.SetSpectateCameraToGameOverMode(false, targetPlayer);

                StartOfRound.Instance.UpdatePlayerVoiceEffects();
                StartOfRound.Instance.ResetMiscValues();

                HUDManager.Instance.HideHUD(false);
            }

            StartOfRound.Instance.livingPlayers++;
        }
    }
}
