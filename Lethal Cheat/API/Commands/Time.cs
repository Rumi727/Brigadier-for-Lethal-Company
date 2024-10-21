using Brigadier.NET.Builder;
using Rumi.BrigadierForLethalCompany.API;
using Rumi.BrigadierForLethalCompany.API.Arguments;
using Rumi.LethalCheat.Networking;
using UnityEngine;

namespace Rumi.LethalCheat.API.Commands
{
    public sealed class Time : ServerCommand
    {
        public const string resultGetText = "The time is {h}:{m} ({value})";
        public const string resultSetText = "Set the time to {h}:{m} ({value})";

        public const string resultSpeedGetText = "The time speed is {value}";
        public const string resultSpeedSetText = "Set the time speed to {value}";

        Time() { }

        public override void Register()
        {
            //time set <float:time>
            //time speed <float:speed>
            dispatcher.Register(static x =>
                x.Literal("time")
                    .Then(x =>
                        x.Literal("get")
                            .Executes(static x =>
                            {
                                float time = LCheatNetworkHandler.GetTimeHour();

                                int hour = Mathf.FloorToInt(time);
                                int minute = (int)((time - hour) * 60);
                                x.Source.SendCommandResult(resultGetText.Replace("{h}", hour.ToString()).Replace("{m}", minute.ToString()).Replace("{value}", time.ToString()));

                                return hour;
                            })
                    )
                    .Then(static x =>
                        x.Literal("set")
                            .Then(static x =>
                                x.Argument("time", RuniArguments.Float(6, 24))
                                    .Executes(static x =>
                                    {
                                        float time = RuniArguments.GetFloat(x, "time");
                                        LCheatNetworkHandler.SetTimeHour(time);

                                        int hour = Mathf.FloorToInt(time);
                                        int minute = (int)((time - hour) * 60);
                                        x.Source.SendCommandResult(resultSetText.Replace("{h}", hour.ToString()).Replace("{m}", minute.ToString()).Replace("{value}", time.ToString()));

                                        return hour;
                                    })
                            )
                                   
                    ).Then(static x =>
                        x.Literal("speed")
                            .Executes(static x =>
                            {
                                float speed = LCheatNetworkHandler.GetTimeSpeed();
                                x.Source.SendCommandResult(resultSpeedGetText.Replace("{value}", speed.ToString()));

                                return Mathf.FloorToInt(speed);
                            })
                            .Then(static x =>
                                x.Argument("speed", RuniArguments.Float())
                                    .Executes(static x =>
                                    {
                                        float speed = RuniArguments.GetFloat(x, "speed");
                                        LCheatNetworkHandler.SetTimeSpeed(speed);

                                        x.Source.SendCommandResult(resultSpeedSetText.Replace("{value}", speed.ToString()));

                                        return Mathf.FloorToInt(speed);
                                    })
                            )
                    )
            );
        }
    }
}
