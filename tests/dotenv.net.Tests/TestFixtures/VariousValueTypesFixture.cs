using System;

namespace dotenv.net.Tests.TestFixtures
{
    public class VariousValueTypesFixture : IDisposable
    {
        public VariousValueTypesFixture()
        {
            DotEnv.Fluent()
                .WithEnvFiles("various-value-types.env")
                .Load();
        }

        public void Dispose()
        {
            // do nothing
        }
    }
}