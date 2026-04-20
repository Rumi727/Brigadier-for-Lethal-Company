using Brigadier.NET.Builder;
using System.Linq;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Commands.Executes
{
    public sealed class ExecutePositionedCommand : ServerCommand
    {
        ExecutePositionedCommand() { }

        public override void Register()
        {
            //execute positioned <Vector3:pos> -> execute
            dispatcher.Register(x =>
                x.Literal("execute")
                    .Requires(x => x.isOp)
                    .Then(x =>
                        x.Literal("positioned")
                            .Then(x =>
                                x.Argument("pos", LethalArguments.Vector3())
                                    .Redirect(ExecuteRunCommand.root, x => x.Source.SetPosition(LethalArguments.GetVector3(x, "pos")))
                            ).Then(x =>
                                x.Literal("as")
                                    .Then(x =>
                                        x.Argument("targets", LethalArguments.Selector(false, true))
                                            .Redirect(ExecuteRunCommand.root, x =>
                                            {
                                                var targets = LethalArguments.GetSelector(x, "targets").GetEntitys(x.Source);
                                                if (targets.CountIsOne())
                                                {
                                                    var target = targets.First();
                                                    return x.Source.SetPosition(target.transform.position);
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
