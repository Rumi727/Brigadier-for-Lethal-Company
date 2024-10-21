using Brigadier.NET;
using Brigadier.NET.Context;
using Brigadier.NET.Exceptions;
using Brigadier.NET.Suggestion;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Arguments
{
    public class EnemyTypeArgumentType : RuniArgumentType<EnemyType>
    {
        public override EnemyType Parse(IStringReader reader)
        {
            string type = reader.ReadString();
            var enemys = GetEnemyTypes().Where(x => x.enemyName == type);
            if (enemys.Any())
                return enemys.First().enemyType;

            throw CommandSyntaxException.BuiltInExceptions.InvalidEntityType().CreateWithContext(reader, type);
        }

        public override Task<Suggestions> ListSuggestions<TSource>(CommandContext<TSource> context, SuggestionsBuilder builder)
        {
            foreach ((string enemyName, _) in GetEnemyTypes())
            {
                if (enemyName.StartsWith(builder.Remaining))
                    builder.Suggest(enemyName);
            }

            return builder.BuildFuture();
        }

        static IEnumerable<(string enemyName, EnemyType enemyType)>? types;
        public static IEnumerable<(string enemyName, EnemyType enemyType)> GetEnemyTypes() => types ??= Resources.FindObjectsOfTypeAll<EnemyType>().Select(static x => (x.enemyName.Replace(" ", "_").ToLower(), x));
    }
}
