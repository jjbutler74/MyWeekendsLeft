using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using CsvHelper;
using MWL.Models.Entities;
using Microsoft.Extensions.Caching.Memory;
using MWL.Services.Interface;

namespace MWL.Services.Implementation
{
    public class CountriesService : ICountriesService
    {
        private IMemoryCache _cache;

        public CountriesService(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        public Dictionary<string, string> GetCountryData()
        {
            // Look for CountryData key
            if (!_cache.TryGetValue("CountryData", out Dictionary<string, string> countries))
            {
                // Key not in cache, so get data
                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MWL.Services.countries.csv");
                if (stream == null)
                {
                    // TODO: log this error
                    throw new NullReferenceException("'stream' object is null.");
                }
                using var reader = new StreamReader(stream); // data from https://population.io/data/countries.csv)
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                var csvRecords = csv.GetRecords<CountryCsvFormat>().ToList();
                
                countries = new Dictionary<string, string>();
                foreach (var csvRecord in csvRecords)
                { 
                    countries.Add(csvRecord.GMI_CNTRY, csvRecord.POPIO_NAME);
                }
              
                // Set cache options AND keep in cache for this time, reset time if accessed
                var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(1));

                // Save data in cache.
                _cache.Set("CountryData", countries, cacheOptions);
            }

            return countries;
        }
    }
}