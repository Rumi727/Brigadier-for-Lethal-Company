using Brigadier.NET.Builder;
using System.Linq;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Commands.Executes
{
    public sealed class ExecuteRotatedCommand : ServerCommand
    {
        ExecuteRotatedCommand() { }

        public override void Register()
        {
            //execute rotated <Vector3:rot> -> execute
            dispatcher.Register(x =>
                x.Literal("execute")
                    .Then(x =>
                        x.Literal("rotated")
                            .Then(x =>
                                x.Argument("rot", LethalArguments.Vector3())
                                    .Redirect(ExecuteRunCommand.root, x => x.Source.SetRotation(LethalArguments.GetVector3(x, "rot")))
                            )
                            .Then(x =>
                                x.Literal("as")
                                            .Then(x =>
                                                x.Argument("targets", LethalArguments.Selector(false, true))
                                                    .Redirect(ExecuteRunCommand.root, x =>
                                                    {
                                                        var targets = LethalArguments.GetSelector(x, "targets").GetEntitys(x.Source);
                                                        if (targets.CountIsOne())
                                                        {
                                                            var target = targets.First();
                                                            return x.Source.SetRotation(target.transform.eulerAngles);
                                                        }

                                                        return x.Source;
                                                    })
                                            )
                            )
                    )
            );
        }
    }
}
