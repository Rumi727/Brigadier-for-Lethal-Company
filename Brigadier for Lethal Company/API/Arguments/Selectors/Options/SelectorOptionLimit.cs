using Brigadier.NET;
using System.Collections.Generic;
using Unity.Netcode;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Arguments.Selectors.Options
{
    [SelectorOption("limit")]
    public sealed class SelectorOptionLimit : SelectorOption
    {
        public override int sort => 1000;

        public int? limit { get; private set; }
        public override void Parse(IStringReader reader) => limit = reader.ReadInt();

        public override IEnumerable<NetworkBehaviour> Calculate(IEnumerable<NetworkBehaviour> entitys, ServerCommandSource source)
        {
            if (limit == null)
                yield break;

            IEnumerator<NetworkBehaviour> enumerator = entitys.GetEnumerator();
            for (int i = 0; i < limit && enumerator.MoveNext(); i++)
                yield return enumerator.Current;
        }
    }
}
