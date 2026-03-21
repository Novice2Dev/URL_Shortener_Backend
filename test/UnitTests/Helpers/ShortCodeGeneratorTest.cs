using URLShortener.Helpers;
using Xunit;

namespace test.UnitTests.Helpers
{
    public class ShortCodeGeneratorTests
    {
        [Fact]
        public void GenerateShortCode_ShouldReturn6CharacterString()
        {
            var shortCode = ShortCodeGenerator.GenerateShortCode();

            Assert.NotNull(shortCode);
            Assert.Equal(6, shortCode.Length);
        }

        [Fact]
        public void GenerateShortCode_ShouldContainOnlyAlphanumericCharacters()
        {
            var code = ShortCodeGenerator.GenerateShortCode();

            Assert.Matches("^[a-zA-Z0-9]+$", code);
        }
    }
}