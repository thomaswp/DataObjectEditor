using ObjectEditor.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ObjectEditor.Editor.Reflect
{
    public static class ReflectionFieldAdder
    {
        private static Dictionary<Type, MethodInfo> primitiveFieldDataAddMethods = new Dictionary<Type, MethodInfo>();
        private static List<MethodInfo> genericFieldDataAddMethods = new List<MethodInfo>();
        static ReflectionFieldAdder()
        {
            foreach (MethodInfo info in typeof(FieldData).GetMethods())
            {
                if (info.Name.StartsWith("add") && info.GetParameters().Length == 2 &&
                    info.ReturnType == info.GetParameters()[0].ParameterType)
                {
                    if (info.IsGenericMethod)
                    {
                        genericFieldDataAddMethods.Add(info);
                    }
                    else
                    {
                        primitiveFieldDataAddMethods.Add(info.ReturnType, info);
                    }
                    //Console.WriteLine("Method: " + info.ReturnType.Name + " => " + info.Name);
                }
            }
        }

        public static void addFields(DataObject dataObject, FieldData fields)
        {
            List<Accessor> accessors = FieldAccessor.GetForObject(dataObject).ToList();
            accessors.AddRange(PropertyAccessor.GetForObject(dataObject));
            foreach (Accessor accessor in accessors)
            {
                Type type = accessor.GetAccessorType();
                string name = accessor.GetName();
                object[] parameters = new object[] { accessor.Get(), name };
                if (primitiveFieldDataAddMethods.ContainsKey(type))
                {
                    if (accessor.IsReadOnly())
                    {
                        Console.WriteLine("Cannot write to readonle field: " + name);
                    }
                    MethodInfo info = primitiveFieldDataAddMethods[type];
                    //if (fields.writeMode())
                    //{
                    //    Console.WriteLine("Write: " + name + " = " + accessor.Get());

                    //}
                    accessor.Set(info.Invoke(fields, parameters));
                    //if (fields.readMode())
                    //{
                    //    Console.WriteLine("Read: " + name + " = " + accessor.Get());
                    //}
                }
                else
                {
                    Type genericType = null;
                    string methodName = null;
                    if (type.IsGenericType)
                    {
                        genericType = type.GetGenericArguments()[0];
                        if (type.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            methodName = typeof(DataObject).IsAssignableFrom(genericType) ? "addList" : "addPrimitiveList";
                        }
                        else if (type.GetGenericTypeDefinition() == typeof(Reference<>))
                        {
                            methodName = "addReference";
                        }
                    }
                    else
                    {
                        genericType = type;
                        if (typeof(DataObject).IsAssignableFrom(genericType))
                        {
                            methodName = "add";
                        }
                        else if (typeof(Enum).IsAssignableFrom(genericType))
                        {
                            methodName = "addEnum";
                        }
                    }
                    if (methodName != null)
                    {
                        MethodInfo info = genericFieldDataAddMethods.Where((i) => i.Name == methodName).First();
                        var value = info.MakeGenericMethod(genericType).Invoke(fields, parameters);
                        if (!accessor.IsReadOnly()) accessor.Set(value);
                    }
                    else
                    {
                        // TODO: dictionaries, primitive arrays
                        Console.WriteLine("No read/write method for: " + type.Name);
                    }
                }
            }
        }
    }
}
