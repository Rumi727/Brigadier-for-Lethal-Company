using Brigadier.NET.Builder;
using GameNetcodeStuff;
using Rumi.BrigadierForLethalCompany;
using Rumi.BrigadierForLethalCompany.API;
using Unity.Netcode;
using UnityEngine;

namespace Rumi.LethalCheat.API.Commands
{
    public sealed class KillCommand : ServerCommand
    {
        public const string resultText = "Killed {0}";

        KillCommand() { }

        public override void Register()
        {
            //kill
            //kill <Entity:targets>
            dispatcher.Register(x =>
                x.Literal("kill")
                    .Requires(x => x.isOp)
                    .Executes(x =>
                    {
                        if (x.Source.sender != null)
                        {
                            KillEntity(x.Source.sender);
                            x.Source.SendCommandResult(string.Format(resultText, x.Source.sender.GetEntityName()));

                            return 1;
                        }
                        else
                            return 0;
                    })
                    .Then(x =>
                        x.Argument("targets", LethalArguments.Selector())
                            .Executes(x =>
                            {
                                var targets = LethalArguments.GetSelector(x, "targets").GetEntitys(x.Source);
                                int count = 0;

                                foreach (var entity in targets)
                                {
                                    if (entity is not (PlayerControllerB or EnemyAI))
                                        continue;

                                    try
                                    {
                                        KillEntity(entity);
                                        count++;
                                    }
                                    catch (System.Exception e)
                                    {
                                        Debug.LogError(e);
                                    }
                                }

                                x.Source.SendCommandResult(string.Format(resultText, targets.GetEntityName(count)));

                                return count;
                            })
                    )
            );
        }

        /// <summary>
        /// 선택한 엔티티를 죽입니다 (죽일 수 없을 경우, 파괴합니다)
        /// </summary>
        /// <param name="entity">죽일 <see cref="NetworkBehaviour"/> 오브젝트</param>
        public static void KillEntity(NetworkBehaviour entity)
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
                return;

            if (entity is PlayerControllerB or EnemyAI)
                InternalKillEntityClientRpc(entity);
        }

        [ClientRpc]
        static void InternalKillEntityClientRpc(NetworkBehaviourReference entityRef)
        {
            if (!entityRef.TryGet(out var entity))
                return;

            if (entity is PlayerControllerB player)
                player.KillPlayer(Vector3.up * 5);
            else if (entity is EnemyAI enemy)
                enemy.KillEnemy();
        }
    }
}
