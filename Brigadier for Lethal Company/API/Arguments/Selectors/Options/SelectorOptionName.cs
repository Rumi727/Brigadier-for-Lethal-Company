using Brigadier.NET;
using GameNetcodeStuff;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Arguments.Selectors.Options
{
    [SelectorOption("name", true)]
    public sealed class SelectorOptionName : SelectorOption
    {
        public IReadOnlyList<string> names => _names;
        readonly List<string> _names = new List<string>();

        public IReadOnlyList<string> ignoreNames => _ignoreNames;
        readonly List<string> _ignoreNames = new List<string>();

        public override void Parse(IStringReader reader)
        {
            _names.Clear();
            _ignoreNames.Clear();

            if (!reader.CanRead())
                return;

            reader.Cursor--;

            do
            {
                reader.Skip();

                if (reader.Peek() == '!')
                {
                    reader.Skip();
                    if (reader.CanRead())
                        _ignoreNames.Add(reader.ReadString());
                }
                else
                    _names.Add(reader.ReadString());
            }
            while (reader.CanRead() && reader.Peek() == '|');
        }

        public override IEnumerable<NetworkBehaviour> Calculate(IEnumerable<NetworkBehaviour> entitys, ServerCommandSource source)
        {
            foreach (NetworkBehaviour entity in entitys)
            {
                string name;
                if (entity is PlayerControllerB player)
                    name = player.playerUsername;
                else if (entity is EnemyAI enemy)
                    name = enemy.enemyType.enemyName.Replace(" ", "_").ToLower();
                else if (entity is Anomaly anomaly)
                    name = anomaly.anomalyType.anomalyName.Replace(" ", "_").ToLower();
                else if (entity is GrabbableObject item)
                    name = item.itemProperties.itemName.Replace(" ", "_").ToLower();
                else
                    name = entity.name;

                if (_names.Any())
                {
                    if (_names.Contains(name))
                        yield return entity;
                }
                else if (_ignoreNames.Any())
                {
                    if (!_ignoreNames.Contains(name))
                        yield return entity;
                }
            }
        }
    }
}
