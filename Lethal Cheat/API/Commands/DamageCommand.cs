using Brigadier.NET;
using Brigadier.NET.Builder;
using GameNetcodeStuff;
using Rumi.BrigadierForLethalCompany;
using Rumi.BrigadierForLethalCompany.API;
using Unity.Netcode;

namespace Rumi.LethalCheat.API.Commands
{
    public sealed class DamageCommand : ServerCommand
    {
        public const string resultText = "Applied {0} damage to {1}";

        DamageCommand() { }

        public override void Register()
        {
            //damage <int:amount>
            //damage <Entity:destination> <int:amount>
            dispatcher.Register(x =>
                x.Literal("damage")
                    .Requires(x => x.isOp)
                    .Then(x =>
                        x.Argument("amount", Arguments.Integer())
                            .Executes(x =>
                            {
                                if (x.Source.sender != null)
                                {
                                    int amount = Arguments.GetInteger(x, "amount");
                                    DamageEntity(x.Source.sender, amount);

                                    x.Source.SendCommandResult(string.Format(resultText, amount, x.Source.sender.GetEntityName()));

                                    return 1;
                                }
                                else
                                    return 0;
                            })
                    )
                    .Then(x =>
                        x.Argument("targets", LethalArguments.Selector())
                            .Then(x =>
                                x.Argument("amount", Arguments.Integer())
                                    .Executes(x =>
                                    {
                                        var targets = LethalArguments.GetSelector(x, "targets").GetEntitys(x.Source);
                                        int amount = Arguments.GetInteger(x, "amount");
                                        int count = 0;

                                        foreach (var entity in targets)
                                        {
                                            if (entity is not (PlayerControllerB or EnemyAI))
                                                continue;

                                            try
                                            {
                                                DamageEntity(entity, amount);
                                                count++;
                                            }
                                            catch (System.Exception e)
                                            {
                                                Debug.LogError(e);
                                            }
                                        }

                                        x.Source.SendCommandResult(string.Format(resultText, amount, targets.GetEntityName(count)));

                                        return count;
                                    })
                            )
                    )
            );
        }

        /// <summary>
        /// 선택한 엔티티에 데미지를 줍니다
        /// </summary>
        /// <param name="entity">데미지를 줄 <see cref="NetworkBehaviour"/> 오브젝트</param>
        public static void DamageEntity(NetworkBehaviour entity, int damage)
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
                return;

            if (entity is PlayerControllerB or EnemyAI)
                InternalDamageEntityClientRpc(entity, damage);
        }

        [ClientRpc]
        static void InternalDamageEntityClientRpc(NetworkBehaviourReference entityRef, int damage)
        {
            if (!entityRef.TryGet(out var entity))
                return;

            if (entity is PlayerControllerB player)
                player.DamagePlayer(damage);
            else if (entity is EnemyAI enemy)
                enemy.HitEnemy(damage, null, true);
        }
    }
}
