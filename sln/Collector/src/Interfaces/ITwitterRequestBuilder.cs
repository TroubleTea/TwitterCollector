using RestSharp;

namespace TwitterCollector.Interafaces
{
  internal interface ITwitterRequestBuilder
  {
    IRestRequest Build();
  }
}