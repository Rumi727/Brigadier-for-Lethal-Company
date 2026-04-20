using Brigadier.NET;
using Brigadier.NET.Builder;
using System.Linq;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Commands.Executes
{
    public sealed class ExecuteLoopCommand : ServerCommand
    {
        ExecuteLoopCommand() { }

        public override void Register()
        {
            //execute loop <int:count> -> execute
            dispatcher.Register(x =>
                x.Literal("execute")
                    .Requires(x => x.isOp)
                    .Then(x =>
                        x.Literal("loop")
                            .Then(x =>
                                x.Argument("count", Arguments.Integer())
                                    .Fork(ExecuteRunCommand.root, x => [.. Enumerable.Repeat(x.Source.Clone(), Arguments.GetInteger(x, "count"))])
                            )
                    )
            );
        }
    }
}
