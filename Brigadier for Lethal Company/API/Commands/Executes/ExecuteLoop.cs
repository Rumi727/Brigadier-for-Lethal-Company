using Brigadier.NET.Builder;
using Rumi.BrigadierForLethalCompany.API.Arguments;
using System.Linq;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Commands.Executes
{
    public sealed class ExecuteLoop : ServerCommand
    {
        ExecuteLoop() { }

        public override void Register()
        {
            //execute loop <int:count> -> execute
            dispatcher.Register(static x =>
                x.Literal("execute")
                    .Then(static x =>
                        x.Literal("loop")
                            .Then(static x =>
                                x.Argument("count", RuniArguments.Integer())
                                    .Fork(Execute.root, static x => Enumerable.Repeat(x.Source.Clone(), RuniArguments.GetInteger(x, "count")).ToArray())
                            )
                    )
            );
        }
    }
}
