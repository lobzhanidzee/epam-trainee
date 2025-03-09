using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Calculations.ConsoleClient
{
    internal static class Program
    {
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <summary>
        /// Calculates the sum from 1 to n synchronously.
        /// </summary>
        /// <param name="n">The last number in the sum.</param>
        /// <returns>A sum: 1 + 2 + ... + n.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Throw if n less or equals zero.</exception>
        public static long CalculateSum(int n)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(n);

            long sum = 0;
            for (int i = 1; i <= n; i++)
            {
                sum += i;
            }

            return sum;
        }

        /// <summary>
        /// Calculates the sum from 1 to n asynchronously.
        /// </summary>
        /// <param name="n">The last number in the sum.</param>
        /// <param name="token">The cancellation token for the cancellation of the asynchronous operation.</param>
        /// <param name="progress">Presents current status of the asynchronous operation in form of the current value of sum and index.</param>
        /// <returns>A task that represents the asynchronous sum: 1 + 2 + ... + n.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Throw if n less or equals zero.</exception>
        public static async Task<long> CalculateSumAsync(int n, CancellationToken token, IProgress<(int, long)>? progress = null)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(n);

            long sum = 0;

            for (int i = 1; i <= n; i++)
            {
                sum += i;

                progress?.Report((i, sum));
                token.ThrowIfCancellationRequested();

                await Task.Yield();
            }

            return sum;
        }

        /// <summary>
        /// Calculates the sum from 1 to n synchronously.
        /// The value of n is set by the user from the console.
        /// The user can change the boundary n during the calculation, which causes the calculation to be restarted,
        /// this should not crash the application.
        /// After receiving the result, be able to continue calculations without leaving the console.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task Main()
        {
            while (true)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();

                Console.WriteLine("Enter the value of n (or 'exit' to quit):");
                var input = Console.ReadLine();

                if (input?.ToLower(CultureInfo.InvariantCulture) == "exit")
                {
                    break;
                }

                if (int.TryParse(input, out int n) && n > 0)
                {
                    using (var cancelTokenSource = new CancellationTokenSource())
                    {
                        var progress = new Progress<(int, long)>(p => Console.WriteLine($"Progress: {p.Item1}/{n}, Sum: {p.Item2}"));

                        try
                        {
                            var calculationTask = CalculateSumAsync(n, cancelTokenSource.Token, progress);
                            var result = await calculationTask;
                            watch.Stop();
                            Console.WriteLine($"Final Sum: {result}. Time {watch}");
                        }
                        catch (OperationCanceledException)
                        {
                            Console.WriteLine("Calculation was canceled.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a positive integer.");
                }
            }
        }
    }
}
