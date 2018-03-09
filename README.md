[![Build status](https://ci.appveyor.com/api/projects/status/38sg7mkx3gi6mahl/branch/master?svg=true)](https://ci.appveyor.com/project/tkoopman/checkpoint-net/branch/master)

# CheckPoint.NET
This is a .NET Library for communicating to a Check Point management server via the Web API.

You can find documentation at https://tkoopman.github.io/CheckPoint.NET/

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
