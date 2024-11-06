namespace ElProApp.Data.SeedData
{
    using System;
    using System.Collections.Generic;

    public class MappingData
    {
        public List<Mapping> Mappings { get; set; } = [];
    }

    public class Mapping
    {
        public Guid TeamId { get; set; }
        public List<Guid> BuildingIds { get; set; } = [];
        public List<Guid> EmployeeIds { get; set; } = [];
        public List<Guid> JobDoneIds { get; set; } = [];
    }
}
