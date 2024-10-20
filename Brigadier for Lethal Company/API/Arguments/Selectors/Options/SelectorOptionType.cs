using Brigadier.NET;
using Brigadier.NET.Exceptions;
using GameNetcodeStuff;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Arguments.Selectors.Options
{
    [SelectorOption("type", true)]
    public sealed class SelectorOptionType : SelectorOption
    {
        public const string playerType = "player";
        public const string enemyType = "enemy";
        public const string anomalyType = "anomaly";
        public const string itemType = "item";

        public IReadOnlyList<string> types => _types;
        readonly List<string> _types = new List<string>();

        public IReadOnlyList<string> ignoreTypes => _ignoreTypes;
        readonly List<string> _ignoreTypes = new List<string>();

        public override void Parse(IStringReader reader)
        {
            _types.Clear();
            _ignoreTypes.Clear();

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
                    {
                        string type = reader.ReadString();
                        if (type == playerType || type == enemyType || type == anomalyType || type == itemType)
                            _ignoreTypes.Add(type);
                        else
                            throw CommandSyntaxException.BuiltInExceptions.InvalidEntityType().CreateWithContext(reader, type);
                    }
                }
                else
                {
                    string type = reader.ReadString();
                    if (type == playerType || type == enemyType || type == anomalyType || type == itemType)
                        _types.Add(type);
                    else
                        throw CommandSyntaxException.BuiltInExceptions.InvalidEntityType().CreateWithContext(reader, type);
                }
            }
            while (reader.CanRead() && reader.Peek() == '|');
        }

        public override IEnumerable<NetworkBehaviour> Calculate(IEnumerable<NetworkBehaviour> entitys, ServerCommandSource source)
        {
            foreach (NetworkBehaviour entity in entitys)
            {
                string type = string.Empty;
                if (entity is PlayerControllerB)
                    type = playerType;
                else if (entity is EnemyAI)
                    type = enemyType;
                else if (entity is Anomaly)
                    type = anomalyType;
                else if (entity is GrabbableObject)
                    type = itemType;

                if (_types.Any())
                {
                    if (_types.Contains(type))
                        yield return entity;
                }
                else if (_ignoreTypes.Any())
                {
                    if (!_ignoreTypes.Contains(type))
                        yield return entity;
                }
            }
        }
    }
}
