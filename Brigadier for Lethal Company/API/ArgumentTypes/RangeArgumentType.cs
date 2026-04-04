using Brigadier.NET;
using Brigadier.NET.ArgumentTypes;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.ArgumentTypes
{
    public sealed class RangeArgumentType : IArgumentType<Range>
    {
        public Range Parse(IStringReader reader) => reader.ReadRange();
    }
}
