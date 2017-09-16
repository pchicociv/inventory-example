using FluentAssertions;
using Inventory.Models.DomainEvents;
using Inventory.Models.RabbitMQ;
using Inventory.WebApi.Models;
using Inventory.WebFx;
using System;
using TechTalk.SpecFlow;

namespace Inventory.Specs
{
    [Binding]
    public class ItemExpiredNotificationSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly FeatureContext _featureContext;

        public ItemExpiredNotificationSteps(ScenarioContext scenarioContext, FeatureContext featureContext)
        {
            if (scenarioContext == null) throw new ArgumentNullException("scenarioContext");
            _scenarioContext = scenarioContext;
            if (featureContext == null) throw new ArgumentNullException("featureContext");
            _featureContext = featureContext;
        }

        [Given(@"an item with low expiration date:")]
        public void GivenAnItemWithLowExpirationDate(Table table)
        {
            ItemModel newItem = new ItemModel();
            newItem.Label = table.Rows[0]["Label"].ToString();
            newItem.ItemType = table.Rows[0]["ItemType"].ToString();
            newItem.ExpirationDate = table.Rows[0]["ExpirationDate"].ToString();
            _scenarioContext.Add("NewItem", newItem);
        }

        [When(@"an item expires")]
        public void WhenAnItemExpires()
        {
            DayFinished e = new DayFinished(new CallContext(),DateTime.Today);
            using (RabbitMqNotificationBus bus = new RabbitMqNotificationBus())
            {
                bus.Publish(e);
            }
        }

        [Then(@"there is a notification about the expired item")]
        public async void ThenThereIsANotificationAboutTheExpiredItem()
        {
            var createdItem = _scenarioContext["CreatedItem"] as ItemModel;

            using (RabbitMqNotificationBus bus = new RabbitMqNotificationBus())
            {
                ItemExpired notification = new ItemExpired(new CallContext());
                await bus.SubscribeAndWaitFirstMessage<ItemExpired>(a => { notification = a; });

                notification.Should().BeOfType(typeof(ItemExpired));
                notification.Label.Should().BeEquivalentTo(createdItem.Label);
            }
        }
    }
}
