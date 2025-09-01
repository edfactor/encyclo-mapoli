using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Attributes;
[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
public sealed class NoMemberDataExposedAttribute : Attribute
{
}
