using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Data.Objects;

namespace KeyHub.Common.Utils
{
    /// <summary>
    /// Class holding utils for reflection
    /// </summary>
    public static class Reflection
    {
        /// <summary>
        /// Returns wether the given type implements the interface
        /// </summary>
        /// <typeparam name="T">The interface type</typeparam>
        /// <param name="type">The type to check with</param>
        /// <returns>true if the given type implements the interface, otherwise false</returns>
        public static bool IsTypeWithInterface<T>(Type type) where T : class
        {
            return typeof(T).IsAssignableFrom(type);
        }

        /// <summary>
        /// Returns wether the given type implements the interface
        /// </summary>
        /// <param name="interfaceType">The interface type</param>
        /// <param name="type">The type to check with</param>
        /// <returns>true if the given type implements the interface, otherwise false</returns>
        public static bool IsTypeWithInterface(Type interfaceType, Type type)
        {
            return interfaceType.IsAssignableFrom(type);
        }

        /// <summary>
        /// Returns the attribute instance of the given class
        /// </summary>
        /// <typeparam name="TClass">The type of the class</typeparam>
        /// <typeparam name="TAttribute">The type of the attribute</typeparam>
        /// <returns>the instance of the attribute class if it exists, otherwise null</returns>
        public static TAttribute GetClassAttribute<TClass, TAttribute>()
        {
            return (TAttribute)typeof(TClass).GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault();
        }

        /// <summary>
        /// Returns wether a class has a attribute
        /// </summary>
        /// <typeparam name="TClass">The type of the class</typeparam>
        /// <typeparam name="TAttribute">The type of the attribute</typeparam>
        /// <returns>true if the class is marked with the attribute, otherwise false</returns>
        public static bool IsClassMarkedWithAttribute<TClass, TAttribute>()
        {
            return typeof(TClass).GetCustomAttributes(typeof(TAttribute), true).Length > 0;
        }

        /// <summary>
        /// Returns wether a class has a attribute
        /// </summary>
        /// <param name="classType">The type of the class</param>
        /// <param name="attributeType">The type of the attribute</param>
        /// <returns>true if the class is marked with the attribute, otherwise false</returns>
        public static bool IsClassMarkedWithAttribute(Type classType, Type attributeType)
        {
            if (classType == null)
                return false;

            if (attributeType == null)
                throw new ArgumentNullException("attributeType");

            return classType.GetCustomAttributes(attributeType, true).Length > 0;
        }

        /// <summary>
        /// Returns wether a class has a attribute
        /// </summary>
        /// <param name="classType">The type of the class as a string</param>
        /// <param name="attributeType">The type of the attribute</param>
        /// <returns>true if the class is marked with the attribute, otherwise false</returns>
        public static bool IsClassMarkedWithAttribute(string classType, Type attributeType)
        {
            return IsClassMarkedWithAttribute(Type.GetType(classType, false), attributeType);
        }

        /// <summary>
        /// Searches all loaded assemblies for classes marked with the attribute passed in.
        /// </summary>
        /// <param name="searchGAC">Wether to search the GAC</param>
        /// <typeparam name="TAttribute">The type of the attribute</typeparam>
        /// <returns>A list of appropriate types</returns>
        public static IEnumerable<Type> FindClassesMarkedWithAttribute<TAttribute>(bool searchGAC)
        {
            List<Type> typeList = new List<Type>();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                // skip if the assembly is part of the GAC
                if (!searchGAC && assembly.GlobalAssemblyCache)
                    continue;

                typeList.AddRange(FindClassesMarkedWithAttribute(assembly, typeof(TAttribute)));
            }

            return typeList.Distinct();
        }

