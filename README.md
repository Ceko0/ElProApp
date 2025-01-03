# Job Earnings Calculator (El Pro Bg Job Done App)

## Description
The **Job Earnings Calculator** is a web application designed to track and calculate the total earnings of workers based on the jobs done within a specific period. The application manages various buildings and the work done by different teams, providing a way to track progress and calculate payments accordingly.

Workers can input information about the quantity of work completed and the time needed to finish specific tasks. The app automatically calculates the owed earnings for each worker based on the tasks performed. The project supports multiple roles to ensure smooth project management, including an administrator, office manager, technician, and workers.

## Features
- **Task Management**: Workers can add completed tasks from a predefined list or create new tasks, specifying the quantity and time to complete them.
- **Earnings Calculation**: Automatically calculates the earnings of workers based on the tasks they’ve completed.
- **Role-Based Access**: Different roles (administrator, office manager, technician, and workers) with specific permissions:
    - **Administrator**: Can manage everything, including restoring deleted records and resetting payment amounts.
    - **Office Manager**: Similar rights to the administrator but cannot restore deleted records or reset payments.
    - **Technician**: Can edit or delete buildings and jobs but cannot manage users or reset payment amounts.
    - **Worker**: Can create new jobs and register completed tasks but has no access to admin-level features.
- **Report Generation**: Generate and download detailed reports on tasks, workers, and payment summaries for a specified period.

## Technologies Used
- **Programming Language**: C#
- **Framework**: ASP.NET Core with Razor Pages
- **Database**: MSSQL with Entity Framework Core
- **Frontend**: HTML, CSS, and Bootstrap
- **Authentication**: ASP.NET Identity
- **Version Control**: GitHub

## How It Works
1. **Task Input**: Workers select a task from the list or create a new one, specifying the task description, quantity, and time needed to complete it.
2. **Period Selection**: A specific period can be chosen to calculate earnings for each worker based on completed tasks.
3. **Role-Based Access**: Different roles have distinct permissions to manage the project data (administrators have the highest level of access, technicians and workers have limited capabilities).
4. **Report Generation**: Detailed earnings reports are available for download.
5. **Secure User Management**: The administrator and office manager are responsible for assigning roles to new users.

## Setup Instructions
1. Clone the repository:
    ```bash
    git clone https://github.com/Ceko0/ElProApp.git
    ```
2. Open the solution in Visual Studio.
3. Configure the database connection string in `appsettings.json`.
4. Apply migrations to create the database schema:
    ```bash
    dotnet ef database update
    ```
5. Run the application:
    ```bash
    dotnet run
    ```

## Future Enhancements
- Add support for multiple languages.
- Include graphical visualizations of worker performance over time.
- Implement export to Excel or PDF functionality for reports.

## Contributing
Contributions are welcome! Please create a pull request with a detailed description of the changes you propose.

## License
This project is licensed under the MIT License.

## Contact
For any questions or feedback, feel free to contact me:

- **Email**: cvetomirivanov1986@gmail.com
- **GitHub**: [Ceko0](https://github.com/Ceko0)
- **LinkedIn**: [Tsvetomir Ivanov](https://www.linkedin.com/in/tsvetomir-ivanov/)
