using SevenWonders.DAL.Context;
using SevenWonders.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SevenWonders.WebAPI.DTO
{
    public class CityRepository: ICityRepository
    {
        SevenWondersContext context;
        public CityRepository()
        {
            context = new SevenWondersContext();
        }

        public CityRepository(SevenWondersContext swc)
        {
            context = swc;
        }

        public IEnumerable<City> GetCities()
        {
            return context.Cities.ToList();
        }

        public City GetCityByID(int id)
        {
            return context.Cities.Find(id);
        }

        public void InsertCity(City City)
        {
            context.Cities.Add(City);
        }

        public void DeleteCity(int CityID)
        {
            City City = context.Cities.Find(CityID);
            context.Cities.Remove(City);
        }

        public void UpdateCity(City City)
        {
            context.Entry(City).State = EntityState.Modified;
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}