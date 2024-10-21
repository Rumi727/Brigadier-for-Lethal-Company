using Brigadier.NET.Builder;
using Rumi.BrigadierForLethalCompany.API;
using Rumi.BrigadierForLethalCompany.API.Arguments;
using Rumi.LethalCheat.Networking;
using Rumi.BrigadierForLethalCompany;
using GameNetcodeStuff;

namespace Rumi.LethalCheat.API.Commands
{
    public sealed class Kill : ServerCommand
    {
        public const string resultText = "Killed {targets}";

        Kill() { }

        public override void Register()
        {
            //kill
            //kill <Entity:targets>
            dispatcher.Register(static x =>
                x.Literal("kill")
                    .Executes(static x =>
                    {
                        if (x.Source.sender != null)
                        {
                            LCheatNetworkHandler.KillEntity(x.Source.sender);
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
                                var targets = RuniArguments.GetSelector(x, "targets").GetEntitys(x.Source);
                                int count = 0;

                                foreach (var entity in targets)
                                {
                                    if (entity is not PlayerControllerB or EnemyAI)
                                        continue;

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

                                x.Source.SendCommandResult(resultText.Replace("{targets}", targets.GetEntityName(count)));

                                return count;
                            })
                    )
            );
        }
    }
}
