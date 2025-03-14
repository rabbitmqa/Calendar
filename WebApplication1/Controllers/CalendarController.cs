using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private static Dictionary<string, Dictionary<string, Dictionary<int, CalendarTask>>> calendarData = DatabaseContext.LoadData();

        [HttpGet]
        public IActionResult GetTasks()
        {
            //var a = new Dictionary<string, Dictionary<string, Dictionary<int, CalendarTask>>>();
            //DatabaseContext.SaveData(a);
            calendarData = DatabaseContext.LoadData();
            return Ok(calendarData);
        }

        [HttpPost]
        public IActionResult SaveTasks([FromBody] Dictionary<string, Dictionary<string, Dictionary<int, CalendarTask>>> newData)
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
            var htmlContent = GetHtmlContent();
            return Content(htmlContent, "text/html");

            //var htmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Calendar.html");
            //if (System.IO.File.Exists(htmlFilePath))
            //{
            //    var htmlContent = System.IO.File.ReadAllText(htmlFilePath);
            //    return Content(htmlContent, "text/html");
            //}

            //return NotFound("Calendar.html not found.");
        }

        private static string GetHtmlContent()
        {
            return @"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>7 AM to 7 AM Weekly Calendar</title>
    <style>

        /* General Styles */
        body {
            font-family: Arial, sans-serif;
            padding: 20px;
            background-color: #f4f4f9;
            text-align: center;
        }

        h1 {
            margin-bottom: 10px;
        }

        /* Wrapper to Enable Scrolling */
        .calendar-wrapper {
            overflow-x: auto;
            overflow-y: auto; /* Allows vertical scrolling */
            max-height: 80vh; /* Limits the table height */
            position: relative;
            border: 1px solid #ddd; /* Adds border for better structure */
        }

        /* Table Layout */
        table {
            width: max-content;
            border-collapse: collapse;
            margin: 20px 0;
            table-layout: fixed;
        }

        table, th, td {
            border: 1px solid #ddd;
        }

        /* Sticky Header */
        thead th {
            position: sticky;
            top: 0;
            background-color: #4CAF50; /* Ensures header background stays visible */
            z-index: 10; /* Keeps headers above other elements */
            color: white;
            padding: 15px;
            text-align: center;
        }
        /* Ensures the first header (Time column) is always above others */
        th:first-child {
            z-index: 12; /* Ensures it stays above the first column */
        }

        /* Sticky First Column (Time Column) */
        th:first-child, td:first-child {
            position: sticky;
            left: 0;
            background-color: green;
            color: white;
            z-index: 11;
            border-right: 2px solid #ddd;
            text-align: center;
            white-space: nowrap; /* Prevents text from wrapping */
        }


        /* Table Cell Styling */
        th, td {
            padding: 10px;
            text-align: center;
            font-size: 20px;
            height: 150px;
            width: 120px;
        }

        /* Reminder Background Colors */
        .task-cell.green {
            background-color: #c8e6c9;
        }

        .task-cell.yellow {
            background-color: #fff59d;
        }

        .task-cell.orange {
            background-color: #ffcc80;
        }

        .task-cell.red {
            background-color: #ff8a80;
        }

        /* Week Navigation Bar */
        .week-navigation {
            position: absolute;
            top: 0;
            left: 50%;
            transform: translateX(-50%);
            display: flex;
            justify-content: center;
            align-items: center;
            background: white;
            z-index: 15; /* Ensure it's above other elements */
            padding: 10px;
            width: 100%;
            max-width: 600px;
            min-width: 300px;
            text-align: center;
            border-radius: 10px;
            box-shadow: 0px 2px 5px rgba(0, 0, 0, 0.1);
        }

            /* Style for Navigation Buttons */
            .week-navigation button {
                flex: 1;
                min-width: 100px;
                font-size: 12px;
                margin: 8px;
            }

        /* Mobile View */
        @media (max-width: 768px) {
            /* Table Styling for Mobile */
            table {
                width: max-content;
                display: table;
            }

            /* Sticky Header for Mobile */
            thead th {
                position: sticky;
                top: 0;
                z-index: 10;
            }

            /* Sticky First Column on Mobile */
            th:first-child, td:first-child {
                font-size: 20px !important; /* Reduce font size for better fit */
                overflow: hidden; /* Hide extra content */
                font-size: 22px; /* Adjust font size for better fit */
            }

            /* Task Input Styling */
            .task-input {
                width: 100%;
                font-size: 16px;
                height: 80px;
                padding: 8px;
                box-sizing: border-box;
            }

            .reminder-select {
                width: 100%;
                font-size: 12px;
                height: 30px;
                padding: 5px;
                box-sizing: border-box;
                margin-top: 5px;
            }

            /* Ensure Task Cells Stack Correctly */
            .task-cell {
                padding: 5px;
                width: 13%;
                text-align: center;
            }

            .task-wrapper {
                display: flex;
                flex-direction: column;
                align-items: center;
                width: 100%;
            }
        }

        /* Increase Input Size on Large Screens */
        @media (min-width: 769px) {
            .task-input {
                width: 100%;
                font-size: 22px; /* Increase font size */
                height: 100px; /* Increase height */
                padding: 12px; /* Increase padding for better UX */
                box-sizing: border-box;
            }
        }


    </style>
</head>
<body>
    <h1>Weekly Calendar</h1>
    <button onclick=""saveTasks()"">Save Tasks</button>

    <div class=""calendar-wrapper"">
        <div class=""week-navigation"">
            <button onclick=""changeWeek(-1)"">◀ Previous</button>
            <span id=""week-label""></span>
            <button onclick=""changeWeek(1)"">Next ▶</button>
        </div>

        <table>
            <thead>
                <tr>
                    <th>Time</th>
                    <th id=""monday-header"">Monday</th>
                    <th id=""tuesday-header"">Tuesday</th>
                    <th id=""wednesday-header"">Wednesday</th>
                    <th id=""thursday-header"">Thursday</th>
                    <th id=""friday-header"">Friday</th>
                    <th id=""saturday-header"">Saturday</th>
                    <th id=""sunday-header"">Sunday</th>
                </tr>
            </thead>
            <tbody id=""calendar-body"">
                <!-- Dynamic rows will be added here -->
            </tbody>
        </table>
    </div>
    <script>
        const API_URL = 'https://calendartaskerapp-awe7dpa5engueccr.israelcentral-01.azurewebsites.net/api/calendar';
        //const API_URL = 'https://localhost:7204/api/calendar';

        let allCalendarData = {};
        let currentWeekOffset = 0;
        const daysOfWeek = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
        let calendarData = {}; // Initialize the calendarData object to store loaded tasks

        function getWeekStart(offset) {
            const now = new Date();
            now.setDate(now.getDate() + offset * 7);
            const day = now.getDay();
            const diff = now.getDate() - day + (day === 0 ? -6 : 1);
            return new Date(now.setDate(diff));
        }

        function formatDate(date) {
            return date.toISOString().split('T')[0]; // YYYY-MM-DD format
        }

        function changeWeek(direction) {
            currentWeekOffset += direction;
            renderCalendar();
        }

        function formatWeekKey(date) {
            const year = date.getFullYear();
            const month = (date.getMonth() + 1).toString().padStart(2, '0');
            const day = date.getDate().toString().padStart(2, '0');
            return `${year}-${month}-${day}`;  // Returns a string like ""2025-03-10""
        }

        function saveTaskDataForWeek(currentWeekOffset) {
            const weekStart = getWeekStart(currentWeekOffset); // Get the start of the current week
            const weekKey = formatWeekKey(weekStart); // Format week start date as a key

            // Create a copy of calendarData for the current week if it doesn't exist
            if (!allCalendarData[weekKey]) {
                allCalendarData[weekKey] = {}; // Initialize for this week if not present
            }

            // Iterate through all days and hours of the current week
            for (let hour = 7; hour < 31; hour++) {  // 7 AM to 7 AM
                daysOfWeek.forEach(day => {
                    const dayCell = document.getElementById(`${day.toLowerCase()}-${hour}`);
                    if (!dayCell) {
                        console.log(`No cell found for ${day} at hour ${hour}`);
                        return;  // Skip this day-hour if no element is found
                    }

                    const input = dayCell.querySelector('.task-input');
                    const reminderSelect = dayCell.querySelector('.reminder-select');

                    // Check if there is any task input or reminder selected
                    if (input && reminderSelect) {
                        // Clone the weekStart date to avoid modifying the original weekStart
                        const clonedDate = new Date(weekStart);
                        clonedDate.setDate(weekStart.getDate() + daysOfWeek.indexOf(day)); // Set the correct date for the day

                        const taskData = {
                            task: input.value.trim(),
                            reminder: reminderSelect.value,
                            dateOnly: formatDate(clonedDate) // Use the cloned date
                        };

                        // Save the task data for the current day and hour in the current week
                        allCalendarData[weekKey][day] = allCalendarData[weekKey][day] || {};  // Initialize day if not present
                        allCalendarData[weekKey][day][hour] = taskData; // Store the task data
                    }
                });
            }
        }

        function renderCalendar() {
            const weekStart = getWeekStart(currentWeekOffset);
            const weekKey = formatWeekKey(weekStart); // Format the week key
            document.getElementById('week-label').textContent = `${weekStart.toDateString()}`;

            // Update weekday headers with actual dates
            for (let i = 0; i < 7; i++) {
                const currentDay = new Date(weekStart);
                currentDay.setDate(weekStart.getDate() + i);
                const formattedDate = formatDate(currentDay);
                document.getElementById(`${daysOfWeek[i].toLowerCase()}-header`).textContent = `${daysOfWeek[i]} (${formattedDate})`;
            }

            // Re-render table body
            const tableBody = document.getElementById('calendar-body');
            tableBody.innerHTML = '';  // Clear the table body before rendering the new data

            for (let hour = 7; hour < 31; hour++) { // 7 AM to 7 AM
                const row = document.createElement('tr');
                const timeCell = document.createElement('td');
                timeCell.textContent = `${hour % 24}:00`;
                row.appendChild(timeCell);

                daysOfWeek.forEach(day => {
                    const dayCell = document.createElement('td');
                    dayCell.classList.add('task-cell'); // Add class to the cell for styling

                    // Assign the correct ID to each day-cell, e.g., monday-7, tuesday-8, etc.
                    dayCell.id = `${day.toLowerCase()}-${hour}`;

                    const input = document.createElement('input');
                    input.classList.add('task-input');
                    input.placeholder = `Task for ${day} ${hour % 24}:00`;

                    const reminderSelect = document.createElement('select');
                    reminderSelect.classList.add('reminder-select');
                    reminderSelect.innerHTML = `
                            <option value=""none"">Just a Task</option>
                            <option value=""notification"">Task with Notification</option>
                            <option value=""call"">Task with Call</option>
                            <option value=""constant"">Task with Constant Reminder</option>
                        `;

                    // Format the current date for the cell
                    const currentDateCell = new Date(weekStart);
                    currentDateCell.setDate(weekStart.getDate() + daysOfWeek.indexOf(day));
                    const formattedCurrentDate = formatDate(currentDateCell);

                    // Check if there's data for the current week, day, and hour
                    const taskData = allCalendarData[weekKey] && allCalendarData[weekKey][day] && allCalendarData[weekKey][day][hour];
                    if (taskData && taskData.dateOnly === formattedCurrentDate) {
                        input.value = taskData.task || '';
                        reminderSelect.value = taskData.reminder || 'none';

                        // Add background color based on reminder
                        switch (taskData.reminder) {
                            case 'none':
                                dayCell.classList.add('green');
                                break;
                            case 'notification':
                                dayCell.classList.add('yellow');
                                break;
                            case 'call':
                                dayCell.classList.add('orange');
                                break;
                            case 'constant':
                                dayCell.classList.add('red');
                                break;
                            default:
                                break;
                        }
                    } else {
                        // Clear cell if no data
                        input.value = '';
                        reminderSelect.value = 'none';
                        dayCell.classList.remove('green', 'yellow', 'orange', 'red');
                    }

                    // Add event listener to change the background color when a reminder type is selected
                    reminderSelect.addEventListener('change', function () {
                        const selectedValue = this.value;
                        dayCell.classList.remove('green', 'yellow', 'orange', 'red');

                        switch (selectedValue) {
                            case 'none':
                                dayCell.classList.add('green'); // Green for ""None""
                                break;
                            case 'notification':
                                dayCell.classList.add('yellow'); // Yellow for ""Notification""
                                break;
                            case 'call':
                                dayCell.classList.add('orange'); // Orange for ""Call""
                                break;
                            case 'constant':
                                dayCell.classList.add('red'); // Red for ""Constant Reminder""
                                break;
                            default:
                                break;
                        }
                    });

                    const taskWrapper = document.createElement('div');
                    taskWrapper.classList.add('task-wrapper');

                    taskWrapper.appendChild(input);
                    taskWrapper.appendChild(document.createElement('br')); // Insert line break
                    taskWrapper.appendChild(reminderSelect);
                    dayCell.appendChild(taskWrapper);

                    row.appendChild(dayCell);
                });

                tableBody.appendChild(row);  // Add row to the table body
            }
        }



        function saveTasks() {

            saveTaskDataForWeek(currentWeekOffset)
            saveDataToServer(allCalendarData);
        }


        function saveDataToServer(data) {
            fetch(API_URL, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data),
            })
                .then(response => response.json())
                .then(data => {
                    console.log('Data saved to server:', data);
                })
                .catch(error => console.error('Error saving data:', error));
        }

        // Load data from the server
        function loadDataFromServer() {
            fetch(API_URL)
                .then(response => response.json())
                .then(data => {
                    allCalendarData = data || {}; // Ensure calendarData is updated with the loaded
                    renderCalendar();
                })
                .catch(error => console.error('Error loading data:', error));
        }

        // Load initial data
        loadDataFromServer();

        const calendarWrapper = document.querySelector('.calendar-wrapper');
        const weekNavigation = document.querySelector('.week-navigation');

        function centerWeekNavigation() {
            const scrollLeft = calendarWrapper.scrollLeft;
            const wrapperWidth = calendarWrapper.clientWidth;

            // Center `.week-navigation` relative to the visible portion
            weekNavigation.style.left = `${scrollLeft + wrapperWidth / 2}px`;
        }

        // Run initially to align correctly
        centerWeekNavigation();

        calendarWrapper.addEventListener('scroll', centerWeekNavigation);

    </script>
</body>
</html>

";
        }
    }


}
