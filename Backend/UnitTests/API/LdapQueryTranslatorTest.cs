using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Moq;
using API.Attributes;
using API.Linq;

namespace UnitTests.API;

[TestClass]
public class LdapQueryTranslatorTest
{
    private readonly Mock<ILogger> _mockLogger;
    private readonly LdapQueryTranslator _translator;

    public LdapQueryTranslatorTest()
    {
        _mockLogger = new Mock<ILogger>();
        _translator = new LdapQueryTranslator(_mockLogger.Object);
    }

    public class TestUser
    {
        public string Name { get; set; } = "";

        [AuthenticationMapping("cn")]
        public string CommonName { get; set; } = "";

        [AuthenticationMapping("mail")]
        public string Email { get; set; } = "";
    }

    [TestMethod]
    public void Translate_SimpleEquality_ReturnsCorrectLdapFilter()
    {
        Console.WriteLine("Testing simple equality translation: u.Name == \"john\"");
        
        // Arrange
        Expression<Func<TestUser, bool>> expression = u => u.Name == "john";

        // Act
        var result = _translator.Translate(expression.Body);

        Console.WriteLine($"Expected: (Name=john)");
        Console.WriteLine($"Actual:   {result}");

        // Assert
        Assert.AreEqual("(Name=john)", result);
        Console.WriteLine("✅ Test passed: Simple equality translation");
    }

    [TestMethod]
    public void Translate_EqualityWithMappedAttribute_ReturnsCorrectLdapFilter()
    {
        Console.WriteLine("Testing equality with mapped attribute: u.CommonName == \"John Doe\"");
        Console.WriteLine("CommonName is mapped to 'cn' via AuthenticationMapping attribute");
        
        // Arrange
        Expression<Func<TestUser, bool>> expression = u => u.CommonName == "John Doe";

        // Act
        var result = _translator.Translate(expression.Body);

        Console.WriteLine($"Expected: (cn=John Doe)");
        Console.WriteLine($"Actual:   {result}");

        // Assert
        Assert.AreEqual("(cn=John Doe)", result);
        Console.WriteLine("✅ Test passed: Equality with mapped attribute");
    }

    [TestMethod]
    public void Translate_AndCondition_ReturnsCorrectLdapFilter()
    {
        Console.WriteLine("Testing AND condition: u.Name == \"john\" && u.Email == \"john@test.com\"");
        Console.WriteLine("Email property is mapped to 'mail' via AuthenticationMapping attribute");
        
        // Arrange
        Expression<Func<TestUser, bool>> expression = u => u.Name == "john" && u.Email == "john@test.com";

        // Act
        var result = _translator.Translate(expression.Body);

        Console.WriteLine($"Expected: (&(Name=john)(mail=john@test.com))");
        Console.WriteLine($"Actual:   {result}");

        // Assert
        Assert.AreEqual("(&(Name=john)(mail=john@test.com))", result);
        Console.WriteLine("✅ Test passed: AND condition translation");
    }

    [TestMethod]
    public void Translate_OrCondition_ReturnsCorrectLdapFilter()
    {
        Console.WriteLine("Testing OR condition: u.Name == \"john\" || u.Name == \"jane\"");
        
        // Arrange
        Expression<Func<TestUser, bool>> expression = u => u.Name == "john" || u.Name == "jane";

        // Act
        var result = _translator.Translate(expression.Body);

        Console.WriteLine($"Expected: (|(Name=john)(Name=jane))");
        Console.WriteLine($"Actual:   {result}");

        // Assert
        Assert.AreEqual("(|(Name=john)(Name=jane))", result);
        Console.WriteLine("✅ Test passed: OR condition translation");
    }

    [TestMethod]
    public void Translate_StringContains_ReturnsCorrectLdapFilter()
    {
        Console.WriteLine("Testing string Contains method: u.Name.Contains(\"joh\")");
        Console.WriteLine("Should translate to wildcard LDAP filter with *value* syntax");
        
        // Arrange
        Expression<Func<TestUser, bool>> expression = u => u.Name.Contains("joh");

        // Act
        var result = _translator.Translate(expression.Body);

        Console.WriteLine($"Expected: (Name=*joh*)");
        Console.WriteLine($"Actual:   {result}");

        // Assert
        Assert.AreEqual("(Name=*joh*)", result);
        Console.WriteLine("✅ Test passed: String Contains translation");
    }

    [TestMethod]
    public void Translate_ComplexCondition_ReturnsCorrectLdapFilter()
    {
        Console.WriteLine("Testing complex condition: (u.Name == \"john\" || u.Name.Contains(\"jane\")) && u.Email == \"test@example.com\"");
        Console.WriteLine("Should combine OR, Contains, and AND operations correctly");
        
        // Arrange
        Expression<Func<TestUser, bool>> expression = u => (u.Name == "john" || u.Name.Contains("jane")) && u.Email == "test@example.com";

        // Act
        var result = _translator.Translate(expression.Body);

        Console.WriteLine($"Expected: (&(|(Name=john)(Name=*jane*))(mail=test@example.com))");
        Console.WriteLine($"Actual:   {result}");

        // Assert
        Assert.AreEqual("(&(|(Name=john)(Name=*jane*))(mail=test@example.com))", result);
        Console.WriteLine("✅ Test passed: Complex condition translation");
    }

