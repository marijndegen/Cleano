# Clearo - Cleaning Checklist Manager

A WPF application for managing cleaning tasks and generating PDF checklists.

## Features

### Admin Tab
- Create and manage task groups (e.g., Kitchen, Bedroom, General, Washing Machines)
- Add tasks to groups with specific frequencies:
  - Daily
  - Weekly
  - Biweekly
  - Monthly
  - Quarterly
  - Yearly
- Delete task groups and tasks (right-click context menu)
- All settings are automatically saved to your user profile

### User Tab
- View all task groups and their associated tasks
- Tasks are automatically sorted by due date (most urgent first)
- Visual indicators show overdue tasks (highlighted in red)
- Select tasks to complete using checkboxes
- Generate PDF checklist with selected tasks
- When PDF is generated:
  - Completion date is recorded for each selected task
  - Checkboxes are automatically cleared
  - Due dates are updated based on task frequency

## Data Storage

All task data is automatically saved in:
`%APPDATA%\Clearo\taskdata.json`

This ensures your settings persist across sessions and are specific to your Windows user account.

## PDF Generation

The application uses QuestPDF library to generate professional PDF checklists. When you click "Generate PDF and Mark as Complete":

1. A save dialog appears to choose the PDF location
2. Only selected tasks are included in the PDF
3. Tasks are organized by group
4. Overdue tasks are clearly marked
5. Task frequency is shown for reference
6. Completion timestamps are recorded

## Usage Instructions

1. **First Time Setup (Admin Tab)**
   - Switch to the "Admin Settings" tab
   - Create task groups (e.g., "Kitchen")
   - Select a group and add tasks with appropriate frequencies
   - Settings save automatically

2. **Daily Use (User Tab)**
   - Switch to the "User View" tab
   - Review tasks - overdue items appear at the top with red highlighting
   - Check off completed or to-be-completed tasks
   - Click "Generate PDF and Mark as Complete" to:
     - Create a printable checklist
     - Record completion dates
     - Reset checkboxes for next use

## Technical Details

- Built with .NET 10 and WPF
- Uses QuestPDF (Community License) for PDF generation
- Data persistence with System.Text.Json
- MVVM architecture pattern
