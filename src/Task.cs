using Koopman.CheckPoint.Common;
using Koopman.CheckPoint.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Koopman.CheckPoint
{
    /// <summary>
    /// Check Point Task
    /// </summary>
    public class Task
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Task" /> class.
        /// </summary>
        /// <param name="session">The current session.</param>
        protected internal Task(Session session)
        {
            Session = session;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Color of the object
        /// </summary>
        [JsonProperty(PropertyName = "color")]
        public Colors Color { get; private set; }

        /// <summary>
        /// Comments string
        /// </summary>
        [JsonProperty(PropertyName = "comments")]
        public string Comments { get; private set; }

        /// <summary>
        /// Information about the domain the object belongs to.
        /// </summary>
        /// <value>The domain.</value>
        [JsonProperty(PropertyName = "domain", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public Domain Domain { get; private set; }

        /// <summary>
        /// Object icon
        /// </summary>
        [JsonProperty(PropertyName = "icon")]
        public string Icon { get; private set; }

        /// <summary>
        /// Gets the last update time.
        /// </summary>
        /// <value>The last update time.</value>
        [JsonProperty(PropertyName = "last-update-time")]
        [JsonConverter(typeof(CheckPointDateTimeConverter))]
        public DateTime LastUpdateTime { get; set; }

        /// <summary>
        /// Meta Information
        /// </summary>
        [JsonProperty(PropertyName = "meta-info", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public MetaInfo MetaInfo { get; private set; }

        /// <summary>
        /// Object name. Should be unique in the domain.
        /// </summary>
        /// <value>The object's name.</value>
        /// <exception cref="System.NotImplementedException"></exception>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }

        /// <summary>
        /// Gets the progress percentage.
        /// </summary>
        /// <value>The progress percentage.</value>
        [JsonProperty(PropertyName = "progress-percentage")]
        public int ProgressPercentage { get; private set; }

        /// <summary>
        /// Indicates whether the object is read-only
        /// </summary>
        [JsonProperty(PropertyName = "read-only")]
        public bool ReadOnly { get; private set; }

        /// <summary>
        /// Gets the start time.
        /// </summary>
        /// <value>The start time.</value>
        [JsonProperty(PropertyName = "start-time")]
        [JsonConverter(typeof(CheckPointDateTimeConverter))]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets the task status.
        /// </summary>
        /// <value>The status.</value>
        [JsonProperty(PropertyName = "status")]
        public TaskStatus Status { get; private set; }

        /// <summary>
        /// Suppressed
        /// </summary>
        /// <value>Suppressed.</value>
        [JsonProperty(PropertyName = "suppressed")]
        public bool Suppressed { get; private set; }

        /// <summary>
        /// Tags assigned to object
        /// </summary>
        [JsonProperty(PropertyName = "tags")]
        public Tag[] Tags { get; private set; }

        /// <summary>
        /// Gets the task details.
        /// </summary>
        /// <value>The task details.</value>
        [JsonProperty(PropertyName = "task-details")]
        public Details[] TaskDetails { get; private set; }

        /// <summary>
        /// Gets the task identifier.
        /// </summary>
        /// <value>The task identifier.</value>
        [JsonProperty(PropertyName = "task-id")]
        public string TaskID { get; private set; }

        /// <summary>
        /// Task name.
        /// </summary>
        /// <value>The task's name.</value>
        [JsonProperty(PropertyName = "task-name")]
        public string TaskName { get; private set; }

        /// <summary>
        /// Type of the object.
        /// </summary>
        /// <value>The type.</value>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; private set; }

        /// <summary>
        /// Object unique identifier.
        /// </summary>
        /// <value>The uid.</value>
        [JsonProperty(PropertyName = "uid")]
        public string UID { get; private set; }

        private Session Session { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString() => (string.IsNullOrWhiteSpace(TaskName)) ? TaskID : TaskName;

        /// <summary>
        /// Asynchronous wait call, that will complete once the test status is no longer In Progress.
        /// </summary>
        /// <param name="delay">The delay between checking current task status.</param>
        /// <param name="cancellationToken">The cancellation token, to stop waiting when triggered.</param>
        /// <param name="progress">To track progress.</param>
        /// <returns>
        /// <c>true</c> if task completed successfully; otherwise <c>false</c> if task failed
        /// </returns>
        public async System.Threading.Tasks.Task<bool> WaitAsync(
                int delay = 1000,
                CancellationToken cancellationToken = default,
                IProgress<int> progress = null
            )
        {
            var detailLevels = DetailLevels.Standard;
            while (true)
            {
                var data = new Dictionary<string, dynamic>
                {
                    { "task-id", TaskID },
                    { "details-level", detailLevels }
                };

                string jsonData = JsonConvert.SerializeObject(data, Session.JsonFormatting);

                string result = await Session.PostAsync("show-task", jsonData, cancellationToken);

                var results = JsonConvert.DeserializeObject<JObject>(result);
                var array = (JArray)results.GetValue("tasks");

                JsonConvert.PopulateObject(array.First.ToString(), this, new JsonSerializerSettings() { Converters = { new SessionConstructorConverter(Session) } });

                if (detailLevels == DetailLevels.Full) { return Status == TaskStatus.Succeeded; }
                if (Status != TaskStatus.InProgress)
                {
                    // Set Detail level to full and do another loop just to populate Task with
                    // updated task details before returning
                    detailLevels = DetailLevels.Full;
                }
                else
                {
                    progress?.Report(ProgressPercentage);
                    await System.Threading.Tasks.Task.Delay(delay, cancellationToken);
                }
            }
        }

        #endregion Methods

        #region Enums

        /// <summary>
        /// Task Statuses
        /// </summary>
        [JsonConverter(typeof(EnumConverter), EnumConverter.StringCases.Lowercase, " ")]
        public enum TaskStatus
        {
            /// <summary>
            /// In progress
            /// </summary>
            InProgress,

            /// <summary>
            /// Completed successfully
            /// </summary>
            Succeeded,

            /// <summary>
            /// Completed with warnings
            /// </summary>
            SucceededWithWarnings,

            /// <summary>
            /// Completed with partial success
            /// </summary>
            PartiallySucceeded,

            /// <summary>
            /// Failed
            /// </summary>
            Failed
        }

        #endregion Enums

        #region Classes

        /// <summary>
        /// Task details
        /// </summary>
        public class Details
        {
            #region Properties

            /// <summary>
            /// <para type="description">Information about the domain the object belongs to..</para>
            /// </summary>
            [JsonProperty(PropertyName = "domain", ObjectCreationHandling = ObjectCreationHandling.Replace)]
            public Domain Domain
            {
                get; private set;
            }

            /// <summary>
            /// Gets the response error message.
            /// </summary>
            /// <value>The response error message.</value>
            public string ResponseError => (ResponseError64 == null) ? null : System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(ResponseError64));

            /// <summary>
            /// Gets the response message.
            /// </summary>
            /// <value>The response message.</value>
            public string ResponseMessage => (ResponseMessage64 == null) ? null : System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(ResponseMessage64));

            /// <summary>
            /// Gets the uid.
            /// </summary>
            /// <value>The uid.</value>
            [JsonProperty(PropertyName = "uid")]
            public string UID { get; private set; }

            [JsonProperty(PropertyName = "responseError")]
            private string ResponseError64 { get; set; }

            [JsonProperty(PropertyName = "responseMessage")]
            private string ResponseMessage64 { get; set; }

            #endregion Properties
        }

        #endregion Classes
    }
}