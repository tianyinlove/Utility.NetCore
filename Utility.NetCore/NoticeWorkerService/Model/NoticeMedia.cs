using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoticeWorkerService.Model
{
    /// <summary>
    /// 素材
    /// </summary>
    public class NoticeMedia
    {
        /// <summary>
        /// 素材ID
        /// </summary>
        [JsonProperty("media_id")]
        public string Media_Id { get; set; }
    }
}
