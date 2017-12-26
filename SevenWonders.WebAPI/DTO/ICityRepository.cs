using SevenWonders.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SevenWonders.WebAPI.DTO
{
    public interface ICityRepository : IDisposable
    {
        IEnumerable<City> GetCities();
        City GetCityByID(int CityId);
        void InsertCity(City City);
        void DeleteCity(int CityID);
        void UpdateCity(City City);
        void Save();
    }
}