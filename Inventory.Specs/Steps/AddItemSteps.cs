using FluentAssertions;
using Inventory.WebApi.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using TechTalk.SpecFlow;

namespace Inventory.Specs
{
    [Binding]
    public class AddItemSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly FeatureContext _featureContext;

        public AddItemSteps(ScenarioContext scenarioContext, FeatureContext featureContext)
        {
            if (scenarioContext == null) throw new ArgumentNullException("scenarioContext");
            _scenarioContext = scenarioContext;
            if (featureContext == null) throw new ArgumentNullException("featureContext");
            _featureContext = featureContext;
        }

        [Given(@"an item with the following fields:")]
        public void GivenAnItemWithTheFollowingFields(Table table)
        {
            ItemModel newItem = new ItemModel();
            newItem.Label = table.Rows[0]["Label"].ToString();
            newItem.ItemType = table.Rows[0]["ItemType"].ToString();
            newItem.ExpirationDate = table.Rows[0]["ExpirationDate"].ToString();
            _scenarioContext.Add("NewItem", newItem);
        }

        [When(@"I add the new item to the inventory")]
        public void WhenIAddANewItemToTheInventory()
        {
            var httpClient = _featureContext.TestServer().HttpClient;
            var newItem = _scenarioContext["NewItem"] as ItemModel;

            newItem.Should().NotBeNull();

            if (newItem != null)
            {
                using (var response = httpClient.PostAsJsonAsync("/api/inventory"+WebServer.QUERY_STRING,newItem).Result)
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

        [Then(@"the inventory contains information about the newly added item")]
        public void ThenTheInventoryContainsInformationAboutTheNewlyAddedItem()
        {
            var httpClient = _featureContext.TestServer().HttpClient;
            var newItem = _scenarioContext["NewItem"] as ItemModel;
            var createdItem = _scenarioContext["CreatedItem"] as ItemModel;

            newItem.Should().NotBeNull();
            createdItem.Should().NotBeNull();

            using (var response = httpClient.GetAsync("/api/inventory/"+createdItem.Label + WebServer.QUERY_STRING).Result)
            {
                using (var httpContent = response.Content)
                {
                    var content = httpContent.ReadAsStringAsync().Result;
                    ItemModel receivedItem = JsonConvert.DeserializeObject<ItemModel>(content);

                    receivedItem.Label.Should().BeEquivalentTo(newItem.Label);
                    receivedItem.ItemType.Should().BeEquivalentTo(newItem.ItemType);
                    receivedItem.ExpirationDate.Should().BeEquivalentTo(newItem.ExpirationDate);
                }
            }
        }
    }
}
