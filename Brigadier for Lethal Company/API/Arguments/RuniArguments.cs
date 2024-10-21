using Brigadier.NET;
using Brigadier.NET.ArgumentTypes;
using Brigadier.NET.Context;
using Rumi.BrigadierForLethalCompany.API.Arguments.Selectors;
using System;
using UnityEngine;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.API.Arguments
{
    /// <summary>
    /// <see cref="Brigadier.NET.Arguments"/> 클래스를 대체합니다
    /// </summary>
    public static class RuniArguments
    {
        public static IntegerArgumentType Integer(int min = int.MinValue, int max = int.MaxValue) => Brigadier.NET.Arguments.Integer(min, max);
        public static int GetInteger<TSource>(CommandContext<TSource> context, string name) => context.GetArgument<int>(name);

        public static BoolArgumentType Bool() => Brigadier.NET.Arguments.Bool();
        public static bool GetBool<TSource>(CommandContext<TSource> context, string name) => context.GetArgument<bool>(name);

        public static DoubleArgumentType Double(double min = double.MinValue, double max = double.MaxValue) => Brigadier.NET.Arguments.Double(min, max);
        public static double GetDouble<TSource>(CommandContext<TSource> context, string name) => context.GetArgument<double>(name);

        public static FloatArgumentType Float(float min = float.MinValue, float max = float.MaxValue) => Brigadier.NET.Arguments.Float(min, max);
        public static float GetFloat<TSource>(CommandContext<TSource> context, string name) => context.GetArgument<float>(name);

        public static LongArgumentType Long(long min = long.MinValue, long max = long.MaxValue) => Brigadier.NET.Arguments.Long(min, max);
        public static long GetLong<TSource>(CommandContext<TSource> context, string name) => context.GetArgument<long>(name);

        public static StringArgumentType Word() => Brigadier.NET.Arguments.Word();
        public static StringArgumentType String() => Brigadier.NET.Arguments.String();
        public static StringArgumentType GreedyString() => Brigadier.NET.Arguments.GreedyString();
        public static string GetString<TSource>(CommandContext<TSource> context, string name) => context.GetArgument<string>(name);

        public static CoordinatesArgument<T> Coordinates<T>(T? min, T? max, Func<IStringReader, T> readFunc, int argumentCount, int localArgumentCount) where T : struct, IComparable<T> => new CoordinatesArgument<T>(min, max, readFunc, argumentCount, localArgumentCount);
        public static CoordinatesArgumentValue<TValue>[] GetCoordinates<TSource, TValue>(CommandContext<TSource> context, string name) => context.GetArgument<CoordinatesArgumentValue<TValue>[]>(name);

        #region Vector2
        public static CoordinatesArgument<float> Vector2(float? min = null, float? max = null, bool allowLocalType = true) => Coordinates(min, max, static x => x.ReadFloat(), 2, allowLocalType ? 3 : 0);
        public static Vector2 GetVector2<TSource>(CommandContext<TSource> context, string name) where TSource : ServerCommandSource => GetVector2(context, name, context.Source.position, context.Source.rotation);
        public static Vector2 GetVector2<TSource>(CommandContext<TSource> context, string name, Vector3 offset, Vector3 localRotation)
        {
            var argument = GetCoordinates<TSource, float>(context, name);
            if (argument.Length < 2)
                return UnityEngine.Vector2.zero;

            Vector2 value = new Vector2(argument[0].value, argument[1].value);
            if (argument[0].coordinatesType == CoordinateType.local && argument.Length == 3)
            {
                Vector3 result = offset + (Quaternion.Euler(localRotation) * new Vector3(value.x, value.y, argument[2].value));
                return new Vector2(result.x, result.z);
            }
            else
            {
                if (argument[0].coordinatesType == CoordinateType.offset)
                    value.x += offset.x;
                if (argument[1].coordinatesType == CoordinateType.offset)
                    value.y += offset.y;

                return value;
            }
        }
        #endregion

        #region Vector2Int
        public static CoordinatesArgument<int> Vector2Int(int? min = null, int? max = null, bool allowLocalType = true) => Coordinates(min, max, static x => x.ReadInt(), 2, allowLocalType ? 3 : 0);
        public static Vector2Int GetVector2Int<TSource>(CommandContext<TSource> context, string name) where TSource : ServerCommandSource => GetVector2Int(context, name, context.Source.position, context.Source.rotation);
        public static Vector2Int GetVector2Int<TSource>(CommandContext<TSource> context, string name, Vector3 offset, Vector3 localRotation)
        {
            var argument = GetCoordinates<TSource, int>(context, name);
            if (argument.Length < 2)
                return UnityEngine.Vector2Int.zero;

            Vector2Int value = new Vector2Int(argument[0].value, argument[1].value);
            if (argument[0].coordinatesType == CoordinateType.local && argument.Length == 3)
            {
                Vector3 result = offset + (Quaternion.Euler(localRotation) * new Vector3(value.x, value.y, argument[2].value));
                return new Vector2Int(Mathf.FloorToInt(result.x), Mathf.FloorToInt(result.z));
            }
            else
            {
                if (argument[0].coordinatesType == CoordinateType.offset)
                    value.x += Mathf.FloorToInt(offset.x);
                if (argument[1].coordinatesType == CoordinateType.offset)
                    value.y += Mathf.FloorToInt(offset.y);

                return value;
            }
        }
        #endregion

        #region Vector3
        public static CoordinatesArgument<float> Vector3(float? min = null, float? max = null, bool allowLocalType = true) => Coordinates(min, max, static x => x.ReadFloat(), 3, allowLocalType ? 3 : 0);
        public static Vector3 GetVector3<TSource>(CommandContext<TSource> context, string name) where TSource : ServerCommandSource => GetVector3(context, name, context.Source.position, context.Source.rotation);
        public static Vector3 GetVector3<TSource>(CommandContext<TSource> context, string name, Vector3 offset, Vector3 localRotation)
        {
            var argument = GetCoordinates<TSource, float>(context, name);
            if (argument.Length < 3)
                return UnityEngine.Vector3.zero;

            Vector3 value = new Vector3(argument[0].value, argument[1].value, argument[2].value);
            if (argument[0].coordinatesType == CoordinateType.local)
                return offset + (Quaternion.Euler(localRotation) * new Vector3(value.x, value.y, value.z));
            else
            {
                if (argument[0].coordinatesType == CoordinateType.offset)
                    value.x += offset.x;
                if (argument[1].coordinatesType == CoordinateType.offset)
                    value.y += offset.y;
                if (argument[2].coordinatesType == CoordinateType.offset)
                    value.z += offset.z;

                return value;
            }
        }
        #endregion

        #region Vector3Int
        public static CoordinatesArgument<int> Vector3Int(int? min = null, int? max = null, bool allowLocalType = true) => Coordinates(min, max, static x => x.ReadInt(), 3, allowLocalType ? 3 : 0);
        public static Vector3Int GetVector3Int<TSource>(CommandContext<TSource> context, string name) where TSource : ServerCommandSource => GetVector3Int(context, name, context.Source.position, context.Source.rotation);
        public static Vector3Int GetVector3Int<TSource>(CommandContext<TSource> context, string name, Vector3 offset, Vector3 localRotation)
        {
            var argument = GetCoordinates<TSource, int>(context, name);
            if (argument.Length < 3)
                return UnityEngine.Vector3Int.zero;

            Vector3Int value = new Vector3Int(argument[0].value, argument[1].value, argument[2].value);
            if (argument[0].coordinatesType == CoordinateType.local && argument.Length == 3)
            {
                Vector3 result = offset + (Quaternion.Euler(localRotation) * new Vector3(value.x, value.y, value.z));
                return new Vector3Int(Mathf.FloorToInt(result.x), Mathf.FloorToInt(result.y), Mathf.FloorToInt(result.z));
            }
            else
            {
                if (argument[0].coordinatesType == CoordinateType.offset)
                    value.x += Mathf.FloorToInt(offset.x);
                if (argument[1].coordinatesType == CoordinateType.offset)
                    value.y += Mathf.FloorToInt(offset.y);
                if (argument[2].coordinatesType == CoordinateType.offset)
                    value.z += Mathf.FloorToInt(offset.z);

                return value;
            }
        }
        #endregion

        #region Vector4
        public static CoordinatesArgument<float> Vector4(float? min = null, float? max = null) => Coordinates(min, max, static x => x.ReadFloat(), 4, 0);
        public static Vector4 GetVector4<TSource>(CommandContext<TSource> context, string name, Vector4 offset)
        {
            var argument = GetCoordinates<TSource, float>(context, name);
            if (argument.Length < 4)
                return UnityEngine.Vector4.zero;

            Vector4 value = new Vector4(argument[0].value, argument[1].value, argument[2].value, argument[3].value);
            if (argument[0].coordinatesType == CoordinateType.offset)
                value.x += offset.x;
            if (argument[1].coordinatesType == CoordinateType.offset)
                value.y += offset.y;
            if (argument[2].coordinatesType == CoordinateType.offset)
                value.z += offset.z;
            if (argument[3].coordinatesType == CoordinateType.offset)
                value.w += offset.w;

            return value;
        }
        #endregion

        #region Rect
        public static CoordinatesArgument<float> Rect(float? min = null, float? max = null) => Coordinates(min, max, static x => x.ReadFloat(), 4, 0);
        public static Rect GetRect<TSource>(CommandContext<TSource> context, string name, Rect offset)
        {
            var argument = GetCoordinates<TSource, float>(context, name);
            if (argument.Length < 4)
                return UnityEngine.Rect.zero;

            Rect value = new Rect(argument[0].value, argument[1].value, argument[2].value, argument[3].value);
            if (argument[0].coordinatesType == CoordinateType.offset)
                value.x += offset.x;
            if (argument[1].coordinatesType == CoordinateType.offset)
                value.y += offset.y;
            if (argument[2].coordinatesType == CoordinateType.offset)
                value.width += offset.width;
            if (argument[3].coordinatesType == CoordinateType.offset)
                value.height += offset.height;

            return value;
        }
        #endregion

        #region RectInt
        public static CoordinatesArgument<int> RectInt(int? min = null, int? max = null) => Coordinates(min, max, static x => x.ReadInt(), 4, 0);
        public static Rect GetRectInt<TSource>(CommandContext<TSource> context, string name, Rect offset)
        {
            var argument = GetCoordinates<TSource, int>(context, name);
            if (argument.Length < 4)
                return UnityEngine.Rect.zero;

            Rect value = new Rect(argument[0].value, argument[1].value, argument[2].value, argument[3].value);
            if (argument[0].coordinatesType == CoordinateType.offset)
                value.x += offset.x;
            if (argument[1].coordinatesType == CoordinateType.offset)
                value.y += offset.y;
            if (argument[2].coordinatesType == CoordinateType.offset)
                value.width += offset.width;
            if (argument[3].coordinatesType == CoordinateType.offset)
                value.height += offset.height;

            return value;
        }
        #endregion

        #region Color
        public static CoordinatesArgument<float> Color() => Coordinates(0, 1, static x => x.ReadInt() / 255f, 3, 0);
        public static Color GetColor<TSource>(CommandContext<TSource> context, string name, Color offset)
        {
            var argument = GetCoordinates<TSource, float>(context, name);
            if (argument.Length < 3)
                return UnityEngine.Color.white;

            Color value = new Color(argument[0].value, argument[1].value, argument[2].value);
            if (argument[0].coordinatesType == CoordinateType.offset)
                value.r += offset.r;
            if (argument[1].coordinatesType == CoordinateType.offset)
                value.g += offset.g;
            if (argument[2].coordinatesType == CoordinateType.offset)
                value.b += offset.b;

            return value;
        }
        #endregion

        #region Color Alpha
        public static CoordinatesArgument<float> ColorAlpha() => Coordinates(0, 1, static x => x.ReadInt() / 255f, 4, 0);
        public static Color GetColorAlpha<TSource>(CommandContext<TSource> context, string name, Color offset)
        {
            var argument = GetCoordinates<TSource, float>(context, name);
            if (argument.Length < 4)
                return UnityEngine.Color.white;

            Color value = new Color(argument[0].value, argument[1].value, argument[2].value, argument[3].value);
            if (argument[0].coordinatesType == CoordinateType.offset)
                value.r += offset.r;
            if (argument[1].coordinatesType == CoordinateType.offset)
                value.g += offset.g;
            if (argument[2].coordinatesType == CoordinateType.offset)
                value.b += offset.b;
            if (argument[3].coordinatesType == CoordinateType.offset)
                value.a += offset.a;

            return value;
        }
        #endregion

        public static SelectorArgument Selector(bool onlyPlayer = false, bool limit = false) => new SelectorArgument(onlyPlayer, limit);
        public static SelectorArgumentValue GetSelector<TSource>(CommandContext<TSource> context, string name) where TSource : ServerCommandSource => context.GetArgument<SelectorArgumentValue>(name);

        public static TransformArgumentType Transform() => new TransformArgumentType();
        public static Transform GetTransform<TSource>(CommandContext<TSource> context, string name) => context.GetArgument<Transform>(name);

        public static TransformsStringArgumentType Transforms() => new TransformsStringArgumentType();
        public static Transform[] GetTransforms<TSource>(CommandContext<TSource> context, string name) => context.GetArgument<Transform[]>(name);

        public static GameObjectArgumentType GameObject() => new GameObjectArgumentType();
        public static GameObject GetGameObject<TSource>(CommandContext<TSource> context, string name) => context.GetArgument<GameObject>(name);

        public static GameObjectsStringArgumentType GameObjects() => new GameObjectsStringArgumentType();
        public static GameObject[] GetGameObjects<TSource>(CommandContext<TSource> context, string name) => context.GetArgument<GameObject[]>(name);

        public static SpriteArgumentType Sprite() => new SpriteArgumentType();
        public static Sprite GetSprite<TSource>(CommandContext<TSource> context, string name) => context.GetArgument<Sprite>(name);

        public static SpritesArgumentType Sprites() => new SpritesArgumentType();
        public static Sprite[] GetSprites<TSource>(CommandContext<TSource> context, string name) => context.GetArgument<Sprite[]>(name);

        public static PosSwizzleArgumentType PosSwizzle() => new PosSwizzleArgumentType();
        public static PosSwizzleEnum GetPosSwizzle<TSource>(CommandContext<TSource> context, string name) => context.GetArgument<PosSwizzleEnum>(name);

        public static EnemyTypeArgumentType EnemyType() => new EnemyTypeArgumentType();
        public static EnemyType GetEnemyType<TSource>(CommandContext<TSource> context, string name) => context.GetArgument<EnemyType>(name);

        public static AnomalyTypeArgumentType AnomalyType() => new AnomalyTypeArgumentType();
        public static AnomalyType GetAnomalyType<TSource>(CommandContext<TSource> context, string name) => context.GetArgument<AnomalyType>(name);

        public static ItemTypeArgumentType ItemType() => new ItemTypeArgumentType();
        public static Item GetItemType<TSource>(CommandContext<TSource> context, string name) => context.GetArgument<Item>(name);
    }
}