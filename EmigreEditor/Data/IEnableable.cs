﻿using ObjectEditor.Json;

namespace Emigre.Data
{
    public interface IEnableable : GuidDataObject, IIgnorable
    {
        bool GetDefaultEnabled();
    }
}
