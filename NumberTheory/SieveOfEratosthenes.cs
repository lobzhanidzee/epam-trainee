using System.Collections.Concurrent;

namespace NumberTheory;

public static class SieveOfEratosthenes
{
    /// <summary>
    /// Generates a sequence of prime numbers up to the specified limit using a sequential approach.
    /// </summary>
    /// <param name="n">The upper limit for generating prime numbers.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing prime numbers up to the specified limit.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the input <paramref name="n"/> is less than or equal to 0.</exception>
    public static IEnumerable<int> GetPrimeNumbersSequentialAlgorithm(int n)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(n, 0);

        List<int> primes = [];

        for (int i = 2; i <= n; i++)
        {
            bool isPrime = true;
            int limit = (int)Math.Sqrt(i);

            for (int j = 2; j <= limit; j++)
            {
                if (i % j == 0)
                {
                    isPrime = false;
                    break;
                }
            }

            if (isPrime)
            {
                primes.Add(i);
            }
        }

        return primes;
    }

    /// <summary>
    /// Generates a sequence of prime numbers up to the specified limit using a modified sequential approach.
    /// </summary>
    /// <param name="n">The upper limit for generating prime numbers.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing prime numbers up to the specified limit.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the input <paramref name="n"/> is less than or equal to 0.</exception>
    ///
    public static IEnumerable<int> GetPrimeNumbersModifiedSequentialAlgorithm(int n)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(n, 0);

        object locker = new();
        List<int> primes = [];
        int sqrtN = (int)Math.Sqrt(n);

        bool[] isPrime = new bool[sqrtN + 1];
        for (int i = 2; i <= sqrtN; i++)
        {
            isPrime[i] = true;
        }

        for (int i = 2; i <= sqrtN; i++)
        {
            if (isPrime[i])
            {
                lock (locker)
                {
                    primes.Add(i);
                }

                for (int j = i * i; j <= sqrtN; j += i)
                {
                    isPrime[j] = false;
                }
            }
        }

        Task.Run(() =>
        {
            for (int i = sqrtN + 1; i <= n; i++)
            {
                bool isPrimeNumber = true;
                foreach (int prime in primes)
                {
                    if (prime * prime > i)
                    {
                        break;
                    }

                    if (i % prime == 0)
                    {
                        isPrimeNumber = false;
                        break;
                    }
                }

                if (isPrimeNumber)
                {
                    lock (locker)
                    {
                        primes.Add(i);
                    }
                }
            }
        }).Wait();

        return primes;
    }

    /// <summary>
    /// Generates a sequence of prime numbers up to the specified limit using a concurrent approach by data decomposition.
    /// </summary>
    /// <param name="n">The upper limit for generating prime numbers.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing prime numbers up to the specified limit.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the input <paramref name="n"/> is less than or equal to 0.</exception>
    public static IEnumerable<int> GetPrimeNumbersConcurrentDataDecomposition(int n)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(n, 0);

        List<int> primes = [];
        int sqrtN = (int)Math.Sqrt(n);

        bool[] isPrime = new bool[sqrtN + 1];
        for (int i = 2; i <= sqrtN; i++)
        {
            isPrime[i] = true;
        }

        for (int i = 2; i <= sqrtN; i++)
        {
            if (isPrime[i])
            {
                primes.Add(i);
                for (int j = i * i; j <= sqrtN; j += i)
                {
                    isPrime[j] = false;
                }
            }
        }

        ConcurrentBag<int> primeBag = new(primes);

        _ = Parallel.For(sqrtN + 1, n + 1, i =>
        {
            bool isPrimeNumber = true;
            foreach (int prime in primes)
            {
                if (prime * prime > i)
                {
                    break;
                }

                if (i % prime == 0)
                {
                    isPrimeNumber = false;
                    break;
                }
            }

            if (isPrimeNumber)
            {
                primeBag.Add(i);
            }
        });

        return primeBag.OrderBy(x => x);
    }

    /// <summary>
    /// Generates a sequence of prime numbers up to the specified limit using a concurrent approach by "basic" primes decomposition.
    /// </summary>
    /// <param name="n">The upper limit for generating prime numbers.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing prime numbers up to the specified limit.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the input <paramref name="n"/> is less than or equal to 0.</exception>
    public static IEnumerable<int> GetPrimeNumbersConcurrentBasicPrimesDecomposition(int n)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(n, 0);

        List<int> primes = [];
        int sqrtN = (int)Math.Sqrt(n);

        bool[] isPrime = new bool[sqrtN + 1];
        for (int i = 2; i <= sqrtN; i++)
        {
            isPrime[i] = true;
        }

        for (int i = 2; i <= sqrtN; i++)
        {
            if (isPrime[i])
            {
                primes.Add(i);
                for (int j = i * i; j <= sqrtN; j += i)
                {
                    isPrime[j] = false;
                }
            }
        }

        int numThreads = Environment.ProcessorCount;
        int range = (n - sqrtN) / numThreads;
        List<Task> tasks = [];
        object locker = new();

        for (int t = 0; t < numThreads; t++)
        {
            int start = sqrtN + 1 + (t * range);
            int end = (t == numThreads - 1) ? n : start + range - 1;

            tasks.Add(Task.Run(() =>
            {
                List<int> localPrimes = [];
                List<int> primesSnapshot;

                lock (locker)
                {
                    primesSnapshot = primes.ToList();
                }

                for (int i = start; i <= end; i++)
                {
                    bool isPrimeNumber = true;

                    foreach (int prime in primesSnapshot)
                    {
                        if (prime * prime > i)
                        {
                            break;
                        }

                        if (i % prime == 0)
                        {
                            isPrimeNumber = false;
                            break;
                        }
                    }

                    if (isPrimeNumber)
                    {
                        localPrimes.Add(i);
                    }
                }

                lock (locker)
                {
                    primes.AddRange(localPrimes);
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());

        return primes.OrderBy(p => p).ToList();
    }

    /// <summary>
    /// Generates a sequence of prime numbers up to the specified limit using thread pool and signaling construct.
    /// </summary>
    /// <param name="n">The upper limit for generating prime numbers.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing prime numbers up to the specified limit.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the input <paramref name="n"/> is less than or equal to 0.</exception>
    public static IEnumerable<int> GetPrimeNumbersConcurrentWithThreadPool(int n)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(n, 0);

        List<int> basePrimes = GetPrimeNumbersSequentialAlgorithm((int)Math.Sqrt(n)).ToList();
        ConcurrentBag<int> primes = new(basePrimes);

        int rangeStart = (int)Math.Sqrt(n) + 1;
        int numThreads = Environment.ProcessorCount;
        int rangeSize = ((n - rangeStart) / numThreads) + 1;

        CountdownEvent countdown = new(numThreads);

        for (int i = 0; i < numThreads; i++)
        {
            int start = rangeStart + (i * rangeSize);
            int end = Math.Min(start + rangeSize - 1, n);

            _ = ThreadPool.QueueUserWorkItem(
                state =>
            {
                for (int num = start; num <= end; num++)
                {
                    bool isPrime = true;
                    foreach (int prime in basePrimes)
                    {
                        if (num % prime == 0)
                        {
                            isPrime = false;
                            break;
                        }
                    }

                    if (isPrime)
                    {
                        primes.Add(num);
                    }
                }

                _ = countdown.Signal();
            });
        }

        countdown.Wait();

        return primes.OrderBy(x => x);
    }
}
