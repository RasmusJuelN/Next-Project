//using System.Reflection;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace UnitTests.API;

//[TestClass]
//public class AuthorizationTests
//{
//    [TestMethod]
//    public void AllEndpointsShouldHaveAuthorization()
//    {
//        IEnumerable<Type>? controllers = Assembly.Load(nameof(API))
//            .GetTypes()
//            .Where(type => typeof(ControllerBase).IsAssignableFrom(type));
        
//        if (controllers is null || !controllers.Any())
//        {
//            Assert.Fail("No controllers found in the assembly.");
//            return;
//        }
        
//        foreach (Type controller in controllers)
//        {
//            bool hasAuthorize = controller.GetCustomAttributes<AuthorizeAttribute>().Any();
//            bool hasAllowAnonymous = controller.GetCustomAttributes<AllowAnonymousAttribute>().Any();

//            IEnumerable<MethodInfo> methods = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
//                .Where(m => m.IsPublic && !m.IsDefined(typeof(AllowAnonymousAttribute), true));
            
//            foreach (MethodInfo method in methods)
//            {
//                bool methodHasAuthorize = method.GetCustomAttributes<AuthorizeAttribute>().Any();
//                bool methodHasAllowAnonymous = method.GetCustomAttributes<AllowAnonymousAttribute>().Any();

//                Assert.IsTrue(
//                    hasAuthorize || hasAllowAnonymous || methodHasAuthorize || methodHasAllowAnonymous,
//                    $"Authorization attribute is missing on {controller.Name}.{method.Name}."
//                );
//            }
//        }
//    }
//}
