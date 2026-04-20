using Brigadier.NET.Builder;
using Rumi.BrigadierForLethalCompany;
using Rumi.BrigadierForLethalCompany.API;
using StaticNetcodeLib;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Rumi.LethalCheat.API.Commands
{
    [StaticNetcode]
    public sealed class TeleportCommand : ServerCommand
    {
        public const string resultEntityText = "Teleported {0} to {1}";
        public const string resultLocationText = "Teleported {0} to {1}, {2}, {3}";

        TeleportCommand() { }

        public override void Register()
        {
            //teleport <Vector3:location>
            //teleport <Entity:destination>
            //teleport <Entity[]:targets> <Vector3:location>
            //teleport <Entity[]:targets> <Entity:destination>
            var node =
            dispatcher.Register(x =>
                x.Literal("teleport")
                    .Requires(x => x.isOp)
                    .Then(x =>
                        x.Argument("location", LethalArguments.Vector3())
                            .Executes(x =>
                            {
                                if (x.Source.sender != null)
                                {
                                    Vector3 position = LethalArguments.GetVector3(x, "location");
                                    TeleportEntity(x.Source.sender, position);

                                    x.Source.SendCommandResult(string.Format(resultLocationText, x.Source.sender.GetEntityName(), position.x, position.y, position.z));

                                    return 1;
                                }

                                return 0;
                            })
                    )
                    .Then(x =>
                        x.Argument("destination", LethalArguments.Selector(false, true))
                            .Executes(x =>
                            {
                                if (x.Source.sender != null)
                                {
                                    var targets = LethalArguments.GetSelector(x, "destination").GetEntitys(x.Source);
                                    if (targets.CountIsOne())
                                    {
                                        TeleportEntity(x.Source.sender, targets.First().transform.position);
                                        x.Source.SendCommandResult(string.Format(resultEntityText, x.Source.sender.GetEntityName(), targets.GetEntityName()));

                                        return 1;
                                    }
                                }

                                return 0;
                            })
                    )
                    .Then(x =>
                        x.Argument("targets", LethalArguments.Selector(false))
                            .Then(x =>
                                x.Argument("location", LethalArguments.Vector3())
                                    .Executes(x =>
                                    {
                                        var targets = LethalArguments.GetSelector(x, "targets").GetEntitys(x.Source);
                                        Vector3 position = LethalArguments.GetVector3(x, "location");

                                        int count = 0;
                                        foreach (var target in targets)
                                        {
                                            TeleportEntity(target, position);
                                            count++;
                                        }

                                        x.Source.SendCommandResult(string.Format(resultLocationText, targets.GetEntityName(count), position.x, position.y, position.z));

                                        return count;
                                    })
                            )
                            .Then(x =>
                                x.Argument("destination", LethalArguments.Selector(false, true))
                                    .Executes(x =>
                                    {
                                        var targets = LethalArguments.GetSelector(x, "targets").GetEntitys(x.Source);
                                        var destination = LethalArguments.GetSelector(x, "destination").GetEntitys(x.Source);

                                        if (destination.CountIsOne())
                                        {
                                            var location = destination.First().transform.position;

                                            int count = 0;
                                            foreach (var target in targets)
                                            {
                                                TeleportEntity(target, location);
                                                count++;
                                            }

                                            x.Source.SendCommandResult(string.Format(resultEntityText, targets.GetEntityName(), destination.GetEntityName()));

                                            return count;
                                        }

                                        return 0;
                                    })
                            )
                    )
            );

            //tp -> teleport
            dispatcher.Register(x =>
                x.Literal("tp")
                    .Requires(x => x.isOp)
                    .Redirect(node)
            );
        }

        /// <summary>
        /// 선택한 엔티티를 특정 좌표로 텔레포트합니다
        /// </summary>
        /// <param name="entity">텔레포트할 <see cref="NetworkBehaviour"/> 오브젝트</param>
        /// <param name="position">텔레포트할 좌표</param>
        public static void TeleportEntity(NetworkBehaviour entity, Vector3 position)
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
                return;

            if (entity is EnemyAI enemy)
            {
                enemy.SetEnemyOutside(enemy.transform.position.y >= -100);
                enemy.agent.Warp(position);
            }
            else if (entity is GrabbableObject item)
                item.FallToGround(false, false, position);

            entity.transform.position = position;
            InternalTeleportEntityClientRpc(entity, position);
        }

        [ClientRpc]
        static void InternalTeleportEntityClientRpc(NetworkBehaviourReference entityRef, Vector3 position)
        {
            if (entityRef.TryGet(out var entity))
            {
                if (entity is EnemyAI enemy)
                {
                    enemy.SetEnemyOutside(enemy.transform.position.y >= -100);
                    enemy.agent.Warp(position);
                }
                else if (entity is GrabbableObject item)
                    item.FallToGround(false, false, position);

                entity.transform.position = position;
            }
        }
    }
}
