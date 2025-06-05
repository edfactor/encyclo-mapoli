using System.Net;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace YourNamespace.Tests
{
    public class YourTests
    {
        [Fact]
        public async Task YourTestName()
        {
            // Arrange
            var client = /* initialize your client */;
            var request = /* create your request */;

            // Act
            var response = await client.SendAsync(request);

            // Assert
            response.Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

            // Act
            response = await client.SendAsync(request);

            // Assert
            response.Response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = /* get your result */;
            result.ShouldNotBeNullOrEmpty();

            var lines = result.Split('\n');
            int l = 0;
            lines[l++].ShouldNotBeEmpty(); // has Date/time
            lines[l++].ShouldBe("Profit Sharing Edit");
            lines[l].ShouldBe("Number,Name,Code,Contribution Amount,Earnings Amount,Incoming Forfeitures,Reason");
        }
    }
}