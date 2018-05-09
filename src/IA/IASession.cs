using Koopman.CheckPoint.Common;
using Koopman.CheckPoint.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Koopman.CheckPoint.IA
{
    public class IASession : HttpSession
    {
        #region Constructors

        public IASession(string gateway, string sharedSecret, string certificateHash = null, int port = 443, CertificateValidation certificateValidation = CertificateValidation.Auto, TextWriter debugWriter = null, bool indentJson = false, int maxConnections = 3) :
            base($"https://{gateway}:{port}/_IA_API/v1.0", certificateValidation, certificateHash, debugWriter, indentJson, maxConnections)
        {
            SharedSecret = sharedSecret;
        }

        #endregion Constructors

        #region Fields

        private readonly string SharedSecret;
        private JObject CurrentBatch;
        private JArray CurrentRequests;
        private string Lock = "";
        private int MaxBatchSize;
        private Action<AddIdentityResponse> OutputAddResult;
        private Action<ShowIdentityResponse> OutputShowResult;
        private Action<DeleteIdentityResponse> OutputDeleteResult;

        #endregion Fields

        #region Properties

        public Command BatchCommand { get; private set; }

        #endregion Properties

        #region Enums

        public enum Command
        {
            None,
            AddIdentity,
            ShowIdentity,
            DeleteIdentity
        }

        #endregion Enums

        #region Methods

        public async Task<AddIdentityResponse> AddIdentity(string ipAddress, string user = null, string machine = null, string domain = null, int sessionTimeout = 43200, bool? fetchUserGroups = null, bool? fetchMachineGroups = null, bool? calculateRoles = null, string[] userGroups = null, string[] machineGroups = null, string[] roles = null, string machineOS = null, string hostType = null)
        {
            var req = new JObject();
            req.AddIfNotNull("ip-address", ipAddress);
            req.AddIfNotNull("user", user);
            req.AddIfNotNull("machine", machine);
            req.AddIfNotNull("domain", domain);
            req.AddIfNotNull("session-timeout", sessionTimeout);
            req.AddIfNotNull("fetch-user-groups", fetchUserGroups);
            req.AddIfNotNull("fetch-machine-groups", fetchMachineGroups);
            req.AddIfNotNull("user-groups", userGroups);
            req.AddIfNotNull("machine-groups", machineGroups);
            req.AddIfNotNull("calculate-roles", calculateRoles);
            req.AddIfNotNull("roles", roles);
            req.AddIfNotNull("machine-os", machineOS);
            req.AddIfNotNull("host-type", hostType);

            if (BatchCommand != Command.AddIdentity)
            {
                req.Add("shared-secret", SharedSecret);
                var result = await ProcessRequest<AddIdentityResponse>(Command.AddIdentity, req);
                return result;
            }
            else
            {
                await AddRequestToBatch(req);
                return null;
            }
        }

        public async Task<DeleteIdentityResponse> DeleteIdentity(string ipAddress)
        {
            var req = new JObject();
            req.AddIfNotNull("ip-address", ipAddress);
            if (BatchCommand != Command.DeleteIdentity)
            {
                req.Add("shared-secret", SharedSecret);
                return await ProcessRequest<DeleteIdentityResponse>(Command.DeleteIdentity, req);
            }
            else
            {
                await AddRequestToBatch(req);
                return null;
            }
        }

        public async Task Flush(bool stopBatch = false)
        {
            var command = BatchCommand;
            var batch = GetCurrentBatchAndClear(stopBatch);

            var result = await ProcessRequest<JObject>(command, batch);
            ProcessBatchResponse(command, result);
        }

        public void ResetBatch(bool stopBatch = false) => GetCurrentBatchAndClear(stopBatch);

        public async Task<ShowIdentityResponse> ShowIdentity(string ipAddress)
        {
            var req = new JObject();
            req.AddIfNotNull("ip-address", ipAddress);
            if (BatchCommand != Command.ShowIdentity)
            {
                req.Add("shared-secret", SharedSecret);
                return await ProcessRequest<ShowIdentityResponse>(Command.ShowIdentity, req);
            }
            else
            {
                await AddRequestToBatch(req);
                return null;
            }
        }

        public void StartAddBatch(Action<AddIdentityResponse> outputAction = null, int maxBatchSize = 20)
        {
            lock (Lock)
            {
                if (BatchCommand != Command.None)
                    throw new Exception("Batch already in progress.");

                BatchCommand = Command.AddIdentity;
                OutputAddResult = outputAction;
                MaxBatchSize = maxBatchSize;
            }
        }

        public void StartShowBatch(Action<ShowIdentityResponse> outputAction = null, int maxBatchSize = 20)
        {
            lock (Lock)
            {
                if (BatchCommand != Command.None)
                    throw new Exception("Batch already in progress.");

                BatchCommand = Command.ShowIdentity;
                OutputShowResult = outputAction;
                MaxBatchSize = maxBatchSize;
            }
        }

        public void StartDeleteBatch(Action<DeleteIdentityResponse> outputAction = null, int maxBatchSize = 20)
        {
            lock (Lock)
            {
                if (BatchCommand != Command.None)
                    throw new Exception("Batch already in progress.");

                BatchCommand = Command.DeleteIdentity;
                OutputDeleteResult = outputAction;
                MaxBatchSize = maxBatchSize;
            }
        }
        private async Task AddRequestToBatch(JObject request)
        {
            JObject processBatch = null;
            Command command;
            lock (Lock)
            {
                command = BatchCommand;
                if (CurrentBatch == null)
                {
                    CurrentBatch = new JObject
                    {
                        { "shared-secret", SharedSecret }
                    };
                    CurrentRequests = new JArray();
                }
                CurrentRequests.Add(request);
                if (CurrentRequests.Count >= MaxBatchSize)
                    processBatch = GetCurrentBatchAndClear(false);
            }

            var result = await ProcessRequest<JObject>(command, processBatch);
            ProcessBatchResponse(command, result);
        }

        private JObject GetCurrentBatchAndClear(bool stopBatch)
        {
            lock (Lock)
            {
                JObject result = null;
                if (CurrentRequests.Count > 0)
                {
                    result = CurrentBatch;
                    if (result != null)
                        result.Add("requests", CurrentRequests);
                }

                CurrentBatch = null;
                CurrentRequests = null;
                if (stopBatch)
                    BatchCommand = Command.None;

                return result;
            }
        }

        private void ProcessBatchResponse(Command command, JObject result)
        {
            if (result == null) return;
            switch (command)
            {
                case Command.AddIdentity:
                    if (result?.GetValue("responses") is JArray addArray)
                        foreach (var r in addArray.ToObject<AddIdentityResponse[]>())
                            OutputAddResult(r);
                    break;
                case Command.ShowIdentity:
                    if (result?.GetValue("responses") is JArray showArray)
                        foreach (var r in showArray.ToObject<ShowIdentityResponse[]>())
                            OutputShowResult(r);
                    break;
                case Command.DeleteIdentity:
                    if (result?.GetValue("responses") is JArray delArray)
                        foreach (var r in delArray.ToObject<DeleteIdentityResponse[]>())
                            OutputDeleteResult(r);
                    break;
            }
        }

        private async Task<T> ProcessRequest<T>(Command command, JObject data)
        {
            if (data == null) return default;
            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);
            string jsonResult = await PostAsync(command.ToString().CamelCaseToRegular("-").ToLower(), jsonData);

            return JsonConvert.DeserializeObject<T>(jsonResult);
        }
        #endregion Methods

        /// <summary>
        /// <para type="synopsis">Response from AddIdentity calls</para>
        /// <para type="description"></para>
        /// </summary>
        public class AddIdentityResponse
        {
            /// <summary>
            /// <para type="description">Created IPv4 identity</para>
            /// </summary>
            [JsonProperty(PropertyName = "ipv4-address")]
            public string IPv4Address { get; set; }

            /// <summary>
            /// <para type="description">Created IPv6 identity</para>
            /// </summary>
            [JsonProperty(PropertyName = "ipv6-address")]
            public string IPv6Address { get; set; }

            /// <summary>
            /// <para type="description">Textual description of the command’s result</para>
            /// </summary>
            [JsonProperty(PropertyName = "message")]
            public string Message { get; set; }
        }

        /// <summary>
        /// <para type="synopsis">Response from Get-CheckPointIdentity</para>
        /// <para type="description"></para>
        /// </summary>
        public class ShowIdentityResponse
        {
            /// <summary>
            /// <para type="description">Queried IPv4 identity</para>
            /// </summary>
            [JsonProperty(PropertyName = "ipv4-address")]
            public string IPv4Address { get; set; }

            /// <summary>
            /// <para type="description">Queried IPv6 identity</para>
            /// </summary>
            [JsonProperty(PropertyName = "ipv6-address")]
            public string IPv6Address { get; set; }

            /// <summary>
            /// <para type="description">Textual description of the command’s result</para>
            /// </summary>
            [JsonProperty(PropertyName = "message")]
            public string Message { get; set; }

            /// <summary>
            /// <para type="description">All user identities on this IP.</para>
            /// </summary>
            [JsonProperty(PropertyName = "users")]
            public ShowIdentityResponseUser[] Users { get; set; }

            /// <summary>
            /// <para type="description">Computer name, if available</para>
            /// </summary>
            [JsonProperty(PropertyName = "machine")]
            public string Machine { get; set; }

            /// <summary>
            /// <para type="description">List of computer groups</para>
            /// </summary>
            [JsonProperty(PropertyName = "machine-groups")]
            public string[] MachineGroups { get; set; }

            /// <summary>
            /// <para type="description">List of all the access roles on this IP, for auditing and enforcement purposes.</para>
            /// </summary>
            [JsonProperty(PropertyName = "combined-roles")]
            public string[] CombinedRoles { get; set; }

            /// <summary>
            /// <para type="description">Machine session’s identity source, if the machine session is available.</para>
            /// </summary>
            [JsonProperty(PropertyName = "machine-identity-source")]
            public string MachineIdentitySource { get; set; }
        }

        /// <summary>
        /// <para type="synopsis">Response from Get-CheckPointIdentity Users parameter</para>
        /// <para type="description"></para>
        /// </summary>
        public class ShowIdentityResponseUser
        {
            /// <summary>
            /// <para type="description">Users' full names (full name if available, falls back to user name if not)</para>
            /// </summary>
            [JsonProperty(PropertyName = "user")]
            public string User { get; set; }

            /// <summary>
            /// <para type="description">Array of groups</para>
            /// </summary>
            [JsonProperty(PropertyName = "groups")]
            public string[] Groups { get; set; }

            /// <summary>
            /// <para type="description">Array of roles</para>
            /// </summary>
            [JsonProperty(PropertyName = "roles")]
            public string[] Roles { get; set; }

            /// <summary>
            /// <para type="description">Identity source</para>
            /// </summary>
            [JsonProperty(PropertyName = "identity-source")]
            public string IdentitySource { get; set; }
        }
        /// <summary>
        /// <para type="synopsis">Response from Remove-CheckPointIdentity</para>
        /// <para type="description"></para>
        /// </summary>
        public class DeleteIdentityResponse
        {
            /// <summary>
            /// <para type="description">Deleted IPv4 association</para>
            /// </summary>
            [JsonProperty(PropertyName = "ipv4-address")]
            public string IPv4Address { get; set; }

            /// <summary>
            /// <para type="description">Deleted IPv6 association</para>
            /// </summary>
            [JsonProperty(PropertyName = "ipv6-address")]
            public string IPv6Address { get; set; }

            /// <summary>
            /// <para type="description">Textual description of the command’s result</para>
            /// </summary>
            [JsonProperty(PropertyName = "message")]
            public string Message { get; set; }

            /// <summary>
            /// <para type="description">Number of deleted identities</para>
            /// </summary>
            [JsonProperty(PropertyName = "count")]
            public uint Count { get; set; }
        }
    }
}