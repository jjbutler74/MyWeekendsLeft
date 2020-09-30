using System;
using System.Collections.Generic;
using System.Text;
using MWL.Models;

namespace MWL.Services.Interface
{
    public interface ICountriesService
    {
        Dictionary<string, string> GetCountryData();
    }
}
