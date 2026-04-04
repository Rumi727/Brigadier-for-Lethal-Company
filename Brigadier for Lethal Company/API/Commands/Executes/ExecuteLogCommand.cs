using Brigadier.NET.Builder;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Commands.Executes
{
    public sealed class ExecuteLogCommand : ServerCommand
    {
        ExecuteLogCommand() { }

        public override void Register()
        {
            //execute log -> execute
            dispatcher.Register(x =>
                x.Literal("execute")
                    .Then(x =>
                        x.Literal("log")
                            .Redirect(ExecuteRunCommand.root, x =>
                            {
                                Debug.Log(x.Source);
                                x.Source.SendCommandResult(LogCommand.resultText);

                                return x.Source;
                            })
                            .Executes(x =>
                            {
                                Debug.Log(x.Source);
                                x.Source.SendCommandResult(LogCommand.resultText);

                                return 1;
                            })
                    )
            );
        }
    }
}
