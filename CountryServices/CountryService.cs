using System.Net;
using System.Text.Json;

namespace CountryServices;

/// <summary>
/// Provides information about country local currency from RESTful API
/// <see><cref>https://restcountries.com/#api-endpoints-v2</cref></see>.
/// </summary>
public class CountryService : ICountryService
{
    private const string ServiceUrl = "https://restcountries.com/v2";
    private static readonly HttpClient Client = new HttpClient();

    private readonly Dictionary<string, WeakReference<LocalCurrency>> currencyCountries = [];

    /// <summary>
    /// Gets information about currency by country code synchronously.
    /// </summary>
    /// <param name="alpha2Or3Code">ISO 3166-1 2-letter or 3-letter country code.</param>
    /// <see><cref>https://en.wikipedia.org/wiki/List_of_ISO_3166_country_codes</cref></see>
    /// <returns>Information about country currency as <see cref="LocalCurrency"/>>.</returns>
    /// <exception cref="ArgumentException">Throw if countryCode is null, empty, whitespace or invalid country code.</exception>
    public LocalCurrency GetLocalCurrencyByAlpha2Or3Code(string? alpha2Or3Code)
    {
        using WebClient client = new WebClient();
        try
        {
            if (alpha2Or3Code == null || alpha2Or3Code.Length != 2)
            {
                throw new ArgumentException("invalid code", nameof(alpha2Or3Code));
            }

            if (this.currencyCountries.TryGetValue(alpha2Or3Code, out var weakReference) && weakReference.TryGetTarget(out var cachedCurrency))
            {
                return cachedCurrency;
            }

            var response = client.DownloadString($"{ServiceUrl}/alpha/{alpha2Or3Code}");

            LocalCurrencyInfo? localCurrencyInfo = JsonSerializer.Deserialize<LocalCurrencyInfo>(response);
            LocalCurrency? localCurrency = new LocalCurrency
            {
                CountryName = localCurrencyInfo?.CountryName,
                CurrencyCode = localCurrencyInfo?.Currencies?[0].Code,
                CurrencySymbol = localCurrencyInfo?.Currencies?[0].Symbol,
            };

            this.currencyCountries[alpha2Or3Code] = new WeakReference<LocalCurrency>(localCurrency);

            return localCurrency;
        }
        catch (NullReferenceException e)
        {
            throw new ArgumentException(nameof(e));
        }
    }

    /// <summary>
    /// Gets information about currency by country code asynchronously.
    /// </summary>
    /// <param name="alpha2Or3Code">ISO 3166-1 2-letter or 3-letter country code.</param>
    /// <see><cref>https://en.wikipedia.org/wiki/List_of_ISO_3166_country_codes</cref></see>.
    /// <param name="token">Token for cancellation asynchronous operation.</param>
    /// <returns>Information about country currency as <see cref="LocalCurrency"/>>.</returns>
    /// <exception cref="ArgumentException">Throw if countryCode is null, empty, whitespace or invalid country code.</exception>
    public async Task<LocalCurrency> GetLocalCurrencyByAlpha2Or3CodeAsync(string? alpha2Or3Code, CancellationToken token)
    {
        try
        {
            if (alpha2Or3Code == null || alpha2Or3Code.Length != 2)
            {
                throw new ArgumentException("invalid code", nameof(alpha2Or3Code));
            }

            if (this.currencyCountries.TryGetValue(alpha2Or3Code, out var weakReference) && weakReference.TryGetTarget(out var cachedCurrency))
            {
                return cachedCurrency;
            }

            var response = await Client.GetStringAsync(new Uri($"{ServiceUrl}/alpha/{alpha2Or3Code}"), token);

            LocalCurrencyInfo? localCurrencyInfo = JsonSerializer.Deserialize<LocalCurrencyInfo>(response);
            LocalCurrency? localCurrency = new LocalCurrency
            {
                CountryName = localCurrencyInfo?.CountryName,
                CurrencyCode = localCurrencyInfo?.Currencies?[0].Code,
                CurrencySymbol = localCurrencyInfo?.Currencies?[0].Symbol,
            };

            this.currencyCountries[alpha2Or3Code] = new WeakReference<LocalCurrency>(localCurrency);

            return localCurrency;
        }
        catch (NullReferenceException)
        {
            throw new ArgumentException("Invalid code");
        }
    }

    /// <summary>
    /// Gets information about the country by the country capital synchronously.
    /// </summary>
    /// <param name="capital">Capital name.</param>
    /// <returns>Information about the country as <see cref="Country"/>>.</returns>
    /// <exception cref="ArgumentException">Throw if the capital name is null, empty, whitespace or nonexistent.</exception>
    public Country GetCountryInfoByCapital(string? capital)
    {
        using WebClient client = new WebClient();
        try
        {
            if (capital == null)
            {
                throw new ArgumentException("invalid capital", nameof(capital));
            }

            ArgumentException.ThrowIfNullOrWhiteSpace(capital);

            var response = client.DownloadString($"{ServiceUrl}/capital/{capital}");

            var countryInfos = JsonSerializer.Deserialize<List<CountryInfo>>(response);
            var countryInfo = countryInfos?.FirstOrDefault();

            ArgumentNullException.ThrowIfNull(countryInfo);

            Country country = new Country
            {
                Name = countryInfo.Name,
                CapitalName = countryInfo.CapitalName,
                Area = countryInfo.Area,
                Population = countryInfo.Population,
                Flag = countryInfo.Flag,
            };

            return country;
        }
        catch (NullReferenceException)
        {
            throw new ArgumentException("Invalid capital");
        }
        catch (WebException)
        {
            throw new ArgumentException("Invalid capital");
        }
    }

    /// <summary>
    /// Gets information about the currency by the country capital asynchronously.
    /// </summary>
    /// <param name="capital">Capital name.</param>
    /// <param name="token">Token for cancellation asynchronous operation.</param>
    /// <returns>Information about the country as <see cref="Country"/>>.</returns>
    /// <exception cref="ArgumentException">Throw if the capital name is null, empty, whitespace or nonexistent.</exception>
    public async Task<Country> GetCountryInfoByCapitalAsync(string? capital, CancellationToken token)
    {
        try
        {
            if (capital == null)
            {
                throw new ArgumentException("invalid capital", nameof(capital));
            }

            ArgumentException.ThrowIfNullOrWhiteSpace(capital);

            var response = await Client.GetStringAsync(new Uri($"{ServiceUrl}/capital/{capital}"), token);
            var countryInfos = JsonSerializer.Deserialize<List<CountryInfo>>(response);
            var countryInfo = countryInfos?.FirstOrDefault();

            ArgumentNullException.ThrowIfNull(countryInfo);

            Country country = new Country
            {
                Name = countryInfo.Name,
                CapitalName = countryInfo.CapitalName,
                Area = countryInfo.Area,
                Population = countryInfo.Population,
                Flag = countryInfo.Flag,
            };

            return country;
        }
        catch (NullReferenceException)
        {
            throw new ArgumentException("Invalid capital");
        }
        catch (WebException)
        {
            throw new ArgumentException("Invalid capital");
        }
        catch (HttpRequestException)
        {
            throw new ArgumentException("error");
        }
    }
}
