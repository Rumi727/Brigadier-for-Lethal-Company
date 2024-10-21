using Brigadier.NET.Builder;
using Rumi.BrigadierForLethalCompany;
using Rumi.BrigadierForLethalCompany.API;
using Rumi.BrigadierForLethalCompany.API.Arguments;
using Rumi.LethalCheat.Networking;
using System.Linq;
using UnityEngine;

namespace Rumi.LethalCheat.API.Commands
{
    public sealed class Teleport : ServerCommand
    {
        public const string resultEntityText = "Teleported {targets} to {destination}";
        public const string resultLocationText = "Teleported {targets} to {x}, {y}, {z}";

        Teleport() { }

        public override void Register()
        {
            //teleport <Vector3:location>
            //teleport <Entity:destination>
            //teleport <Entity[]:targets> <Vector3:location>
            //teleport <Entity[]:targets> <Entity:destination>
            var node =
            dispatcher.Register(static x =>
                x.Literal("teleport")
                    .Then(static x =>
                        x.Argument("location", RuniArguments.Vector3())
                            .Executes(static x =>
                            {
                                if (x.Source.sender != null)
                                {
                                    Vector3 position = RuniArguments.GetVector3(x, "location");
                                    LCheatNetworkHandler.TeleportEntity(x.Source.sender, position);

                                    x.Source.SendCommandResult(resultLocationText.Replace("{targets}", x.Source.sender.GetEntityName()).Replace("{x}", position.x.ToString()).Replace("{y}", position.y.ToString()).Replace("{z}", position.z.ToString()));

                                    return 1;
                                }

                                return 0;
                            })
                    )
                    .Then(static x =>
                        x.Argument("destination", RuniArguments.Selector(false, true))
                            .Executes(static x =>
                            {
                                if (x.Source.sender != null)
                                {
                                    var targets = RuniArguments.GetSelector(x, "destination").GetEntitys(x.Source);
                                    if (targets.CountIsOne())
                                    {
                                        LCheatNetworkHandler.TeleportEntity(x.Source.sender, targets.First().transform.position);
                                        x.Source.SendCommandResult(resultEntityText.Replace("{targets}", x.Source.sender.GetEntityName()).Replace("{destination}", targets.GetEntityName()));

                                        return 1;
                                    }
                                }

                                return 0;
                            })
                    )
                    .Then(static x =>
                        x.Argument("targets", RuniArguments.Selector(false))
                            .Then(static x =>
                                x.Argument("location", RuniArguments.Vector3())
                                    .Executes(static x =>
                                    {
                                        var targets = RuniArguments.GetSelector(x, "targets").GetEntitys(x.Source);
                                        Vector3 position = RuniArguments.GetVector3(x, "location");

                                        int count = 0;
                                        foreach (var target in targets)
                                        {
                                            LCheatNetworkHandler.TeleportEntity(target, position);
                                            count++;
                                        }

                                        x.Source.SendCommandResult(resultLocationText.Replace("{targets}", targets.GetEntityName(count)).Replace("{x}", position.x.ToString()).Replace("{y}", position.y.ToString()).Replace("{z}", position.z.ToString()));

                                        return count;
                                    })
                            )
                            .Then(static x =>
                                x.Argument("destination", RuniArguments.Selector(false, true))
                                    .Executes(static x =>
                                    {
                                        var targets = RuniArguments.GetSelector(x, "targets").GetEntitys(x.Source);
                                        var destination = RuniArguments.GetSelector(x, "destination").GetEntitys(x.Source);

                                        if (destination.CountIsOne())
                                        {
                                            var location = destination.First().transform.position;

                                            int count = 0;
                                            foreach (var target in targets)
                                            {
                                                LCheatNetworkHandler.TeleportEntity(target, location);
                                                count++;
                                            }

                                            x.Source.SendCommandResult(resultEntityText.Replace("{targets}", targets.GetEntityName(count)).Replace("{destination}", destination.GetEntityName()));

                                            return count;
                                        }

                                        return 0;
                                    })
                            )
                    )
            );

            //tp -> teleport
            dispatcher.Register(x =>
                x.Literal("tp")
                    .Redirect(node)
            );
        }
    }
}
