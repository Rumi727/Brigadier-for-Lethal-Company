using Brigadier.NET.Builder;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Commands.Executes
{
    public sealed class ExecuteLog : ServerCommand
    {
        ExecuteLog() { }

        public override void Register()
        {
            //execute log -> execute
            dispatcher.Register(static x =>
                x.Literal("execute")
                    .Then(static x =>
                        x.Literal("log")
                            .Redirect(Execute.root, static x =>
                            {
                                Debug.Log(x.Source);
                                return x.Source;
                            })
                            .Executes(x =>
                            {
                                Debug.Log(x.Source);
                                return 1;
                            })
                    )
            );
        }
    }
}
