﻿// MIT License
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
using System;

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

        [JsonProperty(PropertyName = "alert-when-free-disk-space-below-metrics")]
        public Metrics AlertWhenFreeDiskSpaceBelowMetrics
        {
            get => _freeDiskSpaceMetrics;
            private set => _freeDiskSpaceMetrics = value;
        }

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

        [JsonProperty(PropertyName = "delete-index-files-when-index-size-above-metrics")]
        public Metrics DeleteIndexFilesWhenIndexSizeAboveMetrics
        {
            get => _freeDiskSpaceMetrics;
            private set => _freeDiskSpaceMetrics = value;
        }

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

        [JsonProperty(PropertyName = "delete-when-free-disk-space-below-metrics")]
        public Metrics DeleteWhenFreeDiskSpaceBelowMetrics
        {
            get => _freeDiskSpaceMetrics;
            private set => _freeDiskSpaceMetrics = value;
        }

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

        public Metrics StopLoggingWhenFreeDiskSpaceBelowMetrics
        {
            get => Metrics.MBytes;
        }

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

        #region Methods

        public override void AcceptChanges()
        {
            throw new NotImplementedException("Use AcceptChanges from Parent Object.");
        }

        #endregion Methods
    }
}