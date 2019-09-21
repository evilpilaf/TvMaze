using System;

namespace Scraper.Core.ValueTypes
{
    public readonly struct Result<T>
    {
        private readonly T _content;
        private readonly Exception _error;

        public bool IsSuccess => !Equals(_content, default);

        public Result(T content)
        {
            _content = content;
            _error = null;
        }

        public Result(Exception error)
        {
            _content = default;
            _error = error;
        }

        public static implicit operator T(Result<T> instance)
        {
            return instance.IsSuccess ?
                   instance._content :
                   throw new Exception();
        }

        public static implicit operator Result<T>(T content)
        {
            return new Result<T>(content);
        }

        public static implicit operator Exception(Result<T> instance)
        {
            return instance._error;
        }

        public static implicit operator Result<T>(Exception ex)
        {
            return new Result<T>(ex);
        }
    }

    public static class ResultExtensions
    {
        public static Result<TResult> Match<TInput, TResult>(this Result<TInput> result,
                                                             Func<TInput, TResult> succ,
                                                             Func<Exception, TResult> fail)
        {
            if (result.IsSuccess)
                return succ(result);
            else
                return fail(result);
        }
    }
}
