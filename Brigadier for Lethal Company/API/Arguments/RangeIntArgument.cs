using Brigadier.NET;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Arguments
{
    public sealed class RangeIntArgument : RuniArgumentType<RangeInt>
    {
        public override RangeInt Parse(IStringReader reader) => reader.ReadRangeInt();
    }
}
