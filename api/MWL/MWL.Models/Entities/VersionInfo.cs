using System;
using System.Collections.Generic;
using System.Text;

namespace MWL.Models.Entities
{
    public class VersionInfo
    {
        public string Build { get; set; }
        public string Environment { get; set; }
        public string Runtime { get; set; }
        public string ServerDatetime { get; set; }
    }
}
