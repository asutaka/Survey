﻿using System.Collections.Generic;

namespace SLib.Model.APIModel
{
    public class HSXTudoanhModel
    {
        public List<HSXRowModel> rows { get; set; }
    }

    public class HSXRowModel
    {
        public List<string> cell { get; set; }
    }
}
