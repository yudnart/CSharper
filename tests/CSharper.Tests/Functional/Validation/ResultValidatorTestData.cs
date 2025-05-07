namespace CSharper.Tests.Functional.Validation;

public static class ResultValidatorTestData
{
    public static TheoryData<string> InvalidErrorMessages()
    {
        return new TheoryData<string>
        {
            { null! },
            { "" },
            { " " }
        };
    }
}
