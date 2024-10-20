using Brigadier.NET.Builder;
using GameNetcodeStuff;
using Rumi.BrigadierForLethalCompany.API.Arguments;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Commands.Executes
{
    public sealed class ExecuteAs : ServerCommand
    {
        ExecuteAs() { }

        public override void Register()
        {
            //execute as -> execute
            dispatcher.Register(static x =>
                x.Literal("execute")
                    .Then(static x =>
                        x.Literal("as")
                            .Then(static x =>
                                x.Argument("targets", RuniArguments.Selector())
                                    .Fork(Execute.root, x =>
                                    {
                                        var targets = RuniArguments.GetSelector(x, "targets").GetEntitys(x.Source);
                                        return targets.Select(y => new ServerCommandSource(y, x.Source.position, x.Source.rotation)).ToArray();
                                    })
                            )
                    )
            );
        }
    }
}
