using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities.Navigations;
public class NavigationRole:ILookupTable<byte>
{
    public static class Contants
    {
        public const byte Role1 = 1; //This is just an example. Real Values needs to be determined.
    }

    public byte Id { get; set; }
    public required string Name { get; set; }
}
