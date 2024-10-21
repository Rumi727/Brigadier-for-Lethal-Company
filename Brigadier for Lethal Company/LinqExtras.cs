using System.Collections.Generic;

#pragma warning disable IDE0130 // 네임스페이스가 폴더 구조와 일치하지 않습니다.
namespace System.Linq
#pragma warning restore IDE0130 // 네임스페이스가 폴더 구조와 일치하지 않습니다.
{
    /// <summary>IEnumerable`1 클래스에 대한 Linq 구현 확장입니다</summary>
    public static class LinqExtras
    {
        /// <summary>시퀀스에서 최소 값을 가진 요소를 반환합니다</summary>
        /// <typeparam name="TSource">최소 값을 가져올 시퀀스</typeparam>
        /// <typeparam name="TKey">시퀀스 요소에서 비교할 값</typeparam>
        /// <param name="source">최소 값을 가져올 시퀀스</param>
        /// <param name="selector">각 요소에 적용할 변환 함수</param>
        /// <returns>시퀀스에서 최소 값을 가진 요소</returns>
        /// <exception cref="ArgumentNullException">source가 null 입니다</exception>
        /// <exception cref="InvalidOperationException">빈 시퀀스입니다</exception>
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            using (IEnumerator<TSource> sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                    throw new InvalidOperationException("Empty sequence");

                var comparer = Comparer<TKey>.Default;
                TSource min = sourceIterator.Current;
                TKey minKey = selector(min);

                while (sourceIterator.MoveNext())
                {
                    TSource current = sourceIterator.Current;
                    TKey currentKey = selector(current);

                    if (comparer.Compare(currentKey, minKey) >= 0)
                        continue;

                    min = current;
                    minKey = currentKey;
                }

                return min;
            }
        }

        /// <summary>시퀀스에 요소가 정확히 하나만 있는지 확인합니다</summary>
        /// <typeparam name="TSource">검사할 시퀀스</typeparam>
        /// <param name="source">검사할 시퀀스</param>
        /// <returns>시퀀스에 요소가 하나만 있는지에 대한 여부</returns>
        /// <exception cref="ArgumentNullException">source가 null 입니다</exception>
        public static bool CountIsOne<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            using IEnumerator<TSource> enumerator = source.GetEnumerator();

            if (enumerator.MoveNext())
            {
                if (enumerator.MoveNext())
                    return false;

                return true;
            }

            return false;
        }

        /// <summary>
        /// 시퀀스 요소를 랜덤하게 섞습니다
        /// </summary>
        /// <typeparam name="TSource">섞을 시퀀스</typeparam>
        /// <param name="source">섞을 시퀀스</param>
        public static IEnumerable<TSource> Shuffle<TSource>(this IEnumerable<TSource> source)
        {
            Random random = new Random();
            return source.OrderBy(x => random.Next());
        }

        /// <summary>
        /// 배열을 특정 크기로 자릅니다
        /// </summary>
        public static IEnumerable<IEnumerable<T>> Split<T>(this T[] arr, int size)
        {
            for (var i = 0; i < arr.Length / size + 1; i++)
                yield return arr.Skip(i * size).Take(size);
        }
    }
}
