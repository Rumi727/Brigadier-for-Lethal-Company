using Brigadier.NET.Builder;
using Rumi.BrigadierForLethalCompany.API;
using Rumi.BrigadierForLethalCompany.API.Arguments;
using Rumi.LethalCheat.Networking;

namespace Rumi.LethalCheat.API.Commands
{
    public sealed class Credit : ServerCommand
    {
        public const string resultGetText = "The credit is {value}";
        public const string resultSetText = "Set the credit to {value}";

        Credit() { }

        public override void Register()
        {
            //credit <int:credit>
            dispatcher.Register(static x =>
                x.Literal("credit")
                    .Then(static x =>
                        x.Literal("get")
                            .Executes(static x =>
                            {
                                int credit = LCheatNetworkHandler.GetCredit();
                                x.Source.SendCommandResult(resultGetText.Replace("{value}", credit.ToString()), false);

                                return credit;
                            })
                    )
                    .Then(static x =>
                        x.Literal("set")
                            .Then(static x =>
                                x.Argument("credit", RuniArguments.Integer(0))
                                    .Executes(static x =>
                                    {
                                        int credit = RuniArguments.GetInteger(x, "credit");
                                        LCheatNetworkHandler.SetCredit(credit);

                                        x.Source.SendCommandResult(resultSetText.Replace("{value}", credit.ToString()));
                                        return credit;
                                    })
                            )
                    )
            );
        }
    }
}
