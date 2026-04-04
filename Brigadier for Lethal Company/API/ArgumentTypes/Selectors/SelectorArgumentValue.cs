using Rumi.BrigadierForLethalCompany.API.ArgumentTypes.Selectors.Options;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Rumi.BrigadierForLethalCompany.API.ArgumentTypes.Selectors
{
    /// <summary>
    /// 선택자 인자가 반환한 값입니다
    /// </summary>
    /// <remarks>
    /// 선택자 값을 생성합니다
    /// </remarks>
    /// <param name="entitys">선택할 엔티티 목록 (타입이 <see cref="SelectorType.sender"/>) 타입일 경우 무시됨)</param>
    /// <param name="type">선택자 타입</param>
    /// <param name="selectorOptions">필터 옵션 목록</param>
    public readonly struct SelectorArgumentValue(IEnumerable<NetworkBehaviour> entitys, SelectorType type, params SelectorOption[] selectorOptions)
    {
        /// <summary>
        /// 선택할 엔티티 목록
        /// </summary>
        public IEnumerable<NetworkBehaviour> entitys { get; } = entitys;

        /// <summary>
        /// 선택자의 타입
        /// </summary>
        public SelectorType type { get; } = type;

        /// <summary>
        /// 선택자의 필터 옵션 목록
        /// </summary>
        public IReadOnlyList<SelectorOption> selectorOptions { get; } = selectorOptions;

        /// <summary>
        /// 선택자의 타입과 필터 등의 요소를 사용하여 필터된 엔티티 목록을 반환합니다
        /// </summary>
        /// <param name="source">필터할 때 사용할 커맨드 소스</param>
        public IEnumerable<NetworkBehaviour> GetEntitys(ServerCommandSource source)
        {
            if (type == SelectorType.sender)
            {
                if (source.sender != null)
                    return Enumerable.Repeat(source.sender, 1);

                return [];
            }

            IEnumerable<NetworkBehaviour> entitys = this.entitys;
            for (int i = 0; i < selectorOptions.Count; i++)
                entitys = selectorOptions[i].Calculate(entitys, source);

            if (type == SelectorType.nearby)
                return Enumerable.Repeat(entitys.Where(x => x != source.sender).MinBy(x => Vector3.Distance(x.transform.position, source.position)), 1);
            else if (type == SelectorType.random)
                return entitys.Shuffle();

            return entitys;
        }
    }
}
