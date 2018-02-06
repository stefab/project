using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;

namespace EtsyRobot.Engine.JobModel
{
    public enum JobState
    {
        [Display(Name = "Not started")]
        Incomplete = 0,
        [Display(Name = "Completed")]
        Completed = 1,
        [Display(Name = "Completed with issues")]
        CompletedWithIssues = 2,
        [Display(Name = "Failed")]
        Failed = 3
    }

    [JsonObject]
    public sealed class GameResult
    {
        JobState Result = JobState.Incomplete;
        [JsonProperty("processed")]
        DateTime? Processed =  null;
        [JsonProperty("error")]
        string ErrorMessage;
        [JsonProperty("elapsed")]
        public double? ExecutionTime { get; set; }
    }
}
