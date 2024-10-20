using Brigadier.NET.Builder;
using Rumi.BrigadierForLethalCompany.API;
using Rumi.BrigadierForLethalCompany.API.Arguments;
using Rumi.LethalCheat.Networking;

namespace Rumi.LethalCheat.API.Commands
{
    public sealed class Destroy : ServerCommand
    {
        Destroy() { }

        public override void Register()
        {
            //destroy
            //destroy <Entity:destination>
            dispatcher.Register(x =>
                x.Literal("destroy")
                    .Executes(x =>
                    {
                        if (x.Source.sender != null)
                        {
                            LCheatNetworkHandler.DestroyEntity(x.Source.sender);
                            return 1;
                        }
                        else
                            return 0;
                    })
                    .Then(x =>
                        x.Argument("targets", RuniArguments.Selector())
                            .Executes(x =>
                            {
                                if (x.Source.sender != null)
                                {
                                    var targets = RuniArguments.GetSelector(x, "targets").GetEntitys(x.Source);
                                    int count = 0;

                                    foreach (var entity in targets)
                                    {
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

                                    return count;
                                }

                                return 0;
                            })
                    )
            );
        }
    }
}
