using System;
using dotenv.net.Tests.TestFixtures;
using dotenv.net.Utilities;
using FluentAssertions;
using Xunit;

namespace dotenv.net.Tests.Utilities
{
    public class EnvReaderTests : IClassFixture<VariousValueTypesFixture>
    {
        [Fact]
        public void ShouldReadStringValues()
        {
            EnvReader.GetStringValue("STRING")
                .Should()
                .Be("laravel");

            EnvReader.TryGetStringValue("STRING", out _)
                .Should()
                .BeTrue();

            EnvReader.TryGetStringValue("NON_EXISTENT_KEY", out _)
                .Should()
                .BeFalse();

            Action action = () => EnvReader.GetStringValue("NON_EXISTENT_KEY");
            action.Should()
                .Throw<Exception>();
        }

        [Fact]
        public void ShouldReadIntValues()
        {
            EnvReader.GetIntValue("INTEGER")
                .Should()
                .Be(3306);

            EnvReader.TryGetIntValue("INTEGER", out _)
                .Should()
                .BeTrue();

            EnvReader.TryGetIntValue("NON_EXISTENT_KEY", out _)
                .Should()
                .BeFalse();

            Action action = () => EnvReader.GetIntValue("NON_EXISTENT_KEY");
            action.Should()
                .Throw<Exception>();
        }

        [Fact]
        public void ShouldReadDoubleValues()
        {
            EnvReader.GetDoubleValue("DOUBLE")
                .Should()
                .Be(2762821981981.37627828722);

            EnvReader.TryGetDoubleValue("DOUBLE", out _)
                .Should()
                .BeTrue();

            EnvReader.TryGetDoubleValue("NON_EXISTENT_KEY", out _)
                .Should()
                .BeFalse();

            Action action = () => EnvReader.GetDoubleValue("NON_EXISTENT_KEY");
            action.Should()
                .Throw<Exception>();
        }

        [Fact]
        public void ShouldReadDecimalValues()
        {
            EnvReader.GetDecimalValue("DECIMAL")
                .Should()
                .Be(34.56m);

            EnvReader.TryGetDecimalValue("DECIMAL", out _)
                .Should()
                .BeTrue();

            EnvReader.TryGetDecimalValue("NON_EXISTENT_KEY", out _)
                .Should()
                .BeFalse();

            Action action = () => EnvReader.GetDecimalValue("NON_EXISTENT_KEY");
            action.Should()
                .Throw<Exception>();
        }

        [Fact]
        public void ShouldReadBooleanValues()
        {
            EnvReader.GetBooleanValue("BOOLEAN")
                .Should()
                .BeTrue();

            EnvReader.TryGetBooleanValue("BOOLEAN", out _)
                .Should()
                .BeTrue();

            EnvReader.TryGetBooleanValue("NON_EXISTENT_KEY", out _)
                .Should()
                .BeFalse();

            Action action = () => EnvReader.GetBooleanValue("NON_EXISTENT_KEY");
            action.Should()
                .Throw<Exception>();
        }

        [Fact]
        public void ShouldTellIfAKeyHasAValue()
        {
            EnvReader.HasValue("BOOLEAN")
                .Should()
                .BeTrue();

            EnvReader.HasValue("NON_EXISTENT_KEY")
                .Should()
                .BeFalse();
        }
    }
}