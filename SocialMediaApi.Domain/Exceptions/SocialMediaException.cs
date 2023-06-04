using Microsoft.AspNetCore.Http;
using System.Runtime.Serialization;

namespace SocialMediaApi.Domain.Exceptions
{
    [Serializable]
    public sealed class SocialMediaException : Exception
    {
        public int HttpStatusCode { get; set; }

        public SocialMediaException(string? message, int httpStatusCode = StatusCodes.Status400BadRequest) : base(message)
        {
            HttpStatusCode = httpStatusCode;
        }

        public SocialMediaException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public SocialMediaException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}