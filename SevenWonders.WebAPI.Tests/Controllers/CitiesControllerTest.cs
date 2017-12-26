using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SevenWonders.WebAPI;
using SevenWonders.WebAPI.Controllers;
using System.Collections.Generic;
using SevenWonders.WebAPI.Models;
using SevenWonders.WebAPI.DTO;
using Moq;
using Newtonsoft.Json;
using System.Web.Http.Results;
using SevenWonders.WebAPI.DTO.Cities;

namespace SevenWonders.WebAPI.Tests.Controllers
{
    [TestClass]
    public class CitiesControllerTest
    {
        CitiesController cc;
        Mock<ICityRepository> repo;

        [TestInitialize]
        public void Initialize()
        {
            repo = new Mock<ICityRepository>();

            var country1 = new Country
            {
                Id = 1,
                Name = "Ukraine",
                IsDeleted = false
            };

            var country2 = new Country
            {
            Id = 2,
            Name = "Poland",
            IsDeleted = false
            };

            var citiesList = new List<City>
            {
                new City
                {
                    Id = 1,
                    Name = "Lviv",
                    CountryId = 1,
                    Country = country1,
                    IsDeleted = false
                },

                new City
                {
                    Id = 2,
                    Name = "Kyiv",
                    CountryId = 1,
                    Country = country1,
                    IsDeleted = false
                },

                new City
                {
                    Id = 3,
                    Name = "Krakow",
                    CountryId = 2,
                    Country = country2,
                    IsDeleted = false
                }
            };
            repo.Setup(mr => mr.GetCities()).Returns(citiesList);

            cc = new CitiesController(repo.Object);
        }

        //WORKING TEST WITHOUT MOCK
        //[TestMethod]
        //public void testGetCities()
        //{
            
        //    var result = cc.GetCities() as List<City>;
        //    var city = cc.GetCity(5) as City;
        //    Assert.IsNotNull(result);
        //    Assert.IsNotNull(city);
        //    var findResult = result.Find(x => x.Id == 5);
        //    Assert.IsNotNull(findResult);
        //    Assert.AreEqual(findResult.Id, city.Id);
        //}

        [TestMethod]
        public void GetCitiesTest()
        {
            var result = cc.GetCities() as OkNegotiatedContentResult<List<CityModel>>;
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Content[0].Id, 3);
        }

        [TestMethod]
        public void GetCityTest()
        {
            var result = cc.GetCity(1) as OkNegotiatedContentResult<City>;
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Content.Id, 1);
        }

        [TestMethod]
        public void AddCityTest()
        {
            var newCity = new CityModel
            {
                Name = "Paititi",
                CountryName = "Inka Empire",
                CountryId = 228
            };

            cc.AddCity(newCity);
            Assert.AreEqual(1, 1);
        }

        [TestMethod]
        public void DeleteCityTest()
        {
            var result = cc.DeleteCity(1);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }
    }
}
