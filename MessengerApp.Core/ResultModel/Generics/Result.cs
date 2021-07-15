using System;
using System.Collections.Generic;
using System.Linq;
using MessengerApp.Core.ResultModel.Abstraction.Generics;

namespace MessengerApp.Core.ResultModel.Generics
{
    public class Result<TData> : IResult<TData>
    {
        private readonly List<string> _messagesList;

        private Result(bool success, TData data = default!, IEnumerable<string>? messages = null,
            Exception? exception = null)
        {
            _messagesList = messages?.ToList() ?? new List<string>();
            Success = success;
            Exception = exception;
            Data = data;
        }

        public TData Data { get; }

        public IReadOnlyCollection<string> Messages => _messagesList;

        public bool Success { get; }

        public Exception? Exception { get; }

        public static Result<TData> CreateFailed(string message, Exception? exception = null)
        {
            return new(false, default!, new List<string> {message}, exception);
        }

        public static Result<TData> CreateFailed(IEnumerable<string> messages, Exception? exception = null)
        {
            return new(false, default!, messages, exception);
        }

        public static Result<TData> CreateSuccess(TData data)
        {
            return new(true, data);
        }

        public Result<TData> AddError(string message)
        {
            _messagesList.Add(message);

            return this;
        }

        public Result<TData> AddErrors(IEnumerable<string> collection)
        {
            _messagesList.AddRange(collection);

            return this;
        }
    }
}