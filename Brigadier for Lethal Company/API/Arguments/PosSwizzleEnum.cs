using System;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Arguments
{
    [Flags]
    public enum PosSwizzleEnum
    {
        none = 0,
        x = 1 << 1,
        y = 1 << 2,
        z = 1 << 3,

        all = x | y | z
    }
}
