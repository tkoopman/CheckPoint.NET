using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Koopman.CheckPoint
{
    public partial class Session
    {
        #region Methods

        /// <summary>
        /// Installs the policy to gateways.
        /// </summary>
        /// <param name="policy">The policy to install.</param>
        /// <param name="targets">The target gateways.</param>
        /// <param name="access">if set to <c>true</c> installs the access policy.</param>
        /// <param name="threatPrevention">
        /// if set to <c>true</c> installs the threat prevention policy.
        /// </param>
        /// <param name="installOnAllClusterMembersOrFail">
        /// if set to <c>true</c> will fail if it cannot install policy to all cluster members. if
        /// set to <c>false</c> can complete with partial success if not all cluster members available.
        /// </param>
        /// <param name="prepareOnly">if set to <c>true</c> will prepare only.</param>
        /// <param name="revision">The revision of the policy to install.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task ID</returns>
        public async Task<string> InstallPolicy
            (
                string policy,
                string[] targets,
                bool access,
                bool threatPrevention,
                bool installOnAllClusterMembersOrFail = true,
                bool prepareOnly = false,
                string revision = null,
                CancellationToken cancellationToken = default
            )
        {
            var data = new Dictionary<string, dynamic>
            {
                { "policy-package", policy },
                { "targets", targets },
                { "access", access },
                { "threat-prevention", threatPrevention },
                { "install-on-all-cluster-members-or-fail", installOnAllClusterMembersOrFail },
                { "prepare-only", prepareOnly },
                { "revision", revision }
            };

            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

            string result = await PostAsync("install-policy", jsonData, cancellationToken);

            var taskID = JsonConvert.DeserializeObject<JObject>(result);

            return taskID.GetValue("task-id")?.ToString();
        }

        /// <summary>
        /// Verifies the policy.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task ID</returns>
        public async Task<string> VerifyPolicy
            (
                string policy,
                CancellationToken cancellationToken = default
            )
        {
            var data = new Dictionary<string, dynamic>
            {
                { "policy-package", policy }
            };

            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

            string result = await PostAsync("verify-policy", jsonData, cancellationToken);

            var taskID = JsonConvert.DeserializeObject<JObject>(result);

            return taskID.GetValue("task-id")?.ToString();
        }

        #endregion Methods
    }
}