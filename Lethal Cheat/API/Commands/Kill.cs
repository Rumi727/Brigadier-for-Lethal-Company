using Brigadier.NET.Builder;
using Rumi.BrigadierForLethalCompany.API;
using Rumi.BrigadierForLethalCompany.API.Arguments;
using Rumi.LethalCheat.Networking;

namespace Rumi.LethalCheat.API.Commands
{
    public sealed class Kill : ServerCommand
    {
        Kill() { }

        public override void Register()
        {
            //kill
            //kill <Entity:targets>
            dispatcher.Register(x =>
                x.Literal("kill")
                    .Executes(x =>
                    {
                        if (x.Source.sender != null)
                        {
                            LCheatNetworkHandler.KillEntity(x.Source.sender);
                            return 1;
                        }
                        else
                            return 0;
                    })
                    .Then(x =>
                        x.Argument("targets", RuniArguments.Selector())
                            .Executes(x =>
                            {
                                var targets = RuniArguments.GetSelector(x, "targets").GetEntitys(x.Source);
                                int count = 0;

                                foreach (var entity in targets)
                                {
                                    try
                                    {
                                        LCheatNetworkHandler.KillEntity(entity);
                                        count++;
                                    }
                                    catch (System.Exception e)
                                    {
                                        Debug.LogError(e);
                                    }
                                }

                                return count;
                            })
                    )
            );
        }
    }
}
