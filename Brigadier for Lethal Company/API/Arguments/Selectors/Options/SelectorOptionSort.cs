using Brigadier.NET;
using Brigadier.NET.Exceptions;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Arguments.Selectors.Options
{
    [SelectorOption("sort", true)]
    public sealed class SelectorOptionSort : SelectorOption
    {
        SelectorOptionSorttype type;
        public override void Parse(IStringReader reader)
        {
            string type = reader.ReadString();
            if (type == "near")
                this.type = SelectorOptionSorttype.near;
            else if (type == "far")
                this.type = SelectorOptionSorttype.far;
            else if (type == "random")
                this.type = SelectorOptionSorttype.random;
            else
                throw CommandSyntaxException.BuiltInExceptions.InvalidSort().CreateWithContext(reader, type);
        }

        public override IEnumerable<NetworkBehaviour> Calculate(IEnumerable<NetworkBehaviour> entitys, ServerCommandSource source)
        {
            if (type == SelectorOptionSorttype.near)
                return entitys.OrderBy(x => Vector3.Distance(x.transform.position, source.position));
            else if (type == SelectorOptionSorttype.far)
                return entitys.OrderByDescending(x => Vector3.Distance(x.transform.position, source.position));
            else if (type == SelectorOptionSorttype.random)
                return entitys.Shuffle();

            return Enumerable.Empty<NetworkBehaviour>();
        }
    }
}
