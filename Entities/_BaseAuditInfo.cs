using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPortalV8.Entities
{
    public class _BaseAuditInfo
    {
        /// <summary>
        /// Auto filled
        /// </summary>
        [JsonIgnore]
        public DateTime Created { get; set; }

        /// <summary>
        /// Auto filled
        /// </summary>
        [JsonIgnore]
        public DateTime Modified { get; set; }
    }
}
