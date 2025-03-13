using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private static Dictionary<string, Dictionary<int, CalendarTask>> calendarData = DatabaseContext.LoadData();

        [HttpGet]
        public IActionResult GetTasks()
        {
            return Ok(calendarData);
        }

        [HttpPost]
        public IActionResult SaveTasks([FromBody] Dictionary<string, Dictionary<int, CalendarTask>> newData)
        {
            calendarData = newData; 
            DatabaseContext.SaveData(calendarData); 

            return Ok(calendarData);
        }
    }

    [Route("calendar")]
    public class CalendarPageController : Controller
    {
        // GET: /calendar - Serve the Calendar.html page
        [HttpGet]
        public IActionResult GetCalendarPage()
        {
            var htmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Calendar.html");

            if (System.IO.File.Exists(htmlFilePath))
            {
                var htmlContent = System.IO.File.ReadAllText(htmlFilePath);
                return Content(htmlContent, "text/html");
            }

            return NotFound("Calendar.html not found.");
        }
    }
}
