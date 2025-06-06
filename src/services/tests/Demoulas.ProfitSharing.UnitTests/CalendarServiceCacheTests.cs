using Shouldly;

namespace UnitTests
{
    public class SampleTests
    {
        [Fact]
        public void Test1()
        {
            // Arrange
            var expected = 5;
            var actual = 2 + 3;

            // Act & Assert
            actual.ShouldBe(expected);
        }

        [Fact]
        public void Test2()
        {
            // Arrange
            var str = "Hello, World!";

            // Act & Assert
            str.ShouldStartWith("Hello");
            str.ShouldEndWith("World!");
        }
    }
}
