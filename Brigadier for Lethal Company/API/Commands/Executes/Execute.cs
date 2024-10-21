using Brigadier.NET.Builder;
using Brigadier.NET.Tree;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Commands.Executes
{
    public static class Execute
    {
        public static LiteralCommandNode<ServerCommandSource> root
        {
            get
            {
                //execute run ...
                return _execute ??= ServerCommand.dispatcher.Register(static x =>
                    x.Literal("execute")
                        .Then(static x =>
                            x.Literal("run")
                                .Redirect(ServerCommand.dispatcher.GetRoot())
                        )
                );
            }
        }
        static LiteralCommandNode<ServerCommandSource>? _execute;
    }
}
