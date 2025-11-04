using API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Services
{

    // Sample DTO for testing
    public class TestDto
    {
        public string? Name { get; set; } = default!;
        public TestEnum Status { get; set; }
    }

    public enum TestEnum
    {
        Active,
        Inactive
    }
    public class JsonSerializerServiceTests
    {
        private readonly JsonSerializerService _service;

        public JsonSerializerServiceTests()
        {
            _service = new JsonSerializerService();
        }

        [Fact]
        public void Serialize_ShouldProduceIndentedCamelCaseJson()
        {
            var obj = new TestDto { Name = "Test", Status = TestEnum.Active };

            var json = _service.Serialize(obj);

            Assert.Contains("\n", json); // Ensure indented formatting
            Assert.Contains("active", json); // Enum should be camelCase
            Assert.Contains("Test", json);
        }

        [Fact]
        public void DeSerialize_ShouldParseJsonCorrectly_WithTrailingComma()
        {
            string json = "{ \"Name\": \"Test\", \"Status\": \"active\", }"; // Trailing comma

            var result = _service.DeSerialize<TestDto>(json);

            Assert.NotNull(result);
            Assert.Equal("Test", result!.Name);
            Assert.Equal(TestEnum.Active, result.Status);
        }

        [Fact]
        public void DeSerialize_ShouldThrow_WhenJsonIsNull()
        {
            string? json = null;

            Assert.Throws<ArgumentNullException>(() => _service.DeSerialize<TestDto>(json!));
        }

        [Fact]
        public void SerializeAndDeserialize_ShouldPreserveEnumAndValues()
        {
            var obj = new TestDto { Name = "RoundTrip", Status = TestEnum.Inactive };

            // Serialize
            var json = _service.Serialize(obj);

            // Deserialize
            var result = _service.DeSerialize<TestDto>(json);

            Assert.NotNull(result);
            Assert.Equal(obj.Name, result!.Name);
            Assert.Equal(obj.Status, result.Status);
        }

        [Fact]
        public void DeSerialize_ShouldReturnNull_WhenJsonIsLiteralNull()
        {
            string json = "null";

            var result = _service.DeSerialize<TestDto>(json);

            Assert.Null(result);
        }
    }
}
