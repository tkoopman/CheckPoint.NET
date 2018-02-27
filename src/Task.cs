using Koopman.CheckPoint.Common;
using Koopman.CheckPoint.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Koopman.CheckPoint
{
    public class Task
    {
        #region Constructors

        protected internal Task(Session session)
        {
            Session = session;
        }

        #endregion Constructors

        #region Properties

        [JsonProperty(PropertyName = "progress-percentage")]
        public int ProgressPercentage
        {
            get; private set;
        }

        [JsonProperty(PropertyName = "status")]
        public TaskStatus Status
        {
            get; private set;
        }

        [JsonProperty(PropertyName = "task-details")]
        public Details[] TaskDetails
        {
            get; private set;
        }

        [JsonProperty(PropertyName = "task-id")]
        public string TaskID
        {
            get; private set;
        }

        private Session Session { get; }

        #endregion Properties

        #region Methods

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

        [JsonConverter(typeof(EnumConverter), EnumConverter.StringCases.Lowercase, " ")]
        public enum TaskStatus
        {
            InProgress,
            Succeeded,
            SucceededWithWarnings,
            PartiallySucceeded,
            Failed
        }

        #endregion Enums

        #region Classes

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

            public string ResponseMessage
            {
                get => (ResponseMessage64 == null) ? null : System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(ResponseMessage64));
            }

            [JsonProperty(PropertyName = "uid")]
            public string UID { get; private set; }

            [JsonProperty(PropertyName = "responseMessage")]
            private string ResponseMessage64 { get; set; }

            #endregion Properties
        }

        #endregion Classes
    }
}