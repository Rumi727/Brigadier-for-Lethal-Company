using Brigadier.NET.Builder;
using GameNetcodeStuff;
using Rumi.BrigadierForLethalCompany.API;
using Rumi.BrigadierForLethalCompany.API.Arguments;
using Rumi.LethalCheat.Networking;

namespace Rumi.LethalCheat.API.Commands
{
    public sealed class Damage : ServerCommand
    {
        Damage() { }

        public override void Register()
        {
            //damage <int:amount>
            //damage <Entity:destination> <int:amount>
            dispatcher.Register(x =>
                x.Literal("damage")
                    .Then(x =>
                        x.Argument("amount", RuniArguments.Integer())
                            .Executes(x =>
                            {
                                if (x.Source.sender != null)
                                {
                                    int amount = RuniArguments.GetInteger(x, "amount");
                                    LCheatNetworkHandler.DamageEntity(x.Source.sender, amount);

                                    return 1;
                                }
                                else
                                    return 0;
                            })
                    )
                    .Then(x =>
                        x.Argument("targets", RuniArguments.Selector())
                            .Then(x =>
                                x.Argument("amount", RuniArguments.Integer())
                                    .Executes(x =>
                                    {
                                        var targets = RuniArguments.GetSelector(x, "targets").GetEntitys(x.Source);
                                        int amount = RuniArguments.GetInteger(x, "amount");
                                        int count = 0;

                                        foreach (var entity in targets)
                                        {
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

                                        return count;
                                    })
                            )
                    )
            );
        }
    }
}
