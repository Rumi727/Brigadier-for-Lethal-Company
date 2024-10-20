using System;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API
{
    /// <summary>숫자의 범위를 지정하는 구조체</summary>
    public readonly struct Range : IEquatable<Range>
    {
        public readonly float? min;
        public readonly float? max;

        public Range(float? value) => min = max = value;

        public Range(float? min, float? max)
        {
            if (min != null && max != null && max < min)
            {
                this.max = max;
                this.min = min;
            }
            else
            {
                this.min = min;
                this.max = max;
            }
        }

        public bool Contains(float value) => (min == null || min.Value <= value) && (max == null || value <= max.Value);

        public bool Equals(Range other) => ((min == null && other.min == null) || (min != null && other.min != null && min.Value == other.min.Value)) && ((max == null && other.max == null) || (max != null && other.max != null && max.Value == other.max.Value));

        public override bool Equals(object? obj) => obj is Range range && Equals(range);
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 542216;
                hash += min.GetHashCode() * 215321;
                hash += max.GetHashCode() * 826715;

                return hash;
            }
        }

        public static bool operator ==(Range left, Range right) => left.Equals(right);
        public static bool operator !=(Range left, Range right) => !(left == right);

        public override string ToString() => $"{min}..{max}";
    }
}
