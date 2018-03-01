# CheckPoint.NET
This is a .NET Library for communicating to a Check Point management server via the Web API.

I am not affiliated with Check Point. I am just a customer who has written this and shared for others to use.

# Example
```C#
var Session = new Session(
    new SessionOptions()
		{
			ManagementServer = "192.168.1.1",
			User = "Admin",
			Password = "Password",
			CertificateValidation = false
		}
    );

var host = Session.FindHost("MyHost");
host.Groups.Add("MyGroup");
host.Color = Colors.Red;
host.AcceptChanges();

var newHost = new Host(Session) 
{
	Name = "NewHost",
	IPv4Address = IPAddress.Parse("10.0.0.138")
};
newHost.AcceptChanges(Ignore.Warnings);
```
