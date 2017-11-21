using System;
using FluentAssertions;
using Xunit;

namespace dotenv.net.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            true.Should().BeTrue();
        }
    }
}