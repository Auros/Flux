using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Elysium
{
    /// <summary>
    /// Central place to handle all the reflection related stuff.
    /// Putting it all in one place to easier implement other ways of doing things in the future like CodeGen
    /// </summary>
    internal static class ElysiumInvoker
    {
        private static readonly Dictionary<Type, MemberContext> _memberContexts = new();

        private static MemberContext? GetMemberContext(Type? type)
        {
            if (type == null)
                return null;
            
            if (!_memberContexts.TryGetValue(type, out var ctx))
                ctx = _memberContexts[type] = new MemberContext(type);
            return ctx;
        }

        public static void ParameterlessMethod(object? host, string methodName)
            => GetMemberContext(host?.GetType())?.InvokeParameterlessMethod(host, methodName);

        public static object? GetPropertyValue(object? host, string propertyName)
            => GetMemberContext(host?.GetType())?.FetchPropertyValue(host, propertyName);
        
        public static void SetPropertyValue(object? host, string propertyName, object value)
            => GetMemberContext(host?.GetType())?.ApplyPropertyValue(host, propertyName, value);

        public static string[] GetPropertyNamesOfType<T>(Type? source)
            => GetMemberContext(source ?? typeof(ElysiumInvoker))!.FetchPropertyNamesOfType<T>();
        
        /// <summary>
        /// A wrapper for the common reflection calls we need. Caches properties for better performance.
        /// </summary>
        private class MemberContext
        {
            private readonly Type _type;
            private readonly Dictionary<string, MethodInfo?> _methods = new();
            private readonly Dictionary<string, MethodInfo?> _getters = new();
            private readonly Dictionary<string, MethodInfo?> _setters = new();
            private readonly Dictionary<Type, string[]> _propertiesOfType = new();

            public MemberContext(Type type)
            {
                _type = type;
            }
            
            public void InvokeParameterlessMethod(object? host, string methodName)
            {
                if (!_methods.TryGetValue(methodName, out var method))
                {
                    method = _type.GetMethod(methodName);
                    if (method is null || method.GetParameters().Length != 0)
                        method = null;
                    _methods[methodName] = method;
                }
                method?.Invoke(host, Array.Empty<object>());
            }

            public object? FetchPropertyValue(object? host, string propertyName)
            {
                if (_getters.TryGetValue(propertyName, out var method))
                    return method?.Invoke(host, Array.Empty<object>());
                
                method = _type.GetProperty(propertyName)?.GetMethod;
                _getters[propertyName] = method;
                return method?.Invoke(host, Array.Empty<object>());
            }
            
            public void ApplyPropertyValue(object? host, string propertyName, object value)
            {
                if (!_setters.TryGetValue(propertyName, out var method))
                {
                    method = _type.GetProperty(propertyName)?.SetMethod;
                    _setters[propertyName] = method;
                }
                method?.Invoke(host, new [] { value });
            }

            public string[] FetchPropertyNamesOfType<T>()
            {
                if (!_propertiesOfType.TryGetValue(typeof(T), out var properties))
                    properties = _propertiesOfType[typeof(T)] = _type
                        .GetProperties()
                        .Where(p => p.PropertyType.IsAssignableFrom(typeof(T)))
                        .Select(p => p.Name)
                        .ToArray();
                return properties;
            }
        }
    }
}