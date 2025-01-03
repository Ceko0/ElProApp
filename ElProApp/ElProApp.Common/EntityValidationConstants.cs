namespace ElProApp.Common
{    
    public static class EntityValidationConstants
    {
        public static class Employee
        {
            public const int NameMinLength = 2;
            public const int NameMaxLength = 20;
        }

        public static class Building
        {
            public const int BuildingNameMinLength = 3;
            public const int BuildingNameMaxLength = 50;
            public const int LocationMinLength = 10;
            public const int LocationMaxLength = 100;
        }

        public static class Team
        {
            public const int NameMinLength = 5;
            public const int NameMaxLength = 100;
        }

        public static class Job
        {
            public const int NameMinLength = 5;
            public const int NameMaxLength = 50;
        }

        public static class JobDone
        {
            public const int NameMinLength = 5;
            public const int NameMaxLength = 50;
        }

        public static class CalculationAction
        {
            public const string Add = "add";
            public const string Remove = "remove";
        }
    }
}
