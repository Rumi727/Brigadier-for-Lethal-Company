using Brigadier.NET.Builder;
using Rumi.BrigadierForLethalCompany.API;
using Rumi.BrigadierForLethalCompany.API.Arguments;
using Rumi.LethalCheat.Networking;
using UnityEngine;

namespace Rumi.LethalCheat.API.Commands
{
    public sealed class GameSpeed : ServerCommand
    {
        public const string resultGetText = "The game speed is {value}";
        public const string resultSetText = "Set the game speed to {value}";

        GameSpeed() { }

        public override void Register()
        {
            //gamespeed get
            //gamespeed set <float:time>
            dispatcher.Register(static x =>
                x.Literal("gamespeed")
                    .Then(static x =>
                        x.Literal("get")
                            .Executes(static x =>
                            {
                                float timescale = LCheatNetworkHandler.GetGameSpeed();
                                x.Source.SendCommandResult(resultGetText.Replace("{value}", timescale.ToString()), false);

                                return Mathf.RoundToInt(LCheatNetworkHandler.GetGameSpeed() * 100);
                            })
                    )
                    .Then(static x =>
                        x.Literal("set")
                            .Then(static x =>
                                x.Argument("timescale", RuniArguments.Float(0, 100))
                                    .Executes(static x =>
                                    {
                                        float timescale = RuniArguments.GetFloat(x, "timescale");
                                        LCheatNetworkHandler.SetGameSpeed(timescale);

                                        x.Source.SendCommandResult(resultSetText.Replace("{value}", timescale.ToString()));
                                        return Mathf.RoundToInt(timescale * 100);
                                    })
                            )
                    )
            );
        }
    }
}
