using Scraper.Core.ValueTypes;

namespace Scraper.Test.Utils
{
    public static class AssertionExtensions
    {
        public static ResultAssertions<T> Should<T>(this Result<T> value) => new ResultAssertions<T>(value);
    }
}
