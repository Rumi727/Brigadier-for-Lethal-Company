using Brigadier.NET;
using System.Linq;
using UnityEngine;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Arguments
{
    public class SpriteArgumentType : RuniArgumentType<Sprite>
    {
        public override Sprite Parse(IStringReader reader)
        {
            string name = reader.ReadString();
            return Object.FindObjectsByType<Sprite>(FindObjectsSortMode.None).First(x => x.name == name);
        }
    }
}