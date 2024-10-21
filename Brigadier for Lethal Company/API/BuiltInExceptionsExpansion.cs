using Brigadier.NET;
using Brigadier.NET.Exceptions;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API
{
    public static class BuiltInExceptionsExpansion
    {
#pragma warning disable IDE0060 // 사용하지 않는 매개 변수를 제거하세요.
        public static Dynamic2CommandExceptionType ColonTooMany(this IBuiltInExceptionProvider e) => new(static (found, max) => new LiteralMessage($"Number of colons must not be many than {max}, found {found}"));
        public static SimpleCommandExceptionType InvalidPosSwizzle(this IBuiltInExceptionProvider e) => new SimpleCommandExceptionType(new LiteralMessage("Invalid swizzle, expected combination of 'x', 'y' and 'z'"));
        public static SimpleCommandExceptionType WorldLocalPosMixed(this IBuiltInExceptionProvider e) => new SimpleCommandExceptionType(new LiteralMessage("Cannot mix world & local coordinates (everything must either use ^ or not)"));
        public static SimpleCommandExceptionType EntityToMany(this IBuiltInExceptionProvider e) => new SimpleCommandExceptionType(new LiteralMessage("Only one entity is allowed, but the provided selector allows more than one"));
        public static SimpleCommandExceptionType PlayerToMany(this IBuiltInExceptionProvider e) => new SimpleCommandExceptionType(new LiteralMessage("Only one player is allowed, but the provided selector allows more than one"));
        public static SimpleCommandExceptionType OnlyPlayer(this IBuiltInExceptionProvider e) => new SimpleCommandExceptionType(new LiteralMessage("Only players may be affected by this command, but the provided selector includes entities"));

        public static DynamicCommandExceptionType SelectorUnkown(this IBuiltInExceptionProvider e) => new DynamicCommandExceptionType(static x => new LiteralMessage($"Unknown selector type '{x}'"));
        public static DynamicCommandExceptionType SelectorOptionsUnknown(this IBuiltInExceptionProvider e) => new DynamicCommandExceptionType(static x => new LiteralMessage($"Unknown option '{x}'"));
        public static SimpleCommandExceptionType SelectorOptionsUnterminated(this IBuiltInExceptionProvider e) => new SimpleCommandExceptionType(new LiteralMessage("Expected end of options"));
        public static DynamicCommandExceptionType SelectorOptionsValueless(this IBuiltInExceptionProvider e) => new(static x => new LiteralMessage($"Expected value for option '{x}'"));

        public static SimpleCommandExceptionType RangeEmpty(this IBuiltInExceptionProvider e) => new(new LiteralMessage("Expected value or range of values"));

        public static DynamicCommandExceptionType InvalidSort(this IBuiltInExceptionProvider e) => new(static x => new LiteralMessage($"Invalid or unknown sort type '{x}'"));

        public static DynamicCommandExceptionType InvalidEntityType(this IBuiltInExceptionProvider e) => new(static x => new LiteralMessage($"Invalid or unknown entity type '{x}'"));
        public static DynamicCommandExceptionType InvalidItemType(this IBuiltInExceptionProvider e) => new(static x => new LiteralMessage($"Unknown item '{x}'"));
#pragma warning restore IDE0060 // 사용하지 않는 매개 변수를 제거하세요.
    }
}
