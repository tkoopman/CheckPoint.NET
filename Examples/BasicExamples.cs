using Koopman.CheckPoint;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Examples
{
    [TestClass]
    public class BasicExamples : ExampleBase
    {
        #region Methods

        [TestMethod]
        public void BasicConnection()
        {
            #region Example: Basic Connection

            // Login to Check Point Management Web Service
            var session = new Session(
                    new SessionOptions()
                    {
                        ManagementServer = ManagementServer,
                        User = Username,
                        Password = Password,
                        ReadOnly = true,
                        CertificateValidation = false
                    }
                );

            // Get first 5 hosts
            var hosts = session.FindAllHosts(limit: 5);

            foreach (var host in hosts)
            {
                Console.WriteLine(host.Name);
            }

            // Disconnect from server
            session.Logout();

            #endregion Example: Basic Connection
        }

        #endregion Methods
    }
}