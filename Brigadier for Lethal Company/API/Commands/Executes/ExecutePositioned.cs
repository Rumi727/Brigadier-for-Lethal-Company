using Brigadier.NET.Builder;
using Rumi.BrigadierForLethalCompany.API.Arguments;
using System.Linq;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Commands.Executes
{
    public sealed class ExecutePositioned : ServerCommand
    {
        ExecutePositioned() { }

        public override void Register()
        {
            //execute positioned <Vector3:pos> -> execute
            dispatcher.Register(static x =>
                x.Literal("execute")
                    .Then(static x =>
                        x.Literal("positioned")
                            .Then(static x =>
                                x.Argument("pos", RuniArguments.Vector3())
                                    .Redirect(Execute.root, static x => x.Source.SetPosition(RuniArguments.GetVector3(x, "pos")))
                                    .Then(static x =>
                                        x.Literal("as")
                                            .Then(static x =>
                                                x.Argument("targets", RuniArguments.Selector(false, true))
                                                    .Redirect(Execute.root, static x =>
                                                    {
                                                        var targets = RuniArguments.GetSelector(x, "targets").GetEntitys(x.Source);
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
                    )
            );
        }
    }
}
