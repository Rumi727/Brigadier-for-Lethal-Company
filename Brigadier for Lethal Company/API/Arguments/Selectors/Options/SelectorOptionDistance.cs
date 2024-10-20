using Brigadier.NET;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Arguments.Selectors.Options
{
    [SelectorOption("distance")]
    public sealed class SelectorOptionDistance : SelectorOption
    {
        public Range? range { get; private set; }
        public override void Parse(IStringReader reader) => range = reader.ReadRange();

        public override IEnumerable<NetworkBehaviour> Calculate(IEnumerable<NetworkBehaviour> entitys, ServerCommandSource source)
        {
            if (range == null || source.sender == null)
                return Enumerable.Empty<NetworkBehaviour>();

            return entitys.Where(x =>
            {
                Debug.Log(x.name);
                Debug.Log("distance : " + Vector2.Distance(source.sender.transform.position, x.transform.position));
                Debug.Log("range : " + range);

                return range.Value.Contains(Vector2.Distance(source.sender.transform.position, x.transform.position));
            });
        }
    }
}
