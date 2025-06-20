using System.Net;

namespace DotnetPetSearch.PetFinderHttpClient.Tests.TestData;

public class UnsuccessfulStatusCodes : TheoryData<HttpStatusCode>
{
    public UnsuccessfulStatusCodes()
    {
        Add(HttpStatusCode.Unauthorized);
        Add(HttpStatusCode.BadRequest);
        Add(HttpStatusCode.Forbidden);
        Add(HttpStatusCode.InternalServerError);
    }
}