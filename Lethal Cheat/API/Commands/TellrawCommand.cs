using Brigadier.NET;
using Brigadier.NET.Builder;
using GameNetcodeStuff;
using Rumi.BrigadierForLethalCompany;
using Rumi.BrigadierForLethalCompany.API;

namespace Rumi.LethalCheat.API.Commands
{
    public sealed class TellrawCommand : ServerCommand
    {
        TellrawCommand() { }

        public override void Register()
        {
            //tellraw <Entity:targets> <string:message>
            dispatcher.Register(x =>
                x.Literal("tellraw")
                    .Then(x =>
                        x.Argument("targets", LethalArguments.Selector(true))
                            .Then(x =>
                                x.Argument("message", Arguments.GreedyString())
                                    .Executes(x =>
                                    {
                                        var targets = LethalArguments.GetSelector(x, "targets").GetEntitys(x.Source);
                                        string message = Arguments.GetString(x, "message");

                                        int count = 0;
                                        foreach (var entity in targets)
                                        {
                                            if (entity is not PlayerControllerB player)
                                                continue;

                                            BFLCUtility.SendChat(message, player);
                                        }

                                        return count;
                                    })
                            )
                    )
            );
        }
    }
}
