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

using Newtonsoft.Json;
using System.Net;

namespace Koopman.CheckPoint.Exceptions
{
    public class CheckPointError
    {
        #region Constructors

        [JsonConstructor]
        private CheckPointError(CheckPointErrorCodes code, string message, CheckPointErrorDetails[] warnings, CheckPointErrorDetails[] errors, CheckPointErrorDetails[] blockingErrors)
        {
            Code = code;
            Message = message;
            Warnings = warnings;
            Errors = errors;
            BlockingErrors = blockingErrors;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// <para type="description">Validation blocking-errors.</para>
        /// </summary>
        [JsonProperty(PropertyName = "blocking-errors")]
        public CheckPointErrorDetails[] BlockingErrors { get; private set; }

        /// <summary>
        /// <para type="description">Validation errors.</para>
        /// </summary>
        [JsonProperty(PropertyName = "errors")]
        public CheckPointErrorDetails[] Errors { get; private set; }

        /// <summary>
        /// <para type="description">Error message.</para>
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; private set; }

        /// <summary>
        /// <para type="description">Validation warnings.</para>
        /// </summary>
        [JsonProperty(PropertyName = "warnings")]
        public CheckPointErrorDetails[] Warnings { get; private set; }

        /// <summary>
        /// <para type="description">Error code.</para>
        /// </summary>
        [JsonProperty(PropertyName = "code")]
        internal CheckPointErrorCodes Code { get; private set; }

        #endregion Properties

        #region Methods

        internal static GenericException CreateException(string jsonResponse, HttpStatusCode httpStatusCode)
        {
            try
            {
                CheckPointError e = JsonConvert.DeserializeObject<CheckPointError>(jsonResponse);

                switch (e.Code)
                {
                    case CheckPointErrorCodes.err_bad_url:
                        return new BadURLException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.err_forbidden:
                        return new ForbiddenException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.err_inappropriate_domain_type:
                        return new InappropriateDomainTypeException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.err_login_failed:
                        return new LoginFailedException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.err_login_failed_more_than_one_opened_session:
                        return new LoginFailedMoreThanOneOpenedSessionException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.err_login_failed_wrong_username_or_password:
                        return new LoginFailedWrongUsernameOrPasswordException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.err_normalization_failed:
                        return new NormalizationFailedException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.err_not_a_system_domain_session:
                        return new NotASystemDomainSessionException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.err_policy_installation_failed:
                        return new PolicyInstallationFailedException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.err_publish_failed:
                        return new PublishFailedException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.err_rulebase_invalid_operation:
                        return new RulebaseInvalidOperationException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.err_switch_session_failed:
                        return new SwitchSessionFailedException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.err_unknown_api_version:
                        return new UnknownAPIVersionException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.err_validation_failed:
                        return new ValidationFailedException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.generic_error:
                        return new GenericException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.generic_err_command_not_found:
                        return new CommandNotFoundException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.generic_err_command_version_not_found:
                        return new CommandVersionNotFoundException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.generic_err_invalid_api_object_feature:
                        return new InvalidAPIObjectFeatureException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.generic_err_invalid_api_type:
                        return new InvalidAPITypeException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.generic_err_invalid_header:
                        return new InvalidHeaderException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.generic_err_invalid_parameter:
                        return new InvalidParameterException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.generic_err_invalid_parameter_name:
                        return new InvalidParameterNameException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.generic_err_invalid_syntax:
                        return new InvalidSyntaxException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.generic_err_missing_required_header:
                        return new MissingRequiredHeaderException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.generic_err_missing_required_parameters:
                        return new MissingRequiredParametersException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.generic_err_missing_session_id:
                        return new MissingSessionIDException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.generic_err_normalize:
                        return new NormalizeException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.generic_err_no_permissions:
                        return new NoPermissionsException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.generic_err_object_deletion:
                        return new ObjectDeletionException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.generic_err_object_field_not_unique:
                        return new ObjectFieldNotUniqueException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.generic_err_object_locked:
                        return new ObjectLockedException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.generic_err_object_not_found:
                        return new ObjectNotFoundException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.generic_err_object_type_wrong:
                        return new ObjectTypeWrongException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.generic_err_session_expired:
                        return new SessionExpiredException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.generic_err_session_in_use:
                        return new SessionInUseException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.generic_err_wrong_session_id:
                        return new WrongSessionIDException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.generic_internal_error:
                        return new InternalErrorException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.generic_server_error:
                        return new ServerErrorException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.generic_server_initializing:
                        return new ServerInitializingException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    case CheckPointErrorCodes.not_implemented:
                        return new NotImplementedException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);

                    default:
                        // Should Never Hit This
                        return new GenericException(e.Message, httpStatusCode, e.Code, e.Warnings, e.Errors, e.BlockingErrors);
                }
            }
            catch
            {
                return new GenericException($"Server Error: {httpStatusCode}", httpStatusCode, CheckPointErrorCodes.generic_server_error, null, null, null);
            }
        }

        #endregion Methods
    }
}