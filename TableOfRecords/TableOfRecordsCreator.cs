using System.Reflection;

namespace TableOfRecords;

/// <summary>
/// Presents method that write in table form to the text stream a set of elements of type T.
/// </summary>
public static class TableOfRecordsCreator
{
    /// <summary>
    /// Write in table form to the text stream a set of elements of type T (<see cref="ICollection{T}"/>),
    /// where the state of each object of type T is described by public properties that have only build-in
    /// type (int, char, string etc.)
    /// </summary>
    /// <typeparam name="T">Type selector.</typeparam>
    /// <param name="collection">Collection of elements of type T.</param>
    /// <param name="writer">Text stream.</param>
    /// <exception cref="ArgumentNullException">Throw if <paramref name="collection"/> is null.</exception>
    /// <exception cref="ArgumentNullException">Throw if <paramref name="writer"/> is null.</exception>
    /// <exception cref="ArgumentException">Throw if <paramref name="collection"/> is empty.</exception>
    public static void WriteTable<T>(ICollection<T>? collection, TextWriter? writer)
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(writer);

        if (collection.Count == 0)
        {
            throw new ArgumentException("Collection empty.", nameof(collection));
        }

        Type type = typeof(T);

        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => IsBuiltInType(p.PropertyType))
            .ToArray();

        var columnWidths = properties.ToDictionary(
            p => p.Name,
            p => Math.Max(p.Name.Length, GetMaxColumnWidth(collection, p)));

        string header = $"| {string.Join(" | ", properties.Select(p => p.Name.PadRight(columnWidths[p.Name])))} |";
        string separator = $"+{string.Join("+", properties.Select(p => new string('-', columnWidths[p.Name])).Select(w => w.PadLeft(w.Length + 2, '-')))}+";

        writer.WriteLine(separator);
        writer.WriteLine(header);
        writer.WriteLine(separator);

        foreach (T item in collection)
        {
            string row = $"| {string.Join(" | ", properties.Select(p =>
            {
                object? value = p.GetValue(item);
                string formattedValue = FormatValue(value);
                if (value is int || value is float || value is double || value is decimal || value is DateTime)
                {
                    return formattedValue.PadLeft(columnWidths[p.Name]);
                }
                else
                {
                    return formattedValue.PadRight(columnWidths[p.Name]);
                }
            }))} |";
            writer.WriteLine(row);
            writer.WriteLine(separator);
        }
    }

    private static bool IsBuiltInType(Type type)
    {
        return type == typeof(int) || type == typeof(float) || type == typeof(double) ||
               type == typeof(bool) || type == typeof(char) || type == typeof(string) ||
               type == typeof(decimal) || type == typeof(byte) || type == typeof(short) ||
               type == typeof(long) || type == typeof(uint) || type == typeof(ulong) ||
               type == typeof(ushort);
    }

    private static int GetMaxColumnWidth<T>(ICollection<T> collection, PropertyInfo property)
    {
        int maxWidth = collection.Max(item => FormatValue(property.GetValue(item)).Length);

        if (property.Name.Length > maxWidth)
        {
            maxWidth = property.Name.Length;
        }

        return maxWidth;
    }

    private static string FormatValue(object? value)
    {
        if (value == null)
        {
            return string.Empty;
        }

        return value.ToString()!;
    }
}
