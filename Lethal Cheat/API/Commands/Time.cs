using Brigadier.NET.Builder;
using Rumi.BrigadierForLethalCompany.API;
using Rumi.BrigadierForLethalCompany.API.Arguments;
using Rumi.LethalCheat.Networking;
using System.Linq;
using UnityEngine;

namespace Rumi.LethalCheat.API.Commands
{
    public sealed class Time : ServerCommand
    {
        Time() { }

        public override void Register()
        {
            //time set <float:time>
            //time speed <float:speed>
            dispatcher.Register(x =>
                x.Literal("time")
                    .Then(x =>
                        x.Literal("set")
                            .Then(x =>
                                x.Argument("time", RuniArguments.Float(6, 24))
                                    .Executes(x =>
                                    {
                                        float time = RuniArguments.GetFloat(x, "time");
                                        LCheatNetworkHandler.SetTimeHour(time);

                                        return Mathf.FloorToInt(time);
                                    })
                            )
                                   
                    ).Then(x =>
                        x.Literal("speed")
                            .Then(x =>
                                x.Argument("speed", RuniArguments.Float())
                                    .Executes(x =>
                                    {
                                        float speed = RuniArguments.GetFloat(x, "speed");
                                        LCheatNetworkHandler.SetTimeSpeed(speed);

                                        return Mathf.FloorToInt(speed);
                                    })
                            )

                    )
            );
        }
    }
}
