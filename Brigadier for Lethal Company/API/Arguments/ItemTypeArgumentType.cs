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
    public class ItemTypeArgumentType : RuniArgumentType<Item>
    {
        public override Item Parse(IStringReader reader)
        {
            string type = reader.ReadString();
            var anomalys = GetItemTypes().Where(x => x.itemName == type);
            if (anomalys.Any())
                return anomalys.First().itemType;

            throw CommandSyntaxException.BuiltInExceptions.InvalidItemType().CreateWithContext(reader, type);
        }

        public override Task<Suggestions> ListSuggestions<TSource>(CommandContext<TSource> context, SuggestionsBuilder builder)
        {
            foreach ((string itemName, _) in GetItemTypes())
            {
                if (itemName.StartsWith(builder.Remaining))
                    builder.Suggest(itemName);
            }

            return builder.BuildFuture();
        }

        static IEnumerable<(string itemName, Item itemType)>? types;
        public static IEnumerable<(string itemName, Item itemType)> GetItemTypes() => types ??= Resources.FindObjectsOfTypeAll<Item>().Select(x => (x.itemName.Replace(" ", "_").ToLower(), x));
    }
}
