using ElProApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElProApp.Services.Data.Interfaces
{
    public interface IJobService
    {
        IQueryable<Job> GetAllAttached();
    }
}
