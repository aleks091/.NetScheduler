using System;

namespace Scheduler.IntegrationTests
{
    public static class RandomString
    {
        public static string Generate(int length = 10)
        {
            return Guid.NewGuid().ToString().Substring(0, length);
        }
    }
}
