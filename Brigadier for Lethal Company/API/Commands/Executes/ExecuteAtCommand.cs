using Brigadier.NET.Builder;
using System.Linq;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Commands.Executes
{
    public sealed class ExecuteAtCommand : ServerCommand
    {
        ExecuteAtCommand() { }

        public override void Register()
        {
            //execute as -> execute
            dispatcher.Register(x =>
                x.Literal("execute")
                    .Requires(x => x.isOp)
                    .Then(x =>
                        x.Literal("at")
                            .Then(x =>
                                x.Argument("targets", LethalArguments.Selector(false, true))
                                    .Redirect(ExecuteRunCommand.root, x =>
                                    {
                                        var targets = LethalArguments.GetSelector(x, "targets").GetEntitys(x.Source);
                                        if (targets.Any())
                                        {
                                            var target = targets.First();

                                            x.Source.SetPosition(target.transform.position);
                                            x.Source.SetRotation(target.transform.eulerAngles);
                                        }

                                        return x.Source;
                                    })
                            )
                    )
            );
        }
    }
}
