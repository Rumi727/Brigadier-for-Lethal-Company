using Brigadier.NET.Builder;
using GameNetcodeStuff;
using Rumi.BrigadierForLethalCompany;
using Rumi.BrigadierForLethalCompany.API;
using Rumi.BrigadierForLethalCompany.API.Arguments;
using Rumi.LethalCheat.Networking;

namespace Rumi.LethalCheat.API.Commands
{
    public sealed class Damage : ServerCommand
    {
        public const string resultText = "Applied {value} damage to {targets}";

        Damage() { }

        public override void Register()
        {
            //damage <int:amount>
            //damage <Entity:destination> <int:amount>
            dispatcher.Register(static x =>
                x.Literal("damage")
                    .Then(static x =>
                        x.Argument("amount", RuniArguments.Integer())
                            .Executes(static x =>
                            {
                                if (x.Source.sender != null)
                                {
                                    int amount = RuniArguments.GetInteger(x, "amount");
                                    LCheatNetworkHandler.DamageEntity(x.Source.sender, amount);

                                    x.Source.SendCommandResult(resultText.Replace("{value}", amount.ToString()).Replace("{targets}", x.Source.sender.GetEntityName()));

                                    return 1;
                                }
                                else
                                    return 0;
                            })
                    )
                    .Then(static x =>
                        x.Argument("targets", RuniArguments.Selector())
                            .Then(static x =>
                                x.Argument("amount", RuniArguments.Integer())
                                    .Executes(static x =>
                                    {
                                        var targets = RuniArguments.GetSelector(x, "targets").GetEntitys(x.Source);
                                        int amount = RuniArguments.GetInteger(x, "amount");
                                        int count = 0;

                                        foreach (var entity in targets)
                                        {
                                            if (entity is not (PlayerControllerB or EnemyAI))
                                                continue;

                                            try
                                            {
                                                LCheatNetworkHandler.DamageEntity(entity, amount);
                                                count++;
                                            }
                                            catch (System.Exception e)
                                            {
                                                Debug.LogError(e);
                                            }
                                        }

                                        x.Source.SendCommandResult(resultText.Replace("{value}", amount.ToString()).Replace("{targets}", targets.GetEntityName(count)));

                                        return count;
                                    })
                            )
                    )
            );
        }
    }
}
