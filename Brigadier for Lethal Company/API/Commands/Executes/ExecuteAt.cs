using Brigadier.NET.Builder;
using Rumi.BrigadierForLethalCompany.API.Arguments;
using System.Linq;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Commands.Executes
{
    public sealed class ExecuteAt : ServerCommand
    {
        ExecuteAt() { }

        public override void Register()
        {
            //execute as -> execute
            dispatcher.Register(static x =>
                x.Literal("execute")
                    .Then(static x =>
                        x.Literal("at")
                            .Then(static x =>
                                x.Argument("targets", RuniArguments.Selector(false, true))
                                    .Redirect(Execute.root, static x =>
                                    {
                                        var targets = RuniArguments.GetSelector(x, "targets").GetEntitys(x.Source);
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
