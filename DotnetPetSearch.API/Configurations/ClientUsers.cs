using System.Collections.ObjectModel;

namespace DotnetPetSearch.API.Configurations;

public class ClientUsers
{
    public IDictionary<string, string> Clients = ReadOnlyDictionary<string, string>.Empty;
}