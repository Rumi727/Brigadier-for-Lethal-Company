using Brigadier.NET.Builder;
using System.Linq;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Commands.Executes
{
    public sealed class ExecuteAsCommand : ServerCommand
    {
        ExecuteAsCommand() { }

        public override void Register()
        {
            //execute as -> execute
            dispatcher.Register(x =>
                x.Literal("execute")
                    .Then(x =>
                        x.Literal("as")
                            .Then(x =>
                                x.Argument("targets", LethalArguments.Selector())
                                    .Fork(ExecuteRunCommand.root, x =>
                                    {
                                        var targets = LethalArguments.GetSelector(x, "targets").GetEntitys(x.Source);
                                        return [.. targets.Select(y => x.Source.Clone().SetSender(y))];
                                    })
                            )
                    )
            );
        }
    }
}
