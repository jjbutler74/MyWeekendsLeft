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

        public Dictionary<string, string> GetCountryData()
        {
            // Look for CountryData key
            if (!_cache.TryGetValue("CountryData", out Dictionary<string, string> countries))
            {
                // Key not in cache, so get data
                using var reader = new StreamReader("countries.csv"); // From https://population.io/data/countries.csv
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