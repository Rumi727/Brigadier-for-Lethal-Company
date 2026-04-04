using Brigadier.NET;
using Brigadier.NET.ArgumentTypes;
using System.Linq;
using UnityEngine;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.ArgumentTypes
{
    public class TransformsStringArgumentType : IArgumentType<Transform[]>
    {
        public Transform[] Parse(IStringReader reader)
        {
            string name = reader.ReadString();
            return [.. Object.FindObjectsByType<Transform>(FindObjectsSortMode.None).Where(x => x.name == name)];
        }
    }
}
