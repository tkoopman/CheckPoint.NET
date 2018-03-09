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

        protected internal Task(Session session)
        {
            Session = session;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the progress percentage.
        /// </summary>
        /// <value>The progress percentage.</value>
        [JsonProperty(PropertyName = "progress-percentage")]
        public int ProgressPercentage
        {
            get; private set;
        }

        /// <summary>
        /// Gets the task status.
        /// </summary>
        /// <value>The status.</value>
        [JsonProperty(PropertyName = "status")]
        public TaskStatus Status
        {
            get; private set;
        }

        /// <summary>
        /// Gets the task details.
        /// </summary>
        /// <value>The task details.</value>
        [JsonProperty(PropertyName = "task-details")]
        public Details[] TaskDetails
        {
            get; private set;
        }

        /// <summary>
        /// Gets the task identifier.
        /// </summary>
        /// <value>The task identifier.</value>
        [JsonProperty(PropertyName = "task-id")]
        public string TaskID
        {
            get; private set;
        }

        private Session Session { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Asynchronous wait call, that will complete once the test status is no longer In Progress.
        /// </summary>
        /// <param name="delay">The delay between checking current task status.</param>
        /// <param name="cancellationToken">The cancellation token, to stop waiting when triggered.</param>
        /// <param name="progress">To track progress.</param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<bool> WaitAsync(
                int delay = 1000,
                CancellationToken cancellationToken = default(CancellationToken),
                IProgress<int> progress = null
            )
        {
            DetailLevels detailLevels = DetailLevels.Standard;
            while (true)
            {
                Dictionary<string, dynamic> data = new Dictionary<string, dynamic>
                {
                    { "task-id", TaskID },
                    { "details-level", detailLevels }
                };

                string jsonData = JsonConvert.SerializeObject(data, Session.JsonFormatting);

                string result = await Session.PostAsync("show-task", jsonData, cancellationToken);

                JObject results = JsonConvert.DeserializeObject<JObject>(result);
                JArray array = (JArray)results.GetValue("tasks");

                JsonConvert.PopulateObject(array.First.ToString(), this, new JsonSerializerSettings() { Converters = { new SessionConstructorConverter(Session) } });

                if (detailLevels == DetailLevels.Full) { return Status == TaskStatus.Succeeded; }
                if (Status != TaskStatus.InProgress)
                { detailLevels = DetailLevels.Full; }
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
        /// Task destails
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
            /// Gets the response message.
            /// </summary>
            /// <value>The response message.</value>
            public string ResponseMessage
            {
                get => (ResponseMessage64 == null) ? null : System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(ResponseMessage64));
            }

            /// <summary>
            /// Gets the uid.
            /// </summary>
            /// <value>The uid.</value>
            [JsonProperty(PropertyName = "uid")]
            public string UID { get; private set; }

            [JsonProperty(PropertyName = "responseMessage")]
            private string ResponseMessage64 { get; set; }

            #endregion Properties
        }

        #endregion Classes
    }
}