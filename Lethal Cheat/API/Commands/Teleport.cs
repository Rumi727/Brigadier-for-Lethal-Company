using Brigadier.NET.Builder;
using Rumi.BrigadierForLethalCompany.API;
using Rumi.BrigadierForLethalCompany.API.Arguments;
using Rumi.LethalCheat.Networking;
using System.Linq;
using UnityEngine;

namespace Rumi.LethalCheat.API.Commands
{
    public sealed class Teleport : ServerCommand
    {
        Teleport() { }

        public override void Register()
        {
            //teleport <Vector3:location>
            //teleport <Entity:destination>
            //teleport <Entity[]:targets> <Vector3:location>
            //teleport <Entity[]:targets> <Entity:destination>
            var node = dispatcher.Register(x =>
                           x.Literal("teleport")
                               .Then(x =>
                                   x.Argument("location", RuniArguments.Vector3())
                                       .Executes(x =>
                                       {
                                           if (x.Source.sender != null)
                                           {
                                               Vector3 position = RuniArguments.GetVector3(x, "location");
                                               LCheatNetworkHandler.TeleportEntity(x.Source.sender, position);

                                               return 1;
                                           }

                                           return 0;
                                       })
                               )
                               .Then(x =>
                                   x.Argument("destination", RuniArguments.Selector(false, true))
                                       .Executes(x =>
                                       {
                                           if (x.Source.sender != null)
                                           {
                                               var targets = RuniArguments.GetSelector(x, "destination").GetEntitys(x.Source);
                                               if (targets.CountIsOne())
                                               {
                                                   LCheatNetworkHandler.TeleportEntity(x.Source.sender, targets.First().transform.position);
                                                   return 1;
                                               }
                                           }

                                           return 0;
                                       })
                               )
                               .Then(x =>
                                   x.Argument("targets", RuniArguments.Selector(false))
                                       .Then(x =>
                                           x.Argument("location", RuniArguments.Vector3())
                                               .Executes(x =>
                                               {
                                                   var targets = RuniArguments.GetSelector(x, "targets").GetEntitys(x.Source);
                                                   Vector3 position = RuniArguments.GetVector3(x, "location");

                                                   foreach (var target in targets)
                                                       LCheatNetworkHandler.TeleportEntity(target, position);

                                                   return 0;
                                               })
                                       )
                                       .Then(x =>
                                           x.Argument("destination", RuniArguments.Selector(false, true))
                                               .Executes(x =>
                                               {
                                                   var targets = RuniArguments.GetSelector(x, "targets").GetEntitys(x.Source);
                                                   var destination = RuniArguments.GetSelector(x, "destination").GetEntitys(x.Source);

                                                   if (destination.CountIsOne())
                                                   {
                                                       var location = destination.First().transform.position;
                                                       foreach (var target in targets)
                                                           LCheatNetworkHandler.TeleportEntity(target, location);

                                                       return 1;
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
