namespace Authentication.UnitTests.Factories;

public static class FakeDataHelper
{
    public static string GenerateEmail() => $"test-{Guid.NewGuid().ToString()}@test.com";
    public static string GenerateDeviceId() => Guid.NewGuid().ToString();
    public static string GeneratePassword() => $"My-Secret-Pa$$w0rd-{Guid.NewGuid().ToString()}";
}
