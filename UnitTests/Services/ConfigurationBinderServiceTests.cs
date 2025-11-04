using API.Services;
using Microsoft.Extensions.Configuration;
using Settings.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Services
{
    public class ConfigurationBinderServiceTests
    {
        private class NullKeyConfig : Base
        {
            public override string? Key => null;
            public string? Value { get; set; }
        }

        // Bind root because Key is empty
        private class EmptyKeyConfig : Base
        {
            public override string Key => "";
            public string? Value { get; set; }
        }

        // Bind section because Key is set
        private class SectionKeyConfig : Base
        {
            public override string Key => "MySection";
            public string? Value { get; set; }
        }
        [Fact]
        public void Bind_ShouldThrow_WhenKeyIsNull()
        {
            var config = new ConfigurationBuilder().Build();
            var ex = Assert.Throws<InvalidOperationException>(() => ConfigurationBinderService.Bind<NullKeyConfig>(config));
            Assert.Equal("The configuration section must have a key.", ex.Message);
        }

        [Fact]
        public void Bind_ShouldBindRoot_WhenKeyIsEmpty()
        {
            var inMemoryConfig = new Dictionary<string, string?> { { "Value", "RootValue" } };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemoryConfig)
                .Build();

            var result = ConfigurationBinderService.Bind<EmptyKeyConfig>(configuration);

            Assert.Equal("RootValue", result.Value);
        }

        [Fact]
        public void Bind_ShouldBindSection_WhenKeyIsSet()
        {
            var inMemorySettings = new Dictionary<string, string>
    {
        {"MySection:Value", "HelloWorld"}
    };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var result = ConfigurationBinderService.Bind<SectionKeyConfig>(configuration);

            Assert.Equal("HelloWorld", result.Value);
        }


    }


}

