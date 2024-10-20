using Brigadier.NET;
using GameNetcodeStuff;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Arguments.Selectors.Options
{
    [SelectorOption("death")]
    public sealed class SelectorOptionDeath : SelectorOption
    {
        public override int sort => 1000;

        public bool? death { get; private set; }
        public override void Parse(IStringReader reader) => death = reader.ReadBoolean();

        public override IEnumerable<NetworkBehaviour> Calculate(IEnumerable<NetworkBehaviour> entitys, ServerCommandSource source)
        {
            if (death == null)
                return Enumerable.Empty<NetworkBehaviour>();

            return entitys.Where(x =>
            {
                if (x is EnemyAI enemy)
                    return (death.Value && enemy.isEnemyDead) || (!death.Value && !enemy.isEnemyDead);
                else if (x is PlayerControllerB player)
                    return (death.Value && player.isPlayerDead) || (!death.Value && !player.isPlayerDead);

                return false;
            });
        }
    }
}
