namespace DotnetPetSearch.Data.Tests.TestData;

public class FutureDateTime : TheoryData<DateTime>
{
    public FutureDateTime()
    {
        Add(DateTime.Now.AddYears(1));
        Add(DateTime.Now.AddMonths(1));
        Add(DateTime.Now.AddDays(1));
        Add(DateTime.Now.AddHours(1));
        Add(DateTime.Now.AddMinutes(1));
    }
}