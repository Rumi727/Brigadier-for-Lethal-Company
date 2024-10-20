using Brigadier.NET.ArgumentTypes;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Arguments
{
    public abstract class RuniArgumentType<TResult> : ArgumentType<TResult>
    {
        protected internal RuniArgumentType() { }

        public override bool Equals(object? o) => o is RuniArgumentType<TResult>;

        public override int GetHashCode() => GetType().GetHashCode();

        public override string ToString() => $"{typeof(TResult).Name}()";
    }
}
