using Brigadier.NET.Builder;
using Rumi.BrigadierForLethalCompany.API.Arguments;
using UnityEngine;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Commands.Executes
{
    public sealed class ExecuteFacing : ServerCommand
    {
        ExecuteFacing() { }

        public override void Register()
        {
            //execute facing <Vector3:pos> -> execute
            dispatcher.Register(static x =>
                x.Literal("execute")
                    .Then(static x =>
                        x.Literal("facing")
                            .Then(static x =>
                                x.Argument("pos", RuniArguments.Vector3())
                                    .Redirect(Execute.root, static x =>
                                    {
                                        Vector3 pos = RuniArguments.GetVector3(x, "pos");
                                        Vector3 rotation = Quaternion.LookRotation(pos).eulerAngles;

                                        return x.Source.SetRotation(rotation);
                                    })
                            )
                    )
            );
        }
    }
}
