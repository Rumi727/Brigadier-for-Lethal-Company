#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Arguments
{
    public struct CoordinatesArgumentValue<T>
    {
        public T value;
        public CoordinateType coordinatesType;

        public CoordinatesArgumentValue(T value, CoordinateType coordinatesType)
        {
            this.value = value;
            this.coordinatesType = coordinatesType;
        }
    }
}