        /// <summary>
        /// Searches all loaded assemblies for classes marked with the attribute passed in.
        /// </summary>
        /// <param name="attributeType">The type of the attribute</param>
        /// <param name="searchGAC">Wether to search the GAC</param>
        /// <returns>A list of appropriate types</returns>
        public static IEnumerable<Type> FindClassesMarkedWithAttribute(Type attributeType, bool searchGAC)
        {
            List<Type> typeList = new List<Type>();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                // skip if the assembly is part of the GAC
                if (!searchGAC && assembly.GlobalAssemblyCache)
                    continue;

                typeList.AddRange(FindClassesMarkedWithAttribute(assembly, attributeType));
            }
            return typeList.Distinct();
        }

        /// <summary>
        /// Retusn all type in the assembly that are marked with the attribute
        /// </summary>
        /// <param name="searchableAssembly">The assembly being searched</param>
        /// <param name="attributeType">The type of the attribute</param>
        /// <returns>A list of appropriate types</returns>
        public static IEnumerable<Type> FindClassesMarkedWithAttribute(Assembly searchableAssembly, Type attributeType)
        {
            try
            {
                // Get all types where the attribute is present
                return searchableAssembly.GetTypes().Where(type => type.GetCustomAttributes(attributeType, true).Length > 0);
            }
            catch (ReflectionTypeLoadException ex)
            {
                // return the types that were loaded, ignore those that could not be loaded
                return ex.Types;
            }
        }

        /// <summary>
        /// Gets all properties of a class that are marked with a specific attribute
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type</typeparam>
        /// <param name="classType">The class type</param>
        /// <returns>A dictiory of Properties and attributes</returns>
        public static Dictionary<PropertyInfo, TAttribute> GetClassPropertiesAndAttributes<TAttribute>(Type classType)
        {
            if (classType == null)
                throw new ArgumentNullException("classType");

            Dictionary<PropertyInfo, TAttribute> propertyDictionary = new Dictionary<PropertyInfo, TAttribute>();

            foreach (var propertyInfo in classType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var customAttributes = GetPropertyAttributes<TAttribute>(propertyInfo);
                if (customAttributes.Count() > 0)
                {
                    propertyDictionary.Add(propertyInfo, customAttributes.FirstOrDefault());
                }
            }

            return propertyDictionary;
        }

        /// <summary>
        /// Gets all attributes of <typeparamref name="TAttribute"/> from a property
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute</typeparam>
        /// <param name="propertyInfo">The property to search</param>
        /// <returns>A list of attributes</returns>
        public static IEnumerable<TAttribute> GetPropertyAttributes<TAttribute>(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            return propertyInfo.GetCustomAttributes(typeof(TAttribute), true).Cast<TAttribute>();
        }

        /// <summary>
        /// Gets all attributes of <paramref name="attributeType"/> from a property
        /// </summary>
        /// <param name="propertyInfo">The property to search</param>
        /// <param name="attributeType">The type of the attribute</param>
        /// <returns>A list of attributes</returns>
        public static IEnumerable<object> GetPropertyAttributes(PropertyInfo propertyInfo, Type attributeType)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            return propertyInfo.GetCustomAttributes(attributeType, true);
        }

        /// <summary>
        /// Creates a new instance of a typename
        /// </summary>
        /// <typeparam name="TType">The type of the return instance</typeparam>
        /// <param name="typeName">The full name (including assembly and namespaces) of the type to create</param>
        /// <returns>
        /// A new instance of the type if it is (or boxable to) <typeparamref name="TType"/>, 
        /// otherwise the default of <typeparamref name="TType"/>
        /// </returns>
        public static TType CreateTypeInstance<TType>(string typeName) where TType : class
        {
            Type classType = Type.GetType(typeName, false);

            if (classType == null)
                return default(TType);

            object newType = Activator.CreateInstance(classType);

            return newType as TType;
        }

        /// <summary>
        /// Returns the <c>System.Type</c> corresponding to the type string
        /// </summary>
        /// <param name="type">The input type string</param>
        /// <returns>A <c>System.Type</c> object if the type was found, otherwise null</returns>
        public static Type GetTypeFromString(string type)
        {
            return Type.GetType(type, false);
        }

        /// <summary>
        /// Determines wheter the object is actualy a proxy, or the real POCO class
        /// </summary>
        /// <param name="type">The object you want to check</param>
        /// <returns></returns>
        public static bool IsProxy(object type)
        {
            return type != null && ObjectContext.GetObjectType(type.GetType()) != type.GetType();
        }

        /// <summary>
        /// Returns the underlying type from a Proxy class
        /// </summary>
        /// <param name="type">The object to get the underlying type for</param>
        /// <returns>The underlying type of the proxy class</returns>
        public static Type GetProxyUnderlyingType(object type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return ObjectContext.GetObjectType(type.GetType());
        }

        /// <summary>
        /// Returns all the generic types from the underlying interface
        /// </summary>
        public static IEnumerable<Type> GetGenericParameters(object type)
        {
            return type.GetType()
                            .GetInterfaces()
                            .SelectMany(i => i.GetGenericArguments())
                            .ToArray();
        }
    }
}