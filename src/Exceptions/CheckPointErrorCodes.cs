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

namespace Koopman.CheckPoint.Exceptions
{
    /// <summary>
    /// All error codes Check Point can return on failed posts
    /// </summary>
    internal enum CheckPointErrorCodes
    {
        generic_error,
        generic_err_invalid_syntax,
        generic_err_invalid_parameter_name,
        not_implemented,
        generic_internal_error,
        generic_server_error,
        generic_server_initializing,
        generic_err_command_not_found,
        generic_err_command_version_not_found,
        generic_err_invalid_api_type,
        generic_err_invalid_api_object_feature,
        generic_err_missing_required_parameters,
        generic_err_missing_required_header,
        generic_err_invalid_header,
        generic_err_invalid_parameter,
        generic_err_normalize,
        err_bad_url,
        err_unknown_api_version,
        err_login_failed_wrong_username_or_password,
        err_login_failed_more_than_one_opened_session,
        err_login_failed,
        err_normalization_failed,
        err_validation_failed,
        err_publish_failed,
        generic_err_missing_session_id,
        generic_err_wrong_session_id,
        generic_err_session_expired,
        generic_err_session_in_use,
        err_switch_session_failed,
        generic_err_no_permissions,
        err_forbidden,
        err_not_a_system_domain_session,
        err_inappropriate_domain_type,
        generic_err_object_not_found,
        generic_err_object_field_not_unique,
        generic_err_object_type_wrong,
        generic_err_object_locked,
        generic_err_object_deletion,
        err_policy_installation_failed,
        err_rulebase_invalid_operation
    }
}