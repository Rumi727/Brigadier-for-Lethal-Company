using Brigadier.NET;
using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Netcode;

namespace Rumi.BrigadierForLethalCompany.API.Arguments.Selectors.Options
{
    /// <summary>
    /// If a class inherits from this class and has the <see cref="SelectorOptionAttribute"/> attribute attached, it will automatically become an option available in selectors.
    /// <br/><br/>
    /// 이 클래스를 상속하고 <see cref="SelectorOptionAttribute"/> 어트리뷰트가 붙어있을 경우 자동적으로 선택자에서 사용 가능한 옵션이 됩니다
    /// </summary>
    public abstract class SelectorOption
    {
        static SelectorOption()
        {
            List<(Type type, string name, bool onlyEntity)> list = new();
            var types = ReflectionManager.types;
            for (int i = 0; i < types.Count; i++)
            {
                Type type = types[i];
                if (type.IsSubtypeOf(typeof(SelectorOption)) && type.AttributeContains<SelectorOptionAttribute>())
                {
                    SelectorOptionAttribute attribute = type.GetCustomAttribute<SelectorOptionAttribute>();
                    list.Add((type, attribute.name, attribute.notPlayer));
                }
            }

            selectorOptions = list.ToArray();
        }

        /// <summary>
        /// Gets properties of all selectors in the all assembly.
        /// <br/><br/>
        /// 모든 어셈블리에 있는 모든 선택자의 속성을 가져옵니다
        /// </summary>
        public static IReadOnlyList<(Type type, string name, bool onlyEntity)> selectorOptions;

        /// <summary>
        /// Determines how late to use filtering (higher numbers mean later)
        /// <br/><br/>
        /// 얼마나 늦게 필터링에 사용할 지 결정합니다 (숫자가 높을 수록 늦게 사용 됨)
        /// </summary>
        public virtual int sort => 0;

        /// <summary>
        /// Parse options from the current string
        /// <br/><br/>
        /// 현재 문자열에서 옵션을 구문 분석합니다
        /// </summary>
        /// <param name="reader"></param>
        public abstract void Parse(IStringReader reader);

        /// <summary>
        /// Filters a list of entities using parsed values
        /// <br/><br/>
        /// 구문 분석된 값을 사용하여 엔티티 목록을 필터링합니다
        /// </summary>
        public abstract IEnumerable<NetworkBehaviour> Calculate(IEnumerable<NetworkBehaviour> entitys, ServerCommandSource source);
    }
}
