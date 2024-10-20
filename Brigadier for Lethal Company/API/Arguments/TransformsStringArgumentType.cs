using Brigadier.NET;
using System.Linq;
using UnityEngine;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Arguments
{
    public class TransformsStringArgumentType : RuniArgumentType<Transform[]>
    {
        public override Transform[] Parse(IStringReader reader)
        {
            string name = reader.ReadString();
            return Object.FindObjectsByType<Transform>(FindObjectsSortMode.None).Where(x => x.name == name).ToArray();
        }
    }
}
