using depthchart.api.Models;
using Newtonsoft.Json;
using System.Text;

namespace depthchart.api.tests.IntergrationTests.Fixtures
{
    public class DepthChartTestsFixture : TestWebApplicationFactory<Program>
    {
        private const string PlayerApi = "api/players";

        public async Task<(HttpResponseMessage message, string stringContent)> PostPlayer(PlayerDto player)
        {
            var json = JsonConvert.SerializeObject(player);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await Client.PostAsync($"{PlayerApi}/add/", stringContent);
            var responseString = await response.Content.ReadAsStringAsync();

            return (response, responseString);
        }
    }
}