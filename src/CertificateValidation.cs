using System;

namespace Koopman.CheckPoint
{
    /// <summary>
    /// What server certificatate validation should be performed.
    /// </summary>
    [Flags]
    public enum CertificateValidation
    {
        /// <summary>
        /// Trust any certificate. NOTE: Highly recommended not to use this for security reasons.
        /// </summary>
        None = 0,

        /// <summary>
        /// Validates the server's certificate and chain.
        /// </summary>
        ValidCertificate = 1,

        /// <summary>
        /// Validates server's certificate hash matches the provided hash.
        /// </summary>
        CertificatePinning = 2,

        /// <summary>
        /// Server's certificate must be valid and also match the provided hash.
        /// </summary>
        All = 3,

        /// <summary>
        /// If certificate hash provided then it will be checked, else any valid certificate will be allowed.
        /// </summary>
        Auto = 4
    }
}