using Brigadier.NET.Builder;
using GameNetcodeStuff;
using Rumi.BrigadierForLethalCompany;
using Rumi.BrigadierForLethalCompany.API;
using Rumi.BrigadierForLethalCompany.API.Arguments;
using Rumi.LethalCheat.Networking;

namespace Rumi.LethalCheat.API.Commands
{
    public sealed class Destroy : ServerCommand
    {
        public const string resultText = "Destroyed {targets}";

        Destroy() { }

        public override void Register()
        {
            //destroy
            //destroy <Entity:destination>
            dispatcher.Register(static x =>
                x.Literal("destroy")
                    .Executes(static x =>
                    {
                        if (x.Source.sender != null && x.Source.player == null)
                        {
                            LCheatNetworkHandler.DestroyEntity(x.Source.sender);
                            x.Source.SendCommandResult(resultText.Replace("{targets}", x.Source.sender.GetEntityName()));

                            return 1;
                        }
                        else
                            return 0;
                    })
                    .Then(static x =>
                        x.Argument("targets", RuniArguments.Selector())
                            .Executes(static x =>
                            {
                                if (x.Source.sender != null)
                                {
                                    var targets = RuniArguments.GetSelector(x, "targets").GetEntitys(x.Source);
                                    int count = 0;

                                    foreach (var entity in targets)
                                    {
                                        if (entity is PlayerControllerB)
                                            continue;

                                        try
                                        {
                                            LCheatNetworkHandler.DestroyEntity(entity);
                                            count++;
                                        }
                                        catch (System.Exception e)
                                        {
                                            Debug.LogError(e);
                                        }
                                    }

                                    x.Source.SendCommandResult(resultText.Replace("{targets}", targets.GetEntityName(count)));

                                    return count;
                                }

                                return 0;
                            })
                    )
            );
        }
    }
}
