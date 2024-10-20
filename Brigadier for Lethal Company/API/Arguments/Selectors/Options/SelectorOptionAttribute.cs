using System;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Arguments.Selectors.Options
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class SelectorOptionAttribute : Attribute
    {
        public SelectorOptionAttribute(string name) => this.name = name;
        public SelectorOptionAttribute(string name, bool notPlayer)
        {
            this.name = name;
            this.notPlayer = notPlayer;
        }

        public string name { get; }
        public bool notPlayer { get; }
    }
}
