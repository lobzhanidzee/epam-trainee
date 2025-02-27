using System.Collections.Concurrent;
using System.Globalization;

namespace PalindromeNumberFiltering
{
    /// <summary>
    /// A static class containing methods for filtering palindrome numbers from a collection of integers.
    /// </summary>
    public static class Selector
    {
        /// <summary>
        /// Retrieves a collection of palindrome numbers from the given list of integers using sequential filtering.
        /// </summary>
        /// <param name="numbers">The list of integers to filter.</param>
        /// <returns>A collection of palindrome numbers.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the input list 'numbers' is null.</exception>
        public static IList<int> GetPalindromeInSequence(IList<int>? numbers)
        {
            ArgumentNullException.ThrowIfNull(numbers);

            List<int> filteredArr = new List<int>();

            foreach (var num in numbers)
            {
                if (IsPalindrome(num))
                {
                    filteredArr.Add(num);
                }
            }

            return filteredArr;
        }

        /// <summary>
        /// Retrieves a collection of palindrome numbers from the given list of integers using parallel filtering.
        /// </summary>
        /// <param name="numbers">The list of integers to filter.</param>
        /// <returns>A collection of palindrome numbers.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the input list 'numbers' is null.</exception>
        public static IList<int> GetPalindromeInParallel(IList<int> numbers)
        {
            ArgumentNullException.ThrowIfNull(numbers);

            ConcurrentBag<int> filteredArr = new ConcurrentBag<int>();

            _ = Parallel.ForEach(numbers, num =>
            {
                if (IsPalindrome(num))
                {
                    filteredArr.Add(num);
                }
            });

            return filteredArr.ToList();
        }

        /// <summary>
        /// Checks whether the given integer is a palindrome number.
        /// </summary>
        /// <param name="number">The integer to check.</param>
        /// <returns>True if the number is a palindrome, otherwise false.</returns>
        private static bool IsPalindrome(int number)
        {
            if (number < 9 && number >= 0)
            {
                return true;
            }

            if (number < 0)
            {
                return false;
            }

            int divider = 1;
            while (number / divider >= 10)
            {
                divider *= 10;
            }

            return IsPositiveNumberPalindrome(number, divider);
        }

        /// <summary>
        /// Recursively checks whether a positive number is a palindrome.
        /// </summary>
        /// <param name="number">The positive number to check.</param>
        /// <param name="divider">The divider used in the recursive check.</param>
        /// <returns>True if the positive number is a palindrome, otherwise false.</returns>
        private static bool IsPositiveNumberPalindrome(int number, int divider)
        {
            if (GetLength(number) == 1)
            {
                return true;
            }

            int mostSignificantDigit = number / divider;
            int leastSignificantDigit = number % 10;

            if (mostSignificantDigit != leastSignificantDigit)
            {
                return false;
            }

            number %= divider;
            number /= 10;
            divider /= 100;

            return IsPositiveNumberPalindrome(number, divider);
        }

        /// <summary>
        /// Gets the number of digits in the given integer.
        /// </summary>
        /// <param name="number">The integer to count digits for.</param>
        /// <returns>The number of digits in the integer.</returns>
        private static byte GetLength(int number)
        {
            int numberLength = number.ToString(CultureInfo.InvariantCulture).Length;
            return (byte)numberLength;
        }
    }
}
