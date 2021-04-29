using System;
using System.Runtime.Serialization;
using RestSharp;

namespace TwitterCollector.Exceptions
{
  [Serializable]
  public class TwitterApiLimitExceededException : SearchRequestFailedException
  {
    public TwitterApiLimitExceededException(IRestResponse response) : base(response)
    {
    }

    public TwitterApiLimitExceededException(string message) : base(message)
    {
    }

    public TwitterApiLimitExceededException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected TwitterApiLimitExceededException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}