using System;
using System.Runtime.Serialization;
using RestSharp;

namespace TwitterCollector.Exceptions
{
  [Serializable]
  public class SearchRequestFailedException : Exception
  {
    public IRestResponse Response { get; }
    public SearchRequestFailedException(IRestResponse response)
    {
      Response = response;
    }

    public SearchRequestFailedException(string message) : base(message)
    {
    }

    public SearchRequestFailedException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected SearchRequestFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}