using System;

using FluentAssertions;
using FluentAssertions.Execution;

using Scraper.Core.ValueTypes;

namespace Scraper.Test.Utils
{
    public abstract class ResultAssertionsBase<T, TAssertion>
        where TAssertion : ResultAssertionsBase<T, TAssertion>
    {

        public Result<T> Subject { get; }

        protected ResultAssertionsBase(Result<T> value)
        {
            Subject = value;
        }

        public AndWhichConstraint<TAssertion, T> BeSuccess(string because = "")
        {
            Execute.Assertion
                .ForCondition(Subject.IsSuccess)
                .BecauseOf(because)
                .FailWith("Expected Result<T> to be successful {reason}, but found {0}", (Exception)Subject);

            return new AndWhichConstraint<TAssertion, T>((TAssertion)this, Subject);
        }

        public AndWhichConstraint<TAssertion, Exception> BeFailed(string because = "")
        {
            Execute.Assertion
                .ForCondition(!Subject.IsSuccess)
                .BecauseOf(because)
                .FailWith("Expected Result<T> to be failed {reason}, but found {0}", (Exception)Subject);

            return new AndWhichConstraint<TAssertion, Exception>((TAssertion)this, Subject);
        }
    }

    public class ResultAssertions<T> : ResultAssertionsBase<T, ResultAssertions<T>>
    {
        public ResultAssertions(Result<T> value)
            : base(value)
        {
        }
    }
}
