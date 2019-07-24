// MIT License
//
// Copyright (c) 2018 Tim Koopman
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
// associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT
// OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Koopman.CheckPoint.Common;
using Koopman.CheckPoint.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Koopman.CheckPoint.IA
{
    /// <summary>
    /// Used to manage identities on a IA Gateway.
    /// </summary>
    /// <seealso cref="Koopman.CheckPoint.Common.HttpSession" />
    public class IASession : HttpSession
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IASession" /> class.
        /// </summary>
        /// <param name="gateway">The IA gateway.</param>
        /// <param name="sharedSecret">The shared secret.</param>
        /// <param name="certificateHash">The certificate hash.</param>
        /// <param name="port">The port.</param>
        /// <param name="certificateValidation">The certificate validation.</param>
        /// <param name="debugWriter">The debug writer.</param>
        /// <param name="indentJson">
        /// if set to <c>true</c> the Json data sent to the server is indented.
        /// </param>
        /// <param name="maxConnections">The maximum HTTPS connections to gateway.</param>
        /// <param name="httpTimeout">The HTTP timeout. Default 100 seconds</param>
        public IASession(
            string gateway, string sharedSecret, string certificateHash = null, int port = 443,
            CertificateValidation certificateValidation = CertificateValidation.Auto,
            TextWriter debugWriter = null, bool indentJson = false, int maxConnections = 3, TimeSpan? httpTimeout = null) :
            base($"https://{gateway}:{port}/_IA_API/v1.0/", certificateValidation, certificateHash, debugWriter, indentJson, maxConnections, httpTimeout)
        {
            SharedSecret = sharedSecret;
        }

        #endregion Constructors

        #region Fields

        private static readonly object Lock = new object();
        private readonly string SharedSecret;
        private JObject CurrentBatch;
        private JArray CurrentRequests;
        private Action<AddIdentityResponse> OutputAddResult;
        private Action<DeleteIdentityResponse> OutputDeleteResult;
        private Action<ShowIdentityResponse> OutputShowResult;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the current batch command.
        /// </summary>
        /// <value>The batch command.</value>
        public Command BatchCommand { get; private set; }

        /// <summary>
        /// Gets the maximum size of the batch before automatically processing and starting a new batch.
        /// </summary>
        /// <value>The maximum size of the batch.</value>
        public int MaxBatchSize { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Adds an identity.
        /// </summary>
        /// <param name="ipAddress">The ip address.</param>
        /// <param name="user">The user.</param>
        /// <param name="machine">The machine.</param>
        /// <param name="domain">The domain.</param>
        /// <param name="sessionTimeout">The session timeout.</param>
        /// <param name="fetchUserGroups">Weather to automatically fetch user groups.</param>
        /// <param name="fetchMachineGroups">Weather to automatically fetch machine groups.</param>
        /// <param name="calculateRoles">Weather to automatically calculate roles.</param>
        /// <param name="userGroups">The user groups.</param>
        /// <param name="machineGroups">The machine groups.</param>
        /// <param name="roles">The roles.</param>
        /// <param name="machineOS">The machine os.</param>
        /// <param name="hostType">Type of the host.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains will contain
        /// <c>null</c> if you have started a batch processing with
        /// <see cref="StartAddBatch(Action{AddIdentityResponse}, int)" />, otherwise it will contain
        /// the response from the gateway.
        /// </returns>
        public async Task<AddIdentityResponse> AddIdentity(
            string ipAddress, string user = null, string machine = null, string domain = null,
            int sessionTimeout = 43200, bool? fetchUserGroups = null, bool? fetchMachineGroups = null,
            bool? calculateRoles = null, string[] userGroups = null, string[] machineGroups = null,
            string[] roles = null, string machineOS = null, string hostType = null,
            CancellationToken cancellationToken = default)
        {
            var req = new JObject()
            {
                { "ip-address", ipAddress }
            };
            req.AddIfNotNull("user", user);
            req.AddIfNotNull("machine", machine);
            req.AddIfNotNull("domain", domain);
            req.AddIfNotNull("session-timeout", sessionTimeout);
            req.AddIfNotNull01("fetch-user-groups", fetchUserGroups);
            req.AddIfNotNull01("fetch-machine-groups", fetchMachineGroups);
            req.AddIfNotNull("user-groups", userGroups);
            req.AddIfNotNull("machine-groups", machineGroups);
            req.AddIfNotNull01("calculate-roles", calculateRoles);
            req.AddIfNotNull("roles", roles);
            req.AddIfNotNull("machine-os", machineOS);
            req.AddIfNotNull("host-type", hostType);

            if (BatchCommand != Command.AddIdentity)
            {
                req.Add("shared-secret", SharedSecret);
                var result = await ProcessRequest<AddIdentityResponse>(Command.AddIdentity, req, cancellationToken);
                return result;
            }
            else
            {
                await AddRequestToBatch(req, cancellationToken);
                return null;
            }
        }

        /// <summary>
        /// Deletes an identity.
        /// </summary>
        /// <param name="ipAddress">The ip address.</param>
        /// <param name="clientType">
        /// Deletes only associations created by the specified identity source.
        /// </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains will contain
        /// <c>null</c> if you have started a batch processing with
        /// <see cref="StartDeleteBatch(Action{DeleteIdentityResponse}, int)" />, otherwise it will
        /// contain the response from the gateway.
        /// </returns>
        public Task<DeleteIdentityResponse> DeleteIdentity(string ipAddress, ClientTypes clientType = ClientTypes.Any, CancellationToken cancellationToken = default)
        {
            var req = new JObject()
            {
                { "client-type",  clientType.ToString().CamelCaseToRegular("-").ToLower() },
                { "ip-address", ipAddress }
            };
            return DeleteIdentity(req, cancellationToken);
        }

        /// <summary>
        /// Deletes an identity.
        /// </summary>
        /// <param name="subnet">The subnet.</param>
        /// <param name="subnetMask">The subnet mask.</param>
        /// <param name="clientType">
        /// Deletes only associations created by the specified identity source.
        /// </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains will contain
        /// <c>null</c> if you have started a batch processing with
        /// <see cref="StartDeleteBatch(Action{DeleteIdentityResponse}, int)" />, otherwise it will
        /// contain the response from the gateway.
        /// </returns>
        public Task<DeleteIdentityResponse> DeleteIdentityMask(string subnet, string subnetMask, ClientTypes clientType = ClientTypes.Any, CancellationToken cancellationToken = default)
        {
            var req = new JObject()
            {
                { "revoke-method", "mask" },
                { "client-type",  clientType.ToString().CamelCaseToRegular("-").ToLower()},
                { "subnet", subnet },
                { "subnet-mask", subnetMask }
            };
            return DeleteIdentity(req, cancellationToken);
        }

        /// <summary>
        /// Deletes an identity.
        /// </summary>
        /// <param name="firstIP">The first ip.</param>
        /// <param name="lastIP">The last ip.</param>
        /// <param name="clientType">
        /// Deletes only associations created by the specified identity source.
        /// </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains will contain
        /// <c>null</c> if you have started a batch processing with
        /// <see cref="StartDeleteBatch(Action{DeleteIdentityResponse}, int)" />, otherwise it will
        /// contain the response from the gateway.
        /// </returns>
        public Task<DeleteIdentityResponse> DeleteIdentityRange(string firstIP, string lastIP, ClientTypes clientType = ClientTypes.Any, CancellationToken cancellationToken = default)
        {
            var req = new JObject()
            {
                { "revoke-method", "range" },
                { "client-type",  clientType.ToString().CamelCaseToRegular("-").ToLower()},
                { "ip-address-first", firstIP },
                { "ip-address-last", lastIP }
            };
            return DeleteIdentity(req, cancellationToken);
        }

        /// <summary>
        /// Forces current batch to be sent to gateway, and optionally also finish batch processing.
        /// </summary>
        /// <param name="stopBatch">
        /// if set to <c>true</c> batch processing also disabled after flush.
        /// </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task Flush(bool stopBatch = false, CancellationToken cancellationToken = default)
        {
            var command = BatchCommand;
            var batch = GetCurrentBatchAndClear(stopBatch);

            var result = await ProcessRequest<JObject>(command, batch, cancellationToken);
            ProcessBatchResponse(command, result);
        }

        /// <summary>
        /// Resets the batch without sending any requests to the gateway, and optionally also finish
        /// batch processing.
        /// </summary>
        /// <param name="stopBatch">
        /// if set to <c>true</c> batch processing also disabled after reset.
        /// </param>
        public void ResetBatch(bool stopBatch = false) => GetCurrentBatchAndClear(stopBatch);

        /// <summary>
        /// Shows an identity.
        /// </summary>
        /// <param name="ipAddress">The ip address.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains will contain
        /// <c>null</c> if you have started a batch processing with
        /// <see cref="StartShowBatch(Action{ShowIdentityResponse}, int)" />, otherwise it will
        /// contain the response from the gateway.
        /// </returns>
        public async Task<ShowIdentityResponse> ShowIdentity(string ipAddress, CancellationToken cancellationToken = default)
        {
            var req = new JObject();
            req.AddIfNotNull("ip-address", ipAddress);
            if (BatchCommand != Command.ShowIdentity)
            {
                req.Add("shared-secret", SharedSecret);
                return await ProcessRequest<ShowIdentityResponse>(Command.ShowIdentity, req, cancellationToken);
            }
            else
            {
                await AddRequestToBatch(req, cancellationToken);
                return null;
            }
        }

        /// <summary>
        /// Starts batch processing for <see cref="AddIdentity(string, string, string, string, int, bool?, bool?, bool?, string[], string[], string[], string, string, CancellationToken)" />.
        /// </summary>
        /// <param name="outputAction">
        /// The output action is call for each individual response when the batch is processed.
        /// </param>
        /// <param name="maxBatchSize">
        /// Maximum size of the batch. When this number of requests is reached the batch will
        /// automatically be sent to the gateway and a new batch started.
        /// </param>
        /// <exception cref="Exception">Batch already in progress.</exception>
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

        /// <summary>
        /// Starts batch processing for
        /// <see cref="DeleteIdentity(string, ClientTypes, CancellationToken)" />,
        /// <see cref="DeleteIdentityMask(string, string, ClientTypes, CancellationToken)" /> and <see cref="DeleteIdentityRange(string, string, ClientTypes, CancellationToken)" />.
        /// </summary>
        /// <param name="outputAction">
        /// The output action is call for each individual response when the batch is processed.
        /// </param>
        /// <param name="maxBatchSize">
        /// Maximum size of the batch. When this number of requests is reached the batch will
        /// automatically be sent to the gateway and a new batch started.
        /// </param>
        /// <exception cref="Exception">Batch already in progress.</exception>
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

        /// <summary>
        /// Starts batch processing for <see cref="ShowIdentity(string, CancellationToken)" />.
        /// </summary>
        /// <param name="outputAction">
        /// The output action is call for each individual response when the batch is processed.
        /// </param>
        /// <param name="maxBatchSize">
        /// Maximum size of the batch. When this number of requests is reached the batch will
        /// automatically be sent to the gateway and a new batch started.
        /// </param>
        /// <exception cref="Exception">Batch already in progress.</exception>
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

        /// <summary>
        /// Adds the request to batch. Is responsible for also processing a batch when
        /// <see cref="MaxBatchSize" /> is reached and starting a new batch.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task AddRequestToBatch(JObject request, CancellationToken cancellationToken)
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

            var result = await ProcessRequest<JObject>(command, processBatch, cancellationToken);
            ProcessBatchResponse(command, result);
        }

        /// <summary>
        /// As there are a number of different overloads for DeleteIdentity this is the shared code
        /// they all run.
        /// </summary>
        /// <param name="req">The DeleteIdentity request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        private async Task<DeleteIdentityResponse> DeleteIdentity(JObject req, CancellationToken cancellationToken)
        {
            if (BatchCommand != Command.DeleteIdentity)
            {
                req.Add("shared-secret", SharedSecret);
                return await ProcessRequest<DeleteIdentityResponse>(Command.DeleteIdentity, req, cancellationToken);
            }
            else
            {
                await AddRequestToBatch(req, cancellationToken);
                return null;
            }
        }

        /// <summary>
        /// Gets the current batch and prepares it for a new batch to start.
        /// </summary>
        /// <param name="stopBatch">if set to <c>true</c> batch processing is disabled after clearing.</param>
        /// <returns>The current batch.</returns>
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

        /// <summary>
        /// Processes the batch response.
        /// </summary>
        /// <param name="command">The command called.</param>
        /// <param name="result">The result.</param>
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

        private async Task<T> ProcessRequest<T>(Command command, JObject data, CancellationToken cancellationToken)
        {
            if (data == null) return default;
            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);
            string jsonResult = await PostAsync(command.ToString().CamelCaseToRegular("-").ToLower(), jsonData, cancellationToken);

            return JsonConvert.DeserializeObject<T>(jsonResult);
        }

        #endregion Methods
    }
}