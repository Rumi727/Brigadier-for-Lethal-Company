using Brigadier.NET.Builder;
using GameNetcodeStuff;
using Rumi.BrigadierForLethalCompany;
using Rumi.BrigadierForLethalCompany.API;
using Unity.Netcode;

namespace Rumi.LethalCheat.API.Commands
{
    public sealed class DestroyCommand : ServerCommand
    {
        public const string resultText = "Destroyed {0}";

        DestroyCommand() { }

        public override void Register()
        {
            //destroy
            //destroy <Entity:destination>
            dispatcher.Register(x =>
                x.Literal("destroy")
                    .Requires(x => x.isOp)
                    .Executes(x =>
                    {
                        if (x.Source.sender != null && x.Source.player == null)
                        {
                            DestroyEntity(x.Source.sender);
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
                                if (x.Source.sender != null)
                                {
                                    var targets = LethalArguments.GetSelector(x, "targets").GetEntitys(x.Source);
                                    int count = 0;

                                    foreach (var entity in targets)
                                    {
                                        if (entity is PlayerControllerB)
                                            continue;

                                        try
                                        {
                                            DestroyEntity(entity);
                                            count++;
                                        }
                                        catch (System.Exception e)
                                        {
                                            Debug.LogError(e);
                                        }
                                    }

                                    x.Source.SendCommandResult(string.Format(resultText, targets.GetEntityName(count)));

                                    return count;
                                }

                                return 0;
                            })
                    )
            );
        }

        /// <summary>
        /// 선택한 엔티티를 파괴합니다 (플레이어는 파괴할 수 없습니다)
        /// </summary>
        /// <param name="entity">파괴할 <see cref="NetworkObject"/> 스크립트가 붙어 있는 <see cref="NetworkBehaviour"/> 오브젝트</param>
        public static void DestroyEntity(NetworkBehaviour entity)
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
                return;

            if (entity is not PlayerControllerB && entity.TryGetComponent(out NetworkObject networkObject))
                networkObject.Despawn();
        }
    }
}
