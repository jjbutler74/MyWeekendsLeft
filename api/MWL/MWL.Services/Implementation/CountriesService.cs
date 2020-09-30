using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using CsvHelper;
using MWL.Models.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace MWL.Services.Implementation
{
    public class CountriesService
    {
        private IMemoryCache _cache;

        public CountriesService(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        public IList<string> GetCountryData()
        {
            // Look for CountryData key
            if (!_cache.TryGetValue("CountryData", out List<string> countries))
            {
                // Key not in cache, so get data
                using var reader = new StreamReader("countries.csv"); // From https://population.io/data/countries.csv
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                var csvRecords = csv.GetRecords<CountryFromFile>().ToList();
                countries = csvRecords.Select(csvRecord => csvRecord.POPIO_NAME).ToList();

                // Set cache options AND keep in cache for this time, reset time if accessed
                var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(1));

                // Save data in cache.
                _cache.Set("CountryData", countries, cacheOptions);
            }

            return countries;
        }
    }
}