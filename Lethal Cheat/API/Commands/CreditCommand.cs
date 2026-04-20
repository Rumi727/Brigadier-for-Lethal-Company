using Brigadier.NET;
using Brigadier.NET.Builder;
using Rumi.BrigadierForLethalCompany.API;
using Unity.Netcode;
using UnityEngine;

namespace Rumi.LethalCheat.API.Commands
{
    public sealed class CreditCommand : ServerCommand
    {
        public const string resultGetText = "The credit is {0}";
        public const string resultAddText = "Add the credit to {0}";
        public const string resultSetText = "Set the credit to {0}";

        CreditCommand() { }

        public override void Register()
        {
            //credit get <int:credit>
            //credit add <int:credit>
            //credit set <int:credit>
            dispatcher.Register(x =>
                x.Literal("credit")
                    .Requires(x => x.isOp)
                    .Then(x =>
                        x.Literal("get")
                            .Executes(x =>
                            {
                                int credit = GetCredit();
                                x.Source.SendCommandResult(string.Format(resultGetText, credit), false);

                                return credit;
                            })
                    )
                    .Then(x =>
                        x.Literal("add")
                            .Then(x =>
                                x.Argument("credit", Arguments.Integer(0))
                                    .Executes(x =>
                                    {
                                        int credit = Arguments.GetInteger(x, "credit");
                                        SetCredit(GetCredit() + credit);

                                        x.Source.SendCommandResult(string.Format(resultAddText, credit));
                                        return credit;
                                    })
                            )
                    )
                    .Then(x =>
                        x.Literal("set")
                            .Then(x =>
                                x.Argument("credit", Arguments.Integer(0))
                                    .Executes(x =>
                                    {
                                        int credit = Arguments.GetInteger(x, "credit");
                                        SetCredit(credit);

                                        x.Source.SendCommandResult(string.Format(resultSetText, credit));
                                        return credit;
                                    })
                            )
                    )
            );
        }

        public static int GetCredit()
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
                return 0;

            return Object.FindAnyObjectByType<Terminal>().groupCredits;
        }

        public static void SetCredit(int credit)
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
                return;

            Terminal terminal = Object.FindAnyObjectByType<Terminal>();

            terminal.useCreditsCooldown = true;
            terminal.groupCredits = credit;
            terminal.SyncGroupCreditsServerRpc(terminal.groupCredits, terminal.numberOfItemsInDropship);
        }
    }
}
