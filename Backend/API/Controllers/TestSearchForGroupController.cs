using API.DTO.LDAP;
using API.Interfaces;
using API.Services;
using Database.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Settings.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestSearchForGroupController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly LdapService _ldapService;
        private readonly JWTSettings _JWTSettings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public TestSearchForGroupController(JwtService jwtService,
            LdapService ldapService,
            IConfiguration configuration,
            IUnitOfWork unitOfWork,
            ILoggerFactory loggerFactory)
        {
            _jwtService = jwtService;
            _ldapService = ldapService;
            _JWTSettings = ConfigurationBinderService.Bind<JWTSettings>(configuration);
            _unitOfWork = unitOfWork;
            _logger = loggerFactory.CreateLogger(GetType());

        }

        //[HttpGet]
        //[Route("all")]
        //public IActionResult GetAllGroups()
        //{
        //    var groups = _ldapService.GetAllGroups<GroupDistinguishedName>();
        //    return Ok(groups);
        //}

        //[HttpGet]
        //[Route("{groupName}/students")]
        //public IActionResult GetStudentsInGroup(string groupName)
        //{
        //    var students = _ldapService.GetUsersInGroup<StudentModel>(groupName);
        //    return Ok(students);
        //}
    }
}
