using Brigadier.NET.Builder;
using Rumi.BrigadierForLethalCompany.API.Arguments;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Commands.Executes
{
    public sealed class ExecuteRotated : ServerCommand
    {
        ExecuteRotated() { }

        public override void Register()
        {
            //execute rotated <Vector3:rot> -> execute
            dispatcher.Register(static x =>
                x.Literal("execute")
                    .Then(static x =>
                        x.Literal("rotated")
                            .Then(static x =>
                                x.Argument("rot", RuniArguments.Vector3())
                                    .Redirect(Execute.root, static x => x.Source.SetRotation(RuniArguments.GetVector3(x, "rot")))
                            )
                    )
            );
        }
    }
}
