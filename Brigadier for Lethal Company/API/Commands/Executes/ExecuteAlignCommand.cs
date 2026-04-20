using Brigadier.NET.Builder;
using Rumi.BrigadierForLethalCompany.API.ArgumentTypes;
using UnityEngine;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Commands.Executes
{
    public sealed class ExecuteAlignCommand : ServerCommand
    {
        ExecuteAlignCommand() { }

        public override void Register()
        {
            //execute align <PosSwizzleEnum:axes> -> execute
            dispatcher.Register(x =>
                x.Literal("execute")
                    .Requires(x => x.isOp)
                    .Then(x =>
                        x.Literal("align")
                            .Then(x =>
                                x.Argument("axes", LethalArguments.PosSwizzle())
                                    .Redirect(ExecuteRunCommand.root, x =>
                                    {
                                        PosSwizzleEnum posSwizzle = LethalArguments.GetPosSwizzle(x, "axes");

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
