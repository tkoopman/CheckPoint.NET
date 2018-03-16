using Koopman.CheckPoint;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;

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

        [TestMethod]
        public void NewHost()
        {
            // Login to Check Point Management Web Service
            var session = new Session(
                    new SessionOptions()
                    {
                        ManagementServer = ManagementServer,
                        User = Username,
                        Password = Password,
                        CertificateValidation = false
                    }
                );

            // Create group used for example
            new Group(session)
            {
                Name = "Web Servers"
            }.AcceptChanges();

            #region Example: New Host

            // Create new host and set standard properties
            var host = new Host(session)
            {
                Name = "Web Server",
                IPv4Address = IPAddress.Parse("10.1.1.1"),
                Color = Colors.LightGreen,
                Comments = "Internal Web Server"
            };
            // Add host to an existing group
            host.Groups.Add("Web Servers");

            // Send request to create new host to server and check for any errors
            try
            {
                host.AcceptChanges();

                Console.WriteLine($"Name: {host.Name}");
                Console.WriteLine($"IP: {host.IPv4Address}");
                Console.WriteLine($"UID: {host.UID}");

                /// Uncomment the below line to publish change
                // session.Publish();
            }
            catch (Koopman.CheckPoint.Exceptions.ValidationFailedException e)
            {
                // Catch Check Point validation exception. In this example could be thrown if name
                // already in use, or IP address already used (Call
                // host.AcceptChanges(Ignore.Warnings) to allow objects with the same IP).
                Console.WriteLine($"ValidationFailedException: {e}");

                // By default the ToString method on all GenericException classes will include all
                // messages in the BlockingErrors, Errors and Warnings properties after the main Message
            }
            catch (Koopman.CheckPoint.Exceptions.ObjectNotFoundException e)
            {
                // Catch Check Point object not found exception. In this example should only be
                // thrown if the group Web Servers doesn't already exist
                Console.WriteLine($"ObjectNotFoundException: {e}");
            }
            catch (Koopman.CheckPoint.Exceptions.GenericException e)
            {
                // Catch any other Check Point exception that has not already been caught
                Console.WriteLine($"GenericException ({e.GetType().Name}): {e}");
            }

            #endregion Example: New Host

            session.Discard();
            session.Logout();
        }

        #endregion Methods
    }
}