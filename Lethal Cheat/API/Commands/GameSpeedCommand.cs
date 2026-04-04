using Brigadier.NET;
using Brigadier.NET.Builder;
using Rumi.BrigadierForLethalCompany.API;
using Unity.Netcode;
using UnityEngine;

namespace Rumi.LethalCheat.API.Commands
{
    public sealed class GameSpeedCommand : ServerCommand
    {
        public const string resultGetText = "The game speed is {value}";
        public const string resultSetText = "Set the game speed to {value}";

        GameSpeedCommand() { }

        public override void Register()
        {
            //gamespeed get
            //gamespeed set <float:time>
            dispatcher.Register(x =>
                x.Literal("gamespeed")
                    .Then(x =>
                        x.Literal("get")
                            .Executes(x =>
                            {
                                float timescale = GetGameSpeed();
                                x.Source.SendCommandResult(resultGetText.Replace("{value}", timescale.ToString()), false);

                                return Mathf.RoundToInt(GetGameSpeed() * 100);
                            })
                    )
                    .Then(x =>
                        x.Literal("set")
                            .Then(x =>
                                x.Argument("timescale", Arguments.Float(0, 100))
                                    .Executes(x =>
                                    {
                                        float timescale = Arguments.GetFloat(x, "timescale");
                                        SetGameSpeed(timescale);

                                        x.Source.SendCommandResult(resultSetText.Replace("{value}", timescale.ToString()));
                                        return Mathf.RoundToInt(timescale * 100);
                                    })
                            )
                    )
            );
        }

        public static float GetGameSpeed()
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
                return 0;

            return Time.timeScale;
        }

        public static void SetGameSpeed(float timeScale)
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
                return;

            InternalSetGameSpeedClientRpc(timeScale);
        }

        [ClientRpc]
        static void InternalSetGameSpeedClientRpc(float timeScale) => Time.timeScale = timeScale;
    }
}
