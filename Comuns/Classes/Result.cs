using Comuns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comuns.Classes
{
    public class Result : IResult
    {
        public Result(bool success, Exception? exception, ICollection<string> messages)
        {
            this.Success = success;
            this.Exception = exception;
            this.Messages = messages;
        }

        public bool Success { get; set; }
        public Exception? Exception { get; set; }
        public ICollection<string> Messages { get; set; } = [];

        public static Result Successfull => new(true, null, []);
        public static Result Failed(Exception exception, ICollection<string> messages) => new(false, exception, messages);
        public static Result Failed(Exception exception, string message) => new(false, exception, new List<string> { message });
        public static Result Failed(string message) => new(false, null, new List<string> { message });
    }
}