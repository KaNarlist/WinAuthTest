using Contracts;

namespace WinAuthTest
{
    public class TestService : ITestService
    {
        public void Test()
        {
            System.Diagnostics.Debug.Print("Test called!");
        }
    }
}
