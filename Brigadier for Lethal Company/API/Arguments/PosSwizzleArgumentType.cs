using Brigadier.NET;
using Brigadier.NET.Context;
using Brigadier.NET.Suggestion;
using System.Threading.Tasks;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Arguments
{
    public class PosSwizzleArgumentType : RuniArgumentType<PosSwizzleEnum>
    {
        public override PosSwizzleEnum Parse(IStringReader reader) => reader.ReadPosSwizzle();

        public override Task<Suggestions> ListSuggestions<TSource>(CommandContext<TSource> context, SuggestionsBuilder builder)
        {
            if (builder.Remaining == "x")
            {
                builder.Suggest("xy");
                builder.Suggest("xyz");
            }
            else if (builder.Remaining == "xy")
                builder.Suggest("xyz");
            else if (builder.Remaining == "xz")
                builder.Suggest("xzy");
            else if (builder.Remaining == "y")
            {
                builder.Suggest("yx");
                builder.Suggest("yxz");
            }
            else if (builder.Remaining == "yx")
                builder.Suggest("yxz");
            else if (builder.Remaining == "yz")
                builder.Suggest("yzx");
            else if (builder.Remaining == "z")
            {
                builder.Suggest("zx");
                builder.Suggest("zxy");
            }
            else if (builder.Remaining == "zx")
                builder.Suggest("zxy");
            else if (builder.Remaining == "zy")
                builder.Suggest("zyx");
            else if (string.IsNullOrEmpty(builder.Remaining))
            {
                builder.Suggest("x");
                builder.Suggest("xy");
                builder.Suggest("xyz");
            }

            return builder.BuildFuture();
        }
    }
}
