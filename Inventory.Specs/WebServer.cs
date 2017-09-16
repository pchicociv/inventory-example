using Inventory.WebApi;
using Microsoft.Owin.Testing;
using TechTalk.SpecFlow;

namespace Inventory.Specs
{
    [Binding]
    public class WebServer
    {
        public const string QUERY_STRING = "?apikey=88DA3BBAC33F4C35A7B6453185F38BB2&token=F46B9A9B642943C182E319880F85A554";

        [BeforeFeature]
        public static void CreateServer(FeatureContext featureContext)
        {
            featureContext.TestServer(TestServer.Create<Startup>());
            var testServer = featureContext.TestServer();
            testServer.BaseAddress = new System.Uri("https://localhost:44301/");
            var httpClient = testServer.HttpClient;
            httpClient.DefaultRequestHeaders.Add("Authorization", "Basic cGNoaWNvY2l2OnBjaGljb2Npdg==");
        }

        [AfterFeature]
        public static void StopServer(FeatureContext featureContext)
        {
            var server = featureContext.TestServer();
            server.Dispose();
        }
    }

    public static class FeatureContextExtensions
    {
        private const string KeyServer = "server";

        public static TestServer TestServer(this FeatureContext source)
        {
            return source.Get<TestServer>(KeyServer);
        }

        public static void TestServer(this FeatureContext source, TestServer server)
        {
            source.Add(KeyServer, server);
        }
    }
}
