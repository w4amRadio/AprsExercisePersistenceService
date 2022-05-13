using AprsPersistenceService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AprsPersistenceService.Interfaces
{
    public interface ITncLogReader
    {
        IEnumerable<AprsModel> ParseContinuously();
    }
}