    [TestMethod]
    public void Translate_WithVariableReference_ReturnsCorrectLdapFilter()
    {
        Console.WriteLine("Testing variable reference: u.Name == searchName where searchName = \"john\"");
        Console.WriteLine("Should correctly extract value from variable reference");
        
        // Arrange
        var searchName = "john";
        Expression<Func<TestUser, bool>> expression = u => u.Name == searchName;

        // Act
        var result = _translator.Translate(expression.Body);

        Console.WriteLine($"Expected: (Name=john)");
        Console.WriteLine($"Actual:   {result}");

        // Assert
        Assert.AreEqual("(Name=john)", result);
        Console.WriteLine("✅ Test passed: Variable reference translation");
    }

    [TestMethod]
    public void Translate_ContainsWithVariable_ReturnsCorrectLdapFilter()
    {
        Console.WriteLine("Testing Contains with variable: u.Name.Contains(searchTerm) where searchTerm = \"joh\"");
        Console.WriteLine("Should correctly extract variable value and apply wildcard syntax");
        
        // Arrange
        var searchTerm = "joh";
        Expression<Func<TestUser, bool>> expression = u => u.Name.Contains(searchTerm);

        // Act
        var result = _translator.Translate(expression.Body);

        Console.WriteLine($"Expected: (Name=*joh*)");
        Console.WriteLine($"Actual:   {result}");

        // Assert
        Assert.AreEqual("(Name=*joh*)", result);
        Console.WriteLine("✅ Test passed: Contains with variable translation");
    }

    [TestMethod]
    public void Translate_EmptyStringEquality_ReturnsCorrectLdapFilter()
    {
        Console.WriteLine("Testing empty string equality: u.Name == \"\"");
        Console.WriteLine("Should handle empty string values correctly");
        
        // Arrange
        Expression<Func<TestUser, bool>> expression = u => u.Name == "";

        // Act
        var result = _translator.Translate(expression.Body);

        Console.WriteLine($"Expected: (Name=)");
        Console.WriteLine($"Actual:   {result}");

        // Assert
        Assert.AreEqual("(Name=)", result);
        Console.WriteLine("✅ Test passed: Empty string equality translation");
    }

    [TestMethod]
    public void Translate_NullValue_HandlesGracefully()
    {
        Console.WriteLine("Testing null value handling: u.Name == nullValue where nullValue = null");
        Console.WriteLine("Should handle null values gracefully and convert to empty string");
        
        // Arrange
        string? nullValue = null;
        Expression<Func<TestUser, bool>> expression = u => u.Name == nullValue;

        // Act
        var result = _translator.Translate(expression.Body);

        Console.WriteLine($"Expected: (Name=)");
        Console.WriteLine($"Actual:   {result}");

        // Assert
        Assert.AreEqual("(Name=)", result);
        Console.WriteLine("✅ Test passed: Null value handling");
    }

    [TestMethod]
    public void VisitMethodCall_FirstOrDefault_ProcessesSourceQuery()
    {
        Console.WriteLine("Testing FirstOrDefault method call: queryable.Where(u => u.Name == \"john\").FirstOrDefault()");
        Console.WriteLine("Should process the source query (Where clause) correctly");
        
        // Arrange
        var queryable = new List<TestUser>().AsQueryable();
        Expression<Func<TestUser?>> expression = () => queryable.Where(u => u.Name == "john").FirstOrDefault();

        // Act
        var result = _translator.Translate(expression.Body);

        Console.WriteLine($"Expected: (Name=john)");
        Console.WriteLine($"Actual:   {result}");

        // Assert
        Assert.AreEqual("(Name=john)", result);
        Console.WriteLine("✅ Test passed: FirstOrDefault method call translation");
    }

    [TestMethod]
    public void Translate_ClearsFilterBetweenCalls()
    {
        Console.WriteLine("Testing filter clearing between calls");
        Console.WriteLine("First call: u.Name == \"john\"");
        Console.WriteLine("Second call: u.Name == \"jane\"");
        Console.WriteLine("Should not have any interference between the two translations");
        
        // Arrange
        Expression<Func<TestUser, bool>> expression1 = u => u.Name == "john";
        Expression<Func<TestUser, bool>> expression2 = u => u.Name == "jane";

        // Act
        var result1 = _translator.Translate(expression1.Body);
        var result2 = _translator.Translate(expression2.Body);

        Console.WriteLine($"First call - Expected: (Name=john), Actual: {result1}");
        Console.WriteLine($"Second call - Expected: (Name=jane), Actual: {result2}");

        // Assert
        Assert.AreEqual("(Name=john)", result1);
        Assert.AreEqual("(Name=jane)", result2);
        Console.WriteLine("✅ Test passed: Filter clearing between calls");
    }
}