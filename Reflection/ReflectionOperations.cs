using System.Reflection;

[assembly: CLSCompliant(true)]

namespace Reflection
{
    public static class ReflectionOperations
    {
        public static string GetTypeName(object obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            Type type = obj.GetType();
            return type.Name;
        }

        public static string GetFullTypeName<T>()
        {
            Type type = typeof(T);
            return type.FullName!;
        }

        public static string GetAssemblyQualifiedName<T>()
        {
            Type type = typeof(T);
            return type.AssemblyQualifiedName!;
        }

        public static string[] GetPrivateInstanceFields(object obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            Type type = obj.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

            return fields.Select(field => field.Name).ToArray();
        }

        public static string[] GetPublicStaticFields(object obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            Type type = obj.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);

            return fields.Select(field => field.Name).ToArray();
        }

        public static string?[] GetInterfaceDataDetails(object obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            Type type = obj.GetType();
            Type[] interfaces = type.GetInterfaces();

            return interfaces.Select(i => i.FullName).ToArray();
        }

        public static string?[] GetConstructorsDataDetails(object obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            Type type = obj.GetType();
            ConstructorInfo[] constructors = type.GetConstructors();

            return constructors.Select(i => i.ToString()).ToArray();
        }

        public static string?[] GetTypeMembersDataDetails(object obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            Type type = obj.GetType();
            MemberInfo[] members = type.GetMembers();

            return members.Select(i => i.ToString()).ToArray();
        }

        public static string?[] GetMethodDataDetails(object obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            Type type = obj.GetType();
            MemberInfo[] members = type.GetMethods();

            return members.Select(i => i.ToString()).ToArray();
        }

        public static string?[] GetPropertiesDataDetails(object obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            Type type = obj.GetType();
            MemberInfo[] members = type.GetProperties();

            return members.Select(i => i.ToString()).ToArray();
        }
    }
}
