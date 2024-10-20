namespace Rumi.BrigadierForLethalCompany.API.Arguments.Selectors
{
    /// <summary>
    /// 선택자 타입
    /// </summary>
    public enum SelectorType
    {
        /// <summary>
        /// 지정되지 않은 타입이며, 선택자 옵션에서 최종적으로 걸러진 모든 엔티티를 가져옵니다
        /// </summary>
        none,
        /// <summary>
        /// 현재 개체를 가져옵니다
        /// </summary>
        sender,
        /// <summary>
        /// 선택자 옵션에서 최종적으로 걸러진 엔티티 중에서 현재 위치와 가장 가까운 엔티티 하나를 가져옵니다
        /// </summary>
        nearby,
        /// <summary>
        /// 선택자 옵션에서 최종적으로 걸러진 엔티티 중 랜덤으로 한개를 가져옵니다
        /// </summary>
        random
    }
}
