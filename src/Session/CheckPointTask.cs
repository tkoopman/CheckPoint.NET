using Koopman.CheckPoint.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Koopman.CheckPoint
{
    public partial class Session
    {
        #region Methods

        /// <summary>
        /// Find a task.
        /// </summary>
        /// <param name="taskID">The task identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the CheckPointTask
        /// </returns>
        public async Task<CheckPointTask> FindTask
            (
                string taskID,
                CancellationToken cancellationToken = default
            )
        {
            var data = new Dictionary<string, dynamic>
            {
                { "task-id", taskID },
                { "details-level", DetailLevels.Full }
            };

            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

            string result = await PostAsync("show-task", jsonData, cancellationToken);

            var results = JsonConvert.DeserializeObject<JObject>(result);
            var array = (JArray)results.GetValue("tasks");

            var tasks = JsonConvert.DeserializeObject<CheckPointTask[]>(array.ToString(), new JsonSerializerSettings() { Converters = { new SessionConstructorConverter(this) } });

            return tasks?[0];
        }

        /// <summary>
        /// Runs the script on multiple targets.
        /// </summary>
        /// <param name="scriptName">Name of the script.</param>
        /// <param name="script">The script.</param>
        /// <param name="args">The script arguments.</param>
        /// <param name="targets">The targets.</param>
        /// <param name="comments">Script comments.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a read-only
        /// dictionary detailing the Check Point task ID for each target.
        /// </returns>
        public async Task<IReadOnlyDictionary<string, string>> RunScript
            (
                string scriptName,
                string script,
                string args,
                string[] targets,
                string comments = null,
                CancellationToken cancellationToken = default
            )
        {
            var data = new Dictionary<string, dynamic>
            {
                { "script-name", scriptName },
                { "script", script },
                { "args", args },
                { "targets", targets },
                { "comments", comments }
            };

            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

            string result = await PostAsync("run-script", jsonData, cancellationToken);

            var results = JsonConvert.DeserializeObject<JObject>(result);
            var array = (JArray)results.GetValue("tasks");

            var dicResults = new Dictionary<string, string>();

            foreach (var r in array)
            {
                var j = r as JObject;
                dicResults.Add(j.GetValue("target").ToString(), j.GetValue("task-id").ToString());
            }

            return new ReadOnlyDictionary<string, string>(dicResults);
        }

        /// <summary>
        /// Runs the script on a single target.
        /// </summary>
        /// <param name="scriptName">Name of the script.</param>
        /// <param name="script">The script.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="target">The target.</param>
        /// <param name="comments">The script comments.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the Check
        /// Point task ID
        /// </returns>
        public async Task<string> RunScript
            (
                string scriptName,
                string script,
                string args,
                string target,
                string comments = null,
                CancellationToken cancellationToken = default
            )
        {
            var results = await RunScript(
                    scriptName,
                    script,
                    args,
                    new string[] { target },
                    comments,
                    cancellationToken
                );

            return results?.Values.First();
        }

        #endregion Methods
    }
}