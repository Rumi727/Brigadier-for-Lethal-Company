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
    public class AnomalyTypeArgumentType : RuniArgumentType<AnomalyType>
    {
        public override AnomalyType Parse(IStringReader reader)
        {
            string type = reader.ReadString();
            var anomalys = GetAnomalyTypes().Where(x => x.anomalyName == type);
            if (anomalys.Any())
                return anomalys.First().anomalyType;

            throw CommandSyntaxException.BuiltInExceptions.InvalidEntityType().CreateWithContext(reader, type);
        }

        public override Task<Suggestions> ListSuggestions<TSource>(CommandContext<TSource> context, SuggestionsBuilder builder)
        {
            foreach ((string anomalyName, _) in GetAnomalyTypes())
            {
                if (anomalyName.StartsWith(builder.Remaining))
                    builder.Suggest(anomalyName);
            }

            return builder.BuildFuture();
        }

        static IEnumerable<(string anomalyName, AnomalyType anomalyType)>? types;
        public static IEnumerable<(string anomalyName, AnomalyType anomalyType)> GetAnomalyTypes() => types ??= Resources.FindObjectsOfTypeAll<AnomalyType>().Select(static x => (x.anomalyName.Replace(" ", "_").ToLower(), x));
    }
}
