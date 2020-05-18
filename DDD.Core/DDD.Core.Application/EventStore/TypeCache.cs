using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace DDD.Core.Application
{
    public class TypeCache
    {
        private ConcurrentDictionary<string, Type> _cache =
                            new ConcurrentDictionary<string, Type>();

        public Type FindType(string typeName)
        {
            if (!_cache.TryGetValue(typeName, out Type resultType))
            {
                resultType = FindUncachedType(typeName);
                _cache[typeName] = resultType;
            }
            return resultType;
        }

        private Type FindUncachedType(string typeName)
        {
            Type result = Type.GetType(typeName);
            if (result == null)
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    result = assembly.GetType(typeName);
                    if (result != null) break;
                }
            }
            return result;
        }
    }
}
