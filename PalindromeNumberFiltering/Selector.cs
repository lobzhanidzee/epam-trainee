using System.Collections.Concurrent;
using System.Globalization;

namespace PalindromeNumberFiltering;

/// <summary>
/// A static class containing methods for filtering palindrome numbers from a collection of integers.
/// </summary>
public static class Selector
{
    /// <summary>
    /// Retrieves a collection of palindrome numbers from the given list of integers using concurrent filtering.
    /// </summary>
    /// <param name="numbers">The list of integers to filter.</param>
    /// <returns>A collection of palindrome numbers.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the input list 'numbers' is null.</exception>
    public static IList<int> GetPalindromes(IList<int> numbers)
    {
        ArgumentNullException.ThrowIfNull(numbers);

        ConcurrentBag<int> newNumbers = [];

        _ = Parallel.ForEach(numbers, number =>
        {
            if (IsPalindrome(number))
            {
                newNumbers.Add(number);
            }
        });

        return newNumbers.ToList();
    }

    /// <summary>
    /// Checks whether the given integer is a palindrome number.
    /// </summary>
    /// <param name="number">The integer to check.</param>
    /// <returns>True if the number is a palindrome, otherwise false.</returns>
    private static bool IsPalindrome(int number)
    {
        return IsPositiveNumberPalindrome(number, 0, GetLength(number) - 1);
    }

    /// <summary>
    /// Recursively checks whether a positive number is a palindrome.
    /// </summary>
    /// <param name="number">The positive number to check.</param>
    /// <param name="left">The index of the leftmost digit to compare.</param>
    /// <param name="right">The index of the rightmost digit to compare.</param>
    /// <returns>True if the positive number is a palindrome, otherwise false.</returns>
    private static bool IsPositiveNumberPalindrome(int number, int left, int right)
    {
        if (left >= right)
        {
            return true;
        }

        if (GetDigitInDecimalPlace(number, left) != GetDigitInDecimalPlace(number, right))
        {
            return false;
        }

        return IsPositiveNumberPalindrome(number, left + 1, right - 1);
    }

    /// <summary>
    /// Retrieves the digit at a specified decimal place in a given number.
    /// </summary>
    /// <param name="number">The number from which to retrieve the digit.</param>
    /// <param name="decimalPlace">The decimal place (index) of the desired digit, starting from the rightmost digit (ones place).</param>
    /// <returns>The digit at the specified decimal place.</returns>
    private static int GetDigitInDecimalPlace(int number, int decimalPlace)
    {
        string numberText = number.ToString(CultureInfo.InvariantCulture);
        return numberText[decimalPlace] - '0';
    }

    /// <summary>
    /// Gets the number of digits in the given integer.
    /// </summary>
    /// <param name="number">The integer to count digits for.</param>
    /// <returns>The number of digits in the integer.</returns>
    private static byte GetLength(int number)
    {
        return (byte)number.ToString(CultureInfo.InvariantCulture).Length;
    }
}
