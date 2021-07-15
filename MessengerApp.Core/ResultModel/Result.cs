using System;
using System.Collections.Generic;
using System.Linq;
using MessengerApp.Core.ResultModel.Abstraction;

namespace MessengerApp.Core.ResultModel
{
    public class Result : IResult
    {
        private readonly List<string> _messagesList;

        public Result(bool success, IEnumerable<string>? messages = null, Exception? exception = null)
        {
            _messagesList = messages?.ToList() ?? new List<string>();
            Success = success;
            Exception = exception;
        }

        public IReadOnlyCollection<string> Messages => _messagesList;

        public bool Success { get; }

        public Exception? Exception { get; }

        public static Result CreateSuccess()
        {
            return new(true);
        }

        public static Result CreateFailed(string message, Exception? exception = null)
        {
            return new(false, new List<string> {message}, exception);
        }

        public static Result CreateFailed(IEnumerable<string> messages, Exception? exception = null)
        {
            return new(false, messages, exception);
        }

        public virtual void AddError(string message)
        {
            _messagesList.Add(message);
        }

        public virtual void AddErrors(IEnumerable<string> collection)
        {
            _messagesList.AddRange(collection);
        }
    }
}