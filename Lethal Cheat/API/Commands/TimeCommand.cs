using Brigadier.NET;
using Brigadier.NET.Builder;
using Rumi.BrigadierForLethalCompany.API;
using Unity.Netcode;
using UnityEngine;

namespace Rumi.LethalCheat.API.Commands
{
    public sealed class TimeCommand : ServerCommand
    {
        public const string resultGetText = "The time is {h}:{m} ({value})";
        public const string resultSetText = "Set the time to {h}:{m} ({value})";

        public const string resultSpeedGetText = "The time speed is {value}";
        public const string resultSpeedSetText = "Set the time speed to {value}";

        TimeCommand() { }

        public override void Register()
        {
            //time get
            //time set <float:time>
            //time speed <float:speed>
            dispatcher.Register(x =>
                x.Literal("time")
                    .Then(x =>
                        x.Literal("get")
                            .Executes(x =>
                            {
                                float time = GetTimeHour();

                                int hour = Mathf.FloorToInt(time);
                                int minute = (int)((time - hour) * 60);
                                x.Source.SendCommandResult(resultGetText.Replace("{h}", hour.ToString()).Replace("{m}", minute.ToString("00")).Replace("{value}", time.ToString()), false);

                                return hour;
                            })
                    )
                    .Then(x =>
                        x.Literal("set")
                            .Then(x =>
                                x.Argument("time", Arguments.Float(6, 24))
                                    .Executes(x =>
                                    {
                                        float time = Arguments.GetFloat(x, "time");
                                        SetTimeHour(time);

                                        int hour = Mathf.FloorToInt(time);
                                        int minute = (int)((time - hour) * 60);
                                        x.Source.SendCommandResult(resultSetText.Replace("{h}", hour.ToString()).Replace("{m}", minute.ToString("00")).Replace("{value}", time.ToString()), false);

                                        return hour;
                                    })
                            )

                    ).Then(x =>
                        x.Literal("speed")
                            .Executes(x =>
                            {
                                float speed = GetTimeSpeed();
                                x.Source.SendCommandResult(resultSpeedGetText.Replace("{value}", speed.ToString()), false);

                                return Mathf.FloorToInt(speed);
                            })
                            .Then(x =>
                                x.Argument("speed", Arguments.Float())
                                    .Executes(x =>
                                    {
                                        float speed = Arguments.GetFloat(x, "speed");
                                        SetTimeSpeed(speed);

                                        x.Source.SendCommandResult(resultSpeedSetText.Replace("{value}", speed.ToString()));

                                        return Mathf.FloorToInt(speed);
                                    })
                            )
                    )
            );
        }

        public static float GetTimeHour()
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
                return 0;

            var timeInstance = TimeOfDay.Instance;
            var level = timeInstance.currentLevel;

            return (timeInstance.globalTime * level.DaySpeedMultiplier / timeInstance.lengthOfHours) + 6;
        }

        public static void SetTimeHour(float time)
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
                return;

            InternalSetTimeHourClientRpc(time);
        }

        [ClientRpc]
        static void InternalSetTimeHourClientRpc(float time)
        {
            var timeInstance = TimeOfDay.Instance;
            var level = timeInstance.currentLevel;

            float globalTime = (time - 6) / level.DaySpeedMultiplier * timeInstance.lengthOfHours;

            timeInstance.globalTime = globalTime;
            timeInstance.currentDayTime = timeInstance.CalculatePlanetTime(level);
            timeInstance.hour = (int)(timeInstance.currentDayTime / timeInstance.lengthOfHours);
            timeInstance.previousHour = timeInstance.hour;
            timeInstance.globalTimeAtEndOfDay = globalTime + ((timeInstance.totalTime - timeInstance.currentDayTime) / level.DaySpeedMultiplier);
            timeInstance.normalizedTimeOfDay = timeInstance.currentDayTime / timeInstance.totalTime;

            timeInstance.RefreshClockUI();
        }

        public static float GetTimeSpeed() => TimeOfDay.Instance.globalTimeSpeedMultiplier;

        public static void SetTimeSpeed(float speed)
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
                return;

            InternalSetTimeSpeedClientRpc(speed);
        }

        [ClientRpc]
        static void InternalSetTimeSpeedClientRpc(float speed) => TimeOfDay.Instance.globalTimeSpeedMultiplier = speed;
    }
}
