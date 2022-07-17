using AprsPersistenceService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AprsPersistenceService.Interfaces
{
    public interface IAprsPacketParser
    {
        AprsModel ParseAprsPacket(List<string> textBlock);
    }
}
