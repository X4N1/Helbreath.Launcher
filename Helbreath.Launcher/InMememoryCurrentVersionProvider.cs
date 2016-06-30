using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helbreath.Launcher
{
    public class InMememoryCurrentVersionProvider : ICurrentVersionProvider
    {
        public double GetCurrentVersionFromComputer()
        {
            return 0.1;
        }
    }
}
