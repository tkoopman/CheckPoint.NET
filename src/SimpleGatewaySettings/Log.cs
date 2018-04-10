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
using Newtonsoft.Json;

namespace Koopman.CheckPoint.SimpleGatewaySettings
{
    /// <summary>
    /// Simple Gateway Logs Settings
    /// </summary>
    /// <seealso cref="Koopman.CheckPoint.Common.ChangeTracking" />
    public class Logs : ChangeTracking
    {
        #region Fields

        private bool _alertWhenFreeDiskSpaceBelow;
        private int _alertWhenFreeDiskSpaceBelowThreshold;
        private AlertType _alertWhenFreeDiskSpaceBelowType;
        private bool _beforeDeleteKeepLogsFromTheLastDays;
        private int _beforeDeleteKeepLogsFromTheLastDaysThreshold;
        private bool _beforeDeleteRunScript;
        private string _beforeDeleteRunScriptCommand;
        private bool _deleteIndexFilesOlderThanDays;
        private int _deleteIndexFilesOlderThanDaysThreshold;
        private bool _deleteIndexFilesWhenIndexSizeAbove;
        private int _deleteIndexFilesWhenIndexSizeAboveThreshold;
        private bool _deleteWhenFreeDiskSpaceBelow;
        private int _deleteWhenFreeDiskSpaceBelowThreshold;
        private bool _detectNewCitrixIcaApplicationNames;
        private bool _forwardLogsToLogServer;
        private string _forwardLogsToLogServerName;
        private string _forwardLogsToLogServerScheduleName;
        private Metrics _freeDiskSpaceMetrics;
        private bool _performLogRotateBeforeLogForwarding;
        private bool _rejectConnectionsWhenFreeDiskSpaceBelowThreshold;
        private Metrics _reserveForPacketCaptureMetrics;
        private int _reserveForPacketCaptureThreshold;
        private bool _rotateLogByFileSize;
        private int _rotateLogFileSizeThreshold;
        private bool _rotateLogOnSchedule;
        private string _rotateLogScheduleName;
        private bool _stopLoggingWhenFreeDiskSpaceBelow;
        private int _stopLoggingWhenFreeDiskSpaceBelowThreshold;
        private bool _turnOnQOSLogging;
        private int _updateAccountLogEvery;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether to alert when free disk space below value set in <see cref="AlertWhenFreeDiskSpaceBelowThreshold" />.
        /// </summary>
        /// <value><c>true</c> if [alert when free disk space below]; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "alert-when-free-disk-space-below")]
        public bool AlertWhenFreeDiskSpaceBelow
        {
            get => _alertWhenFreeDiskSpaceBelow;
            set
            {
                _alertWhenFreeDiskSpaceBelow = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the alert when free disk space below threshold metrics.
        /// </summary>
        /// <value>The alert when free disk space below metrics.</value>
        [JsonProperty(PropertyName = "alert-when-free-disk-space-below-metrics")]
        public Metrics AlertWhenFreeDiskSpaceBelowMetrics
        {
            get => _freeDiskSpaceMetrics;
            private set => _freeDiskSpaceMetrics = value;
        }

        /// <summary>
        /// Gets or sets the alert when free disk space below threshold. Use
        /// <see cref="AlertWhenFreeDiskSpaceBelowMetrics" /> to control what metrics this property in.
        /// </summary>
        /// <value>The alert when free disk space below threshold.</value>
        [JsonProperty(PropertyName = "alert-when-free-disk-space-below-threshold")]
        public int AlertWhenFreeDiskSpaceBelowThreshold
        {
            get => _alertWhenFreeDiskSpaceBelowThreshold;
            set
            {
                _alertWhenFreeDiskSpaceBelowThreshold = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the alert type for when free disk space below threshold.
        /// </summary>
        /// <value>The alert type for when free disk space below threshold.</value>
        [JsonProperty(PropertyName = "alert-when-free-disk-space-below-type")]
        public AlertType AlertWhenFreeDiskSpaceBelowType
        {
            get => _alertWhenFreeDiskSpaceBelowType;
            set
            {
                _alertWhenFreeDiskSpaceBelowType = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to keep logs from the last days
        /// <see cref="BeforeDeleteKeepLogsFromTheLastDaysThreshold" /> before deleting.
        /// </summary>
        [JsonProperty(PropertyName = "before-delete-keep-logs-from-the-last-days")]
        public bool BeforeDeleteKeepLogsFromTheLastDays
        {
            get => _beforeDeleteKeepLogsFromTheLastDays;
            set
            {
                _beforeDeleteKeepLogsFromTheLastDays = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the days of logs to keep before deleting.
        /// </summary>
        [JsonProperty(PropertyName = "before-delete-keep-logs-from-the-last-days-threshold")]
        public int BeforeDeleteKeepLogsFromTheLastDaysThreshold
        {
            get => _beforeDeleteKeepLogsFromTheLastDaysThreshold;
            set
            {
                _beforeDeleteKeepLogsFromTheLastDaysThreshold = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to run a script before deleting logs.
        /// </summary>
        /// <value><c>true</c> if to run a script first; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "before-delete-run-script")]
        public bool BeforeDeleteRunScript
        {
            get => _beforeDeleteRunScript;
            set
            {
                _beforeDeleteRunScript = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the script command to run before deleting logs.
        /// </summary>
        /// <value>The script command.</value>
        [JsonProperty(PropertyName = "before-delete-run-script-command")]
        public string BeforeDeleteRunScriptCommand
        {
            get => _beforeDeleteRunScriptCommand;
            set
            {
                _beforeDeleteRunScriptCommand = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to delete index files older than
        /// <see cref="DeleteIndexFilesOlderThanDaysThreshold" /> days.
        /// </summary>
        /// <value>
        /// <c>true</c> if to delete index files older than
        /// <see cref="DeleteIndexFilesOlderThanDaysThreshold" /> days; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty(PropertyName = "delete-index-files-older-than-days")]
        public bool DeleteIndexFilesOlderThanDays
        {
            get => _deleteIndexFilesOlderThanDays;
            set
            {
                _deleteIndexFilesOlderThanDays = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the number of days that index older than should be deleted.
        /// </summary>
        /// <value>The delete index files older than days threshold.</value>
        [JsonProperty(PropertyName = "delete-index-files-older-than-days-threshold")]
        public int DeleteIndexFilesOlderThanDaysThreshold
        {
            get => _deleteIndexFilesOlderThanDaysThreshold;
            set
            {
                _deleteIndexFilesOlderThanDaysThreshold = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to delete index files when index size above <see cref="DeleteIndexFilesWhenIndexSizeAboveThreshold" />.
        /// </summary>
        /// <value>
        /// <c>true</c> if to delete index files when index size above threshold; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty(PropertyName = "delete-index-files-when-index-size-above")]
        public bool DeleteIndexFilesWhenIndexSizeAbove
        {
            get => _deleteIndexFilesWhenIndexSizeAbove;
            set
            {
                _deleteIndexFilesWhenIndexSizeAbove = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the metrics that <see cref="DeleteIndexFilesWhenIndexSizeAboveThreshold" /> is in.
        /// </summary>
        /// <value>The delete index files when index size above metrics.</value>
        [JsonProperty(PropertyName = "delete-index-files-when-index-size-above-metrics")]
        public Metrics DeleteIndexFilesWhenIndexSizeAboveMetrics
        {
            get => _freeDiskSpaceMetrics;
            private set => _freeDiskSpaceMetrics = value;
        }

        /// <summary>
        /// Gets or sets the delete index files when index size above, threshold.
        /// </summary>
        /// <value>The delete index files when index size above, threshold.</value>
        [JsonProperty(PropertyName = "delete-index-files-when-index-size-above-threshold")]
        public int DeleteIndexFilesWhenIndexSizeAboveThreshold
        {
            get => _deleteIndexFilesWhenIndexSizeAboveThreshold;
            set
            {
                _deleteIndexFilesWhenIndexSizeAboveThreshold = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to delete logs when free disk space below <see cref="DeleteWhenFreeDiskSpaceBelowThreshold" />.
        /// </summary>
        /// <value><c>true</c> if to delete when free disk space below threshold; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "delete-when-free-disk-space-below")]
        public bool DeleteWhenFreeDiskSpaceBelow
        {
            get => _deleteWhenFreeDiskSpaceBelow;
            set
            {
                _deleteWhenFreeDiskSpaceBelow = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the metrics <see cref="DeleteWhenFreeDiskSpaceBelowThreshold" /> is in.
        /// </summary>
        /// <value>The delete when free disk space below metrics.</value>
        [JsonProperty(PropertyName = "delete-when-free-disk-space-below-metrics")]
        public Metrics DeleteWhenFreeDiskSpaceBelowMetrics
        {
            get => _freeDiskSpaceMetrics;
            private set => _freeDiskSpaceMetrics = value;
        }

        /// <summary>
        /// Gets or sets the threshold of free disk space when below to delete logs.
        /// </summary>
        /// <value>The delete when free disk space below threshold.</value>
        [JsonProperty(PropertyName = "delete-when-free-disk-space-below-threshold")]
        public int DeleteWhenFreeDiskSpaceBelowThreshold
        {
            get => _deleteWhenFreeDiskSpaceBelowThreshold;
            set
            {
                _deleteWhenFreeDiskSpaceBelowThreshold = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to detect new Citrix ICA application names.
        /// </summary>
        /// <value><c>true</c> if to detect new Citrix ICA application names; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "detect-new-citrix-ica-application-names")]
        public bool DetectNewCitrixIcaApplicationNames
        {
            get => _detectNewCitrixIcaApplicationNames;
            set
            {
                _detectNewCitrixIcaApplicationNames = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to forward local logs to log server.
        /// </summary>
        /// <value><c>true</c> if to forward logs to log server; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "forward-logs-to-log-server")]
        public bool ForwardLogsToLogServer
        {
            get => _forwardLogsToLogServer;
            set
            {
                _forwardLogsToLogServer = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the name of the server to forward logs too.
        /// </summary>
        /// <value>The name of the forward logs to log server.</value>
        [JsonProperty(PropertyName = "forward-logs-to-log-server-name")]
        public string ForwardLogsToLogServerName
        {
            get => _forwardLogsToLogServerName;
            set
            {
                _forwardLogsToLogServerName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the name of the forward logs to log server schedule.
        /// </summary>
        /// <value>The name of the forward logs to log server schedule.</value>
        [JsonProperty(PropertyName = "forward-logs-to-log-server-schedule-name")]
        public string ForwardLogsToLogServerScheduleName
        {
            get => _forwardLogsToLogServerScheduleName;
            set
            {
                _forwardLogsToLogServerScheduleName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the free disk space metrics.
        /// </summary>
        /// <value>The free disk space metrics.</value>
        [JsonProperty(PropertyName = "free-disk-space-metrics")]
        public Metrics FreeDiskSpaceMetrics
        {
            get => _freeDiskSpaceMetrics;
            set
            {
                _freeDiskSpaceMetrics = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to perform a log rotate before log forwarding.
        /// </summary>
        /// <value><c>true</c> if to perform a log rotate before log forwarding; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "perform-log-rotate-before-log-forwarding")]
        public bool PerformLogRotateBeforeLogForwarding
        {
            get => _performLogRotateBeforeLogForwarding;
            set
            {
                _performLogRotateBeforeLogForwarding = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to reject connections when free disk space below threshold.
        /// </summary>
        /// <value>
        /// <c>true</c> if to reject connections when free disk space below threshold; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty(PropertyName = "reject-connections-when-free-disk-space-below-threshold")]
        public bool RejectConnectionsWhenFreeDiskSpaceBelowThreshold
        {
            get => _rejectConnectionsWhenFreeDiskSpaceBelowThreshold;
            set
            {
                _rejectConnectionsWhenFreeDiskSpaceBelowThreshold = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the metrics used by <see cref="ReserveForPacketCaptureThreshold" />.
        /// </summary>
        /// <value>The reserve for packet capture metrics.</value>
        [JsonProperty(PropertyName = "reserve-for-packet-capture-metrics")]
        public Metrics ReserveForPacketCaptureMetrics
        {
            get => _reserveForPacketCaptureMetrics;
            set
            {
                _reserveForPacketCaptureMetrics = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the reserve for packet capture threshold.
        /// </summary>
        /// <value>The reserve for packet capture threshold.</value>
        [JsonProperty(PropertyName = "reserve-for-packet-capture-threshold")]
        public int ReserveForPacketCaptureThreshold
        {
            get => _reserveForPacketCaptureThreshold;
            set
            {
                _reserveForPacketCaptureThreshold = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to rotate logs by file size.
        /// </summary>
        /// <value><c>true</c> if to rotate logs by file size; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "rotate-log-by-file-size")]
        public bool RotateLogByFileSize
        {
            get => _rotateLogByFileSize;
            set
            {
                _rotateLogByFileSize = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the rotate log file size threshold.
        /// </summary>
        /// <value>The rotate log file size threshold.</value>
        [JsonProperty(PropertyName = "rotate-log-file-size-threshold")]
        public int RotateLogFileSizeThreshold
        {
            get => _rotateLogFileSizeThreshold;
            set
            {
                _rotateLogFileSizeThreshold = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to rotate logs on a schedule.
        /// </summary>
        /// <value><c>true</c> if to rotate logs on a schedule; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "rotate-log-on-schedule")]
        public bool RotateLogOnSchedule
        {
            get => _rotateLogOnSchedule;
            set
            {
                _rotateLogOnSchedule = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the name of the schedule to rotate logs by.
        /// </summary>
        /// <value>The name of the rotate log schedule.</value>
        [JsonProperty(PropertyName = "rotate-log-schedule-name")]
        public string RotateLogScheduleName
        {
            get => _rotateLogScheduleName;
            set
            {
                _rotateLogScheduleName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to stop logging when free disk space below <see cref="StopLoggingWhenFreeDiskSpaceBelowThreshold" />.
        /// </summary>
        /// <value>
        /// <c>true</c> if to stop logging when free disk space below threshold; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty(PropertyName = "stop-logging-when-free-disk-space-below")]
        public bool StopLoggingWhenFreeDiskSpaceBelow
        {
            get => _stopLoggingWhenFreeDiskSpaceBelow;
            set
            {
                _stopLoggingWhenFreeDiskSpaceBelow = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the stop logging when free disk space below metrics.
        /// </summary>
        /// <value>The stop logging when free disk space below metrics.</value>
        public Metrics StopLoggingWhenFreeDiskSpaceBelowMetrics => Metrics.MBytes;

        /// <summary>
        /// Gets or sets the stop logging when free disk space below threshold.
        /// </summary>
        /// <value>The stop logging when free disk space below threshold.</value>
        [JsonProperty(PropertyName = "stop-logging-when-free-disk-space-below-threshold")]
        public int StopLoggingWhenFreeDiskSpaceBelowThreshold
        {
            get => _stopLoggingWhenFreeDiskSpaceBelowThreshold;
            set
            {
                _stopLoggingWhenFreeDiskSpaceBelowThreshold = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to turn on QOS logging.
        /// </summary>
        /// <value><c>true</c> to turn on QOS logging; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "turn-on-qos-logging")]
        public bool TurnOnQOSLogging
        {
            get => _turnOnQOSLogging;
            set
            {
                _turnOnQOSLogging = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the frequency that accounting logs should be updated in seconds.
        /// </summary>
        /// <value>The update account log every.</value>
        [JsonProperty(PropertyName = "update-account-log-every")]
        public int UpdateAccountLogEvery
        {
            get => _updateAccountLogEvery;
            set
            {
                _updateAccountLogEvery = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties
    }
}