﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helbreath.Launcher
{
    public interface IVersionProvider : IRemoteVersionProvider, ILocalVersionProvider
    {

    }
}
