using Brigadier.NET.Builder;
using GameNetcodeStuff;
using Rumi.BrigadierForLethalCompany.API.ArgumentTypes.Selectors;
using System.Linq;
using Unity.Netcode;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Commands
{
    public sealed class OpCommand : ServerCommand
    {
        public const string resultText = "Operator granted to {0}";

        OpCommand() { }

        public override void Register()
        {
            //op <Entity:targets>
            dispatcher.Register(x =>
                x.Literal("op")
                    .Requires(x =>
                    {
                        if (x.isOp)
                            return true;

                        return x.firstPlayer != null && x.firstPlayer.OwnerClientId == NetworkManager.Singleton.LocalClientId;
                    })
                    .Then(x =>
                        x.Argument("targets", LethalArguments.Selector(true))
                            .Suggests((context, builder) =>
                            {
                                SelectorArgumentType.PlayerListSuggestions(BFLCUtility.GetPlayers().Where(x => !IsOp(x)), context, builder);
                                return builder.BuildAsync();
                            })
                            .Executes(x =>
                            {
                                var targets = LethalArguments.GetSelector(x, "targets").GetEntitys(x.Source).OfType<PlayerControllerB>();

                                int count = 0;
                                foreach (var target in targets)
                                {
                                    if (target is PlayerControllerB player)
                                    {
                                        AddOp(player);
                                        count++;
                                    }
                                }

                                x.Source.SendCommandResult(string.Format(resultText, targets.GetPlayerName()));
                                return count;
                            })
                    )
            );
        }
    }
}
