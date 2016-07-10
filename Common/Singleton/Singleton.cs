using System;
using System.Collections;
using System.Linq;

namespace Common.Singleton
{
    public static class Singleton
    {
        static Hashtable ObjectList = new Hashtable();
        static Object Sync = new Object();

        public static T GetInstance<T>() where T : class
        {
            var typeName = typeof(T).FullName;

            lock (Sync)
            {
                if (ObjectList.ContainsKey(typeName))
                    return (T)ObjectList[typeName];
            }

            var constructorInfo = typeof(T).GetConstructor(Type.EmptyTypes);
            if (constructorInfo == null)
            {
                var paramCount = typeof(T).GetConstructors().Min(construct => construct.GetParameters().Count());
                var constructorToUse = typeof(T).GetConstructors().Where(construct => construct.GetParameters().Count() == paramCount).First();

                var paramNullList = new object[paramCount];

                var parameters = constructorToUse.GetParameters();
                for (int i = 0; i < paramCount; i++)
                {
                    paramNullList[i] = parameters[i].ParameterType.IsValueType ? Activator.CreateInstance(parameters[i].ParameterType) : null;
                }

                T instance = (T)Activator.CreateInstance(typeof(T), paramNullList);
                ObjectList.Add(instance.ToString(), instance);
                return (T)ObjectList[typeName];
            }
            else
            {
                T instance = (T)constructorInfo.Invoke(new object[0]);
                if (!ObjectList.ContainsKey(instance.ToString()))
                    ObjectList.Add(instance.ToString(), instance);
                return (T)ObjectList[typeName];
            }
        }
    }
}
