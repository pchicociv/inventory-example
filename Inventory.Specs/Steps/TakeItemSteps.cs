using FluentAssertions;
using Inventory.WebApi.Models;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using TechTalk.SpecFlow;

namespace Inventory.Specs.Steps
{
    [Binding]
    public class TakeItemSteps
    {

        private readonly ScenarioContext _scenarioContext;
        private readonly FeatureContext _featureContext;

        public TakeItemSteps(ScenarioContext scenarioContext, FeatureContext featureContext)
        {
            if (scenarioContext == null) throw new ArgumentNullException("scenarioContext");
            _scenarioContext = scenarioContext;
            if (featureContext == null) throw new ArgumentNullException("featureContext");
            _featureContext = featureContext;
        }


        [Given(@"the item has been added to the inventory previously")]
        public void GivenTheItemHasBeenAddedToTheInventoryPreviously()
        {
            var httpClient = _featureContext.TestServer().HttpClient;
            var newItem = _scenarioContext["NewItem"] as ItemModel;

            newItem.Should().NotBeNull();

            if (newItem != null)
            {
                using (var response = httpClient.PostAsJsonAsync("/api/inventory" + WebServer.QUERY_STRING, newItem).Result)
                {
                    using (var httpContent = response.Content)
                    {
                        var content = httpContent.ReadAsStringAsync().Result;
                        ItemModel createdItem = JsonConvert.DeserializeObject<ItemModel>(content);
                        createdItem.Should().NotBeNull();
                        _scenarioContext.Add("CreatedItem", createdItem);
                    }
                }
            }
        }
        
        [When(@"I took an item out from the inventory")]
        public void WhenITookAnItemOutFromTheInventory()
        {
            var httpClient = _featureContext.TestServer().HttpClient;
            var createdItem = _scenarioContext["CreatedItem"] as ItemModel;

            createdItem.Should().NotBeNull();

            if (createdItem != null)
            {
                using (var response = httpClient.DeleteAsync(createdItem.Url + WebServer.QUERY_STRING).Result)
                {
                    using (var httpContent = response.Content)
                    {
                        var content = httpContent.ReadAsStringAsync().Result;
                        ItemModel deletedItem = JsonConvert.DeserializeObject<ItemModel>(content);
                        _scenarioContext.Add("DeletedItem", deletedItem);
                    }
                }
            }
        }
        
        [Then(@"the item is no longer in the inventory")]
        public void ThenTheItemIsNoLongerInTheInventory()
        {
            var httpClient = _featureContext.TestServer().HttpClient;
            var newItem = _scenarioContext["NewItem"] as ItemModel;
            var deletedItem = _scenarioContext["DeletedItem"] as ItemModel;

            newItem.Should().NotBeNull();
            deletedItem.Should().NotBeNull();

            using (var response = httpClient.GetAsync(deletedItem.Url + WebServer.QUERY_STRING).Result)
            {
                response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }
        }
    }
}
