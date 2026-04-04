using Brigadier.NET;
using Brigadier.NET.Builder;
using GameNetcodeStuff;
using Rumi.BrigadierForLethalCompany;
using Rumi.BrigadierForLethalCompany.API;
using StaticNetcodeLib;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;

namespace Rumi.LethalCheat.API.Commands
{
    [StaticNetcode]
    public sealed class InvincibilityCommand : ServerCommand
    {
        public const string resultGetText = "Value of invincibility for player {0} is {1}";
        public const string resultSetText = "Value for invincibility for player {0} set to {1}";

        InvincibilityCommand() { }

        public override void Register()
        {
            //invincibility
            //invincibility <Entity:targets> get
            //invincibility <Entity:targets> set <bool:value>
            dispatcher.Register(x =>
                x.Literal("invincibility")
                    .Executes(x =>
                    {
                        if (x.Source.sender is PlayerControllerB player)
                        {
                            SetInvincibility(player, true);
                            x.Source.SendCommandResult(string.Format(resultSetText, x.Source.sender.GetEntityName(), true));

                            return 1;
                        }
                        else
                            return 0;
                    })
                    .Then(x =>
                        x.Argument("target", LethalArguments.Selector(true, true))
                            .Executes(x =>
                            {
                                var target = LethalArguments.GetSelector(x, "target").GetEntitys(x.Source).First();
                                if (target is PlayerControllerB player)
                                {
                                    SetInvincibility(player, true);
                                    x.Source.SendCommandResult(string.Format(resultSetText, target.GetEntityName(), true));

                                    return 1;
                                }

                                return 0;
                            })
                            .Then(x =>
                                x.Literal("get")
                                    .Executes(x =>
                                    {
                                        var target = LethalArguments.GetSelector(x, "target").GetEntitys(x.Source).First();
                                        if (target is PlayerControllerB player)
                                        {
                                            Async();

                                            async void Async()
                                            {
                                                bool? value = await GetInvincibility(player);
                                                x.Source.SendCommandResult(string.Format(resultSetText, target.GetEntityName(), value?.ToString() ?? "timeout"), false);
                                            }

                                            return 1;
                                        }
                                        else
                                            return 0;
                                    })
                            )
                            .Then(x =>
                                x.Literal("set")
                                    .Then(x =>
                                        x.Argument("value", Arguments.Bool())
                                            .Executes(x =>
                                            {
                                                var target = LethalArguments.GetSelector(x, "target").GetEntitys(x.Source).First();
                                                bool value = Arguments.GetBool(x, "value");

                                                if (target is PlayerControllerB player)
                                                {
                                                    SetInvincibility(player, value);
                                                    x.Source.SendCommandResult(string.Format(resultSetText, target.GetEntityName(), value));

                                                    return 1;
                                                }

                                                return 0;
                                            })
                                    )
                            )
                    )
            );
        }

        static Dictionary<PlayerControllerB, bool> invincibilityList = new();

        /// <summary>
        /// 선택한 플레이어의 무적 여부를 변경합니다.
        /// </summary>
        /// <param name="entity">변경할 <see cref="PlayerControllerB"/> 오브젝트</param>
        public static async Task<bool?> GetInvincibility(PlayerControllerB player)
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
                return false;

            //클라에 요청 보내기
            if (invincibilityList.ContainsKey(player))
                invincibilityList.Remove(player);

            InternalGetInvincibilityClientRpc(player);

            Stopwatch stopwatch = Stopwatch.StartNew();
            while (!invincibilityList.ContainsKey(player))
            {
                if (stopwatch.ElapsedMilliseconds >= 3000)
                    return null;

                await Task.Delay(1);
            }

            bool result = invincibilityList[player];
            invincibilityList.Remove(player);

            return result;
        }

        [ClientRpc]
        static void InternalGetInvincibilityClientRpc(NetworkBehaviourReference entityRef)
        {
            if (!entityRef.TryGet(out PlayerControllerB player) || player != GameNetworkManager.Instance.localPlayerController)
                return;

            //클라에서 요청 수신
            InternalGetInvincibilityServerRpc(player, !StartOfRound.Instance.allowLocalPlayerDeath);
        }

        [ServerRpc]
        static void InternalGetInvincibilityServerRpc(NetworkBehaviourReference entityRef, bool value)
        {
            if (entityRef.TryGet(out PlayerControllerB player))
                invincibilityList[player] = value;
        }

        /// <summary>
        /// 선택한 플레이어의 무적 여부를 변경합니다.
        /// </summary>
        /// <param name="entity">변경할 <see cref="PlayerControllerB"/> 오브젝트</param>
        public static void SetInvincibility(PlayerControllerB player, bool value)
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
                return;

            InternalSetInvincibilityClientRpc(player, value);
        }

        [ClientRpc]
        static void InternalSetInvincibilityClientRpc(NetworkBehaviourReference entityRef, bool value)
        {
            if (!entityRef.TryGet(out PlayerControllerB player) || player != GameNetworkManager.Instance.localPlayerController)
                return;

            StartOfRound.Instance.allowLocalPlayerDeath = !value;
        }
    }
}
