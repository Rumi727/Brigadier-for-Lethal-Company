using Brigadier.NET.Builder;
using Rumi.BrigadierForLethalCompany.API;
using Rumi.BrigadierForLethalCompany.API.Arguments;
using Rumi.LethalCheat.Networking;

namespace Rumi.LethalCheat.API.Commands
{
    public sealed class Credit : ServerCommand
    {
        Credit() { }

        public override void Register()
        {
            //credit <int:credit>
            dispatcher.Register(x =>
                x.Literal("credit")
                    .Then(x =>
                        x.Argument("credit", RuniArguments.Integer(0))
                            .Executes(x =>
                            {
                                int credit = RuniArguments.GetInteger(x, "credit");
                                LCheatNetworkHandler.SetCredit(credit);

                                return credit;
                            })
                    )
            );
        }
    }
}
