using Brigadier.NET;
using System.Linq;
using UnityEngine;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Arguments
{
    public class GameObjectArgumentType : RuniArgumentType<GameObject>
    {
        public override GameObject Parse(IStringReader reader)
        {
            int hashCode = reader.ReadInt();
            return Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None).First(x => x.GetHashCode() == hashCode);
        }
    }
}
