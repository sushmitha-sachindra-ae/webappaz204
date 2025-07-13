using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Diagnostics;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public IActionResult Index()
        {
            List<Course> courses = new List<Course>();
            var _config= _configuration.GetSection("common:settings");
            var connectionString = _config.GetValue<string>("dbpassword");
            _logger.LogInformation("Connection String: {ConnectionString}", connectionString);
            try
            {

        if(!string.IsNullOrEmpty(connectionString))
            {
                _logger.LogInformation("Successfully retrieved the connection string.");
            }
            else
            {
                _logger.LogWarning("Connection string is null or empty.");
                return BadRequest("Connection string is not configured properly.");
            }
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            SqlCommand
             cmd = new SqlCommand("SELECT * FROM tblCourse", conn);
            SqlDataReader reader = cmd.ExecuteReader();
          
            while (reader.Read())
            {
                Course course = new Course
                {
                    Id = reader.GetInt32(0),
                    sName = reader.GetString(1),
                    Rating =( reader.GetDecimal(2))
                };
                courses.Add(course);
            }   
            reader.Close();
            conn.Close();
            _logger.LogInformation("Successfully accessed the database and retrieved courses.");
            return View(courses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while accessing the connection string.");
            return StatusCode(500, "Internal server error");
            }
            // Use the connection string as needed
            return View(courses);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
