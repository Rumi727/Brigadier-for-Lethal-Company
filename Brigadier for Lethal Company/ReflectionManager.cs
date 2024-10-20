using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Rumi.BrigadierForLethalCompany
{
    static class ReflectionManager
    {
        static ReflectionManager()
        {
            assemblys = AppDomain.CurrentDomain.GetAssemblies();

            List<Type> result = new List<Type>();
            for (int assemblysIndex = 0; assemblysIndex < assemblys.Count; assemblysIndex++)
            {
                Type[] types = assemblys[assemblysIndex].GetTypes();
                for (int typesIndex = 0; typesIndex < types.Length; typesIndex++)
                {
                    Type type = types[typesIndex];
                    result.Add(type);
                }
            }

            types = result.ToArray();
        }

        /// <summary>
        /// All loaded assemblys
        /// </summary>
        public static IReadOnlyList<Assembly> assemblys { get; }

        /// <summary>
        /// All loaded types
        /// </summary>
        public static IReadOnlyList<Type> types { get; }

        /// <summary>type != typeof(T) &amp;&amp; typeof(T).IsAssignableFrom(type)</summary>
        public static bool IsSubtypeOf<T>(this Type type) => type != typeof(T) && typeof(T).IsAssignableFrom(type);
        /// <summary>type != surclass &amp;&amp; surclass.IsAssignableFrom(type)</summary>
        public static bool IsSubtypeOf(this Type type, Type surclass) => type != surclass && surclass.IsAssignableFrom(type);

        public static bool AttributeContains<T>(this MemberInfo element) where T : Attribute => element.AttributeContains(typeof(T));
        public static bool AttributeContains(this MemberInfo element, Type attribute) => Attribute.GetCustomAttributes(element, attribute).Length > 0;

        public static bool AttributeContains<T>(this Assembly element) where T : Attribute => element.AttributeContains(typeof(T));
        public static bool AttributeContains(this Assembly element, Type attribute) => Attribute.GetCustomAttributes(element, attribute).Length > 0;

        public static bool AttributeContains<T>(this ParameterInfo element) where T : Attribute => element.AttributeContains(typeof(T));
        public static bool AttributeContains(this ParameterInfo element, Type attribute) => Attribute.GetCustomAttributes(element, attribute).Length > 0;

        public static bool AttributeContains<T>(this Module element) where T : Attribute => element.AttributeContains(typeof(T));
        public static bool AttributeContains(this Module element, Type attribute) => element.GetCustomAttributes(attribute, false).Length > 0;

        public static bool IsAsyncMethod(this MethodBase methodBase) => methodBase.AttributeContains<AsyncStateMachineAttribute>();

        public static bool IsCompilerGenerated(this Type type) => type.AttributeContains<CompilerGeneratedAttribute>();
        public static bool IsCompilerGenerated(this MemberInfo memberInfo) => memberInfo.AttributeContains<CompilerGeneratedAttribute>();
    }
}
