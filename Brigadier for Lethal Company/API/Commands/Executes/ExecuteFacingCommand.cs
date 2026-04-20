using Brigadier.NET.Builder;
using UnityEngine;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Commands.Executes
{
    public sealed class ExecuteFacingCommand : ServerCommand
    {
        ExecuteFacingCommand() { }

        public override void Register()
        {
            //execute facing <Vector3:pos> -> execute
            dispatcher.Register(x =>
                x.Literal("execute")
                    .Requires(x => x.isOp)
                    .Then(x =>
                        x.Literal("facing")
                            .Then(x =>
                                x.Argument("pos", LethalArguments.Vector3())
                                    .Redirect(ExecuteRunCommand.root, x =>
                                    {
                                        Vector3 pos = LethalArguments.GetVector3(x, "pos");
                                        Vector3 rotation = Quaternion.LookRotation(pos).eulerAngles;

                                        return x.Source.SetRotation(rotation);
                                    })
                            )
                    )
            );
        }
    }
}
