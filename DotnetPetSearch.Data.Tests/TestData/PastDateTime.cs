namespace DotnetPetSearch.Data.Tests.TestData;

public class PastDateTime: TheoryData<DateTime>
{
    public PastDateTime()
    {
        Add(DateTime.Now.AddYears(-1));
        Add(DateTime.Now.AddMonths(-1));
        Add(DateTime.Now.AddDays(-1));
        Add(DateTime.Now.AddHours(-1));
        Add(DateTime.Now.AddMinutes(-1));
        Add(DateTime.Now.AddSeconds(-1));
        Add(DateTime.Now);
    }
    
}