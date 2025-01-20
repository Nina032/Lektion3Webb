using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lektion3Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Caching.Memory;

namespace Lektion3Web.Controllers
{
    public class LocationsController : Controller
    {
        private readonly static string COUNTRIES_KEY = "Globocations";

        private IMemoryCache countryCache;
        private CosmosClient docClient;
        private Microsoft.Azure.Cosmos.Container docContainer;
        public LocationsController(IMemoryCache cache, CosmosClient client)
        {
            countryCache = cache;
            docClient = client;
            docContainer = docClient.GetContainer("lektion3cdb", "lektion3locations");
        }
        public async Task<IActionResult> Index()
        {
            //get countries for search list
            var countries = await GetCountriesListAsync();
            ViewData["CountriesList"] = countries;

            //get the list of all locations from CosmosDb
            var locations = new List<LocationModel>();

            var iterator = docContainer.GetItemQueryIterator<LocationModel>();
            while (iterator.HasMoreResults)
            {
                var pageOfLocations = await iterator.ReadNextAsync();
                locations.AddRange(pageOfLocations.ToList());
            }

            //pass locations and countries
            return View(locations);
        }

        private async Task<List<string>> GetCountriesListAsync()
        {
            var countries = await countryCache.GetOrCreateAsync<List<string>>(
                COUNTRIES_KEY, (entry) => {
                    //TODO: Get countries from CosmosDB
                    
                    return Task.FromResult(new List<string>(new string[]{ "United States" }));
                });

            return countries;
           
        }

        [HttpPost]
        public async Task<IActionResult> Add(LocationModel model)
        {
            if (ModelState.IsValid)
            {
                //Add a new item to CosmosDB
                model.Id = Guid.NewGuid().ToString();
                await docContainer.CreateItemAsync(
                    model, new PartitionKey(model.Country)
                    );

                return RedirectToAction("index");
            }
            else
            {
                return View();
            }

            
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string country)
        {
            //get all locations for a country
            var iterator = docContainer.GetItemQueryIterator<LocationModel>(
                requestOptions: new QueryRequestOptions()
                {
                    PartitionKey = new PartitionKey(country)
                });
            var locations = new List<LocationModel>();
            return View(locations);
        }

    }
}