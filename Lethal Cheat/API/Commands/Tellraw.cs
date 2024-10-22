using Brigadier.NET.Builder;
using GameNetcodeStuff;
using Rumi.BrigadierForLethalCompany.API;
using Rumi.BrigadierForLethalCompany.API.Arguments;
using Rumi.BrigadierForLethalCompany.Networking;

namespace Rumi.LethalCheat.API.Commands
{
    public sealed class Tellraw : ServerCommand
    {
        Tellraw() { }

        public override void Register()
        {
            //tellraw <Entity:targets> <string:message>
            dispatcher.Register(static x =>
                x.Literal("tellraw")
                    .Then(x =>
                        x.Argument("targets", RuniArguments.Selector(true))
                            .Then(static x =>
                                x.Argument("message", RuniArguments.GreedyString())
                                    .Executes(static x =>
                                    {
                                        var targets = RuniArguments.GetSelector(x, "targets").GetEntitys(x.Source);
                                        string message = RuniArguments.GetString(x, "message");

                                        int count = 0;
                                        foreach (var entity in targets)
                                        {
                                            if (entity is not PlayerControllerB player)
                                                continue;

                                            BFLCNetworkHandler.AddChat(message, player);
                                        }

                                        return count;
                                    })
                            )
                    )
            );
        }
    }
}
