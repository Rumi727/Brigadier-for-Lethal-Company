using Brigadier.NET.Builder;
using Rumi.BrigadierForLethalCompany.API.Arguments;
using UnityEngine;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Commands.Executes
{
    public sealed class ExecuteAlign : ServerCommand
    {
        ExecuteAlign() { }

        public override void Register()
        {
            //execute align <PosSwizzleEnum:axes> -> execute
            dispatcher.Register(static x =>
                x.Literal("execute")
                    .Then(static x =>
                        x.Literal("align")
                            .Then(static x =>
                                x.Argument("axes", RuniArguments.PosSwizzle())
                                    .Redirect(Execute.root, static x =>
                                    {
                                        PosSwizzleEnum posSwizzle = RuniArguments.GetPosSwizzle(x, "axes");

                                        Vector3 position = x.Source.position;
                                        if (posSwizzle.HasFlag(PosSwizzleEnum.x))
                                            position.x = Mathf.Floor(position.x);
                                        if (posSwizzle.HasFlag(PosSwizzleEnum.y))
                                            position.y = Mathf.Floor(position.y);
                                        if (posSwizzle.HasFlag(PosSwizzleEnum.z))
                                            position.z = Mathf.Floor(position.z);

                                        return x.Source.SetPosition(position);
                                    })
                            )
                    )
            );
        }
    }
}
