﻿using Microsoft.AspNetCore.Mvc;
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
            //var htmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Calendar.html");
            var htmlContent = GetHtmlContent();
            return Content(htmlContent, "text/html");

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
        body {
            font-family: Arial, sans-serif;
            padding: 20px;
            background-color: #f4f4f9;
            text-align: center;
        }

        h1 {
            margin-bottom: 20px;
        }

        table {
            width: 100%;
            border-collapse: collapse;
            margin: 20px 0;
            table-layout: fixed;
        }

        table, th, td {
            border: 1px solid #ddd;
        }

        th, td {
            padding: 20px; /* Increased row height */
            text-align: center;
        }

        th {
            background-color: #4CAF50;
            color: white;
        }

        td {
            background-color: #f9f9f9;
        }

        .time-column {
            background-color: #f1f1f1;
            font-weight: bold;
            position: sticky;
            left: 0;
            z-index: 1; /* To make sure the time column stays above other cells */
        }

        .task-input {
            width: 100%;
            height: 50px; /* Make the task input field larger */
            padding: 10px;
            margin-top: 5px;
            box-sizing: border-box; /* Ensure padding does not affect width */
        }

        .reminder-select {
            width: 100%;
            padding: 5px;
            margin-top: 5px;
            font-size: 12px; /* Make the dropdown smaller */
            box-sizing: border-box;
        }

        .save-button {
            margin-top: 20px;
            padding: 10px 20px;
            font-size: 16px;
            background-color: #4CAF50;
            color: white;
            border: none;
            cursor: pointer;
        }

            .save-button:hover {
                background-color: #45a049;
            }
    </style>
</head>
<body>

    <h1>7 AM to 7 AM Weekly Calendar</h1>

    <table>
        <thead>
            <tr>
                <th class=""time-column"">Time</th>
                <th>Monday</th>
                <th>Tuesday</th>
                <th>Wednesday</th>
                <th>Thursday</th>
                <th>Friday</th>
                <th>Saturday</th>
                <th>Sunday</th>
            </tr>
        </thead>
        <tbody id=""calendar-body"">
            <!-- Dynamic rows will be added here via JavaScript -->
        </tbody>
    </table>

    <button id=""saveButton"" class=""save-button"">Save Data</button>

    <script>
    // Time slots from 7 AM to 7 AM (24 hours)
    const hours = Array.from({ length: 24 }, (_, i) => (i + 7) % 24); // This makes hours 7 to 7
    const daysOfWeek = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];

    let calendarData = JSON.parse(localStorage.getItem('calendarData')) || {}; // Load data from localStorage

    const tableBody = document.getElementById('calendar-body');

    // Render the calendar with time slots and task inputs
    function renderCalendar() {
      tableBody.innerHTML = ''; // Clear previous table content

      // Create the rows for each hour slot
      hours.forEach(hour => {
        const row = document.createElement('tr');

        // Create the hour column (7:00, 8:00, etc.)
        const timeCell = document.createElement('td');
        timeCell.textContent = `${hour}:00`;
        timeCell.classList.add('time-column');
        row.appendChild(timeCell);

        // Create the cells for each day of the week
        daysOfWeek.forEach(day => {
          const dayCell = document.createElement('td');
          const input = document.createElement('input');
          input.classList.add('task-input');
          input.placeholder = `Enter task for ${day} ${hour}:00`;

          // Create a select dropdown for reminder options
          const reminderSelect = document.createElement('select');
          reminderSelect.classList.add('reminder-select');
          reminderSelect.innerHTML = `
            <option value=""none"">Just a Task</option>
            <option value=""notification"">Task with Notification</option>
            <option value=""call"">Task with Call</option>
            <option value=""constant"">Task with Constant Reminder</option>
          `;

          // Load any saved task and reminder type for this specific hour and day
          if (calendarData[day] && calendarData[day][hour]) {
            input.value = calendarData[day][hour].task;
            reminderSelect.value = calendarData[day][hour].reminder;
            applyReminderColor(dayCell, reminderSelect.value);
          }

          // Add event listener to save the task and reminder type to localStorage on input change
          input.addEventListener('input', () => {
            saveData(day, hour, input.value, reminderSelect.value, dayCell);
          });

          reminderSelect.addEventListener('change', () => {
            saveData(day, hour, input.value, reminderSelect.value, dayCell);
          });

          // Add the input and reminder select to the day cell
          dayCell.appendChild(input);
          dayCell.appendChild(reminderSelect);
          row.appendChild(dayCell);
        });

        tableBody.appendChild(row);
      });
    }

    // Function to apply the color to the cell based on the reminder type
    function applyReminderColor(cell, reminder) {
      if (reminder === 'notification') {
        cell.style.backgroundColor = '#ADD8E6'; // Light Blue
      } else if (reminder === 'call') {
        cell.style.backgroundColor = '#FFCC99'; // Light Orange
      } else if (reminder === 'constant') {
        cell.style.backgroundColor = '#FFB6C1'; // Light Red
      } else {
        cell.style.backgroundColor = '#f9f9f9'; // Default background
      }
    }

    // Function to save task data and reminder type to localStorage
    function saveData(day, hour, task, reminder, cell) {
      if (!calendarData[day]) {
        calendarData[day] = {};
      }
      calendarData[day][hour] = { task, reminder };

      // Apply the correct color based on reminder type
      applyReminderColor(cell, reminder);

      // Save the updated data to localStorage
      localStorage.setItem('calendarData', JSON.stringify(calendarData));
    }

    // Function to save data to the server
    function saveDataToServer() {
      fetch('https://calendartaskerapp-awe7dpa5engueccr.israelcentral-01.azurewebsites.net/api/calendar', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(calendarData),
      })
        .then(response => response.json())
        .then(data => {
          console.log('Data saved to server:', data);
        })
        .catch(error => console.error('Error saving data:', error));
    }

    // Event listener for the Save button
    document.getElementById('saveButton').addEventListener('click', () => {
      saveDataToServer();
    });

    // Load saved data from server and render the calendar
    function loadDataFromServer() {
      fetch('https://calendartaskerapp-awe7dpa5engueccr.israelcentral-01.azurewebsites.net/api/calendar')
        .then(response => response.json())
        .then(data => {
          calendarData = data || {};
          renderCalendar();
        })
        .catch(error => console.error('Error loading data:', error));
    }

    // Initial load
    loadDataFromServer();
    </script>

</body>
</html>
";
        }
    }


}
