using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Dtos.Position
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class UpdatePosition
    {
        [JsonProperty("PX")]
        public string Px { get; set; }

        [JsonProperty("PY")]
        public string Py { get; set; }

        [JsonProperty("PZ")]
        public string Pz { get; set; }

        [JsonProperty("RX")]
        public string Rx { get; set; }

        [JsonProperty("RY")]
        public string Ry { get; set; }

        [JsonProperty("RZ")]
        public string Rz { get; set; }
    }
}
