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
    public class NotificationThatAnItemHasBeenTakenOutSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly FeatureContext _featureContext;

        public NotificationThatAnItemHasBeenTakenOutSteps(ScenarioContext scenarioContext, FeatureContext featureContext)
        {
            if (scenarioContext == null) throw new ArgumentNullException("scenarioContext");
            _scenarioContext = scenarioContext;
            if (featureContext == null) throw new ArgumentNullException("featureContext");
            _featureContext = featureContext;
        }

        [Then(@"there is a notification that an item has been taken out")]
        public async void ThenThereIsANotificationThatAnItemHasBeenTakenOut()
        {
            var createdItem = _scenarioContext["CreatedItem"] as ItemModel;

            using (RabbitMqNotificationBus bus = new RabbitMqNotificationBus())
            {
                ItemTaken notification = new ItemTaken(new CallContext());
                await bus.SubscribeAndWaitFirstMessage<ItemTaken>(a => { notification = a; });

                notification.Should().BeOfType(typeof(ItemTaken));
                notification.Label.Should().BeEquivalentTo(createdItem.Label);
            }
        }
    }
}
