# Kilometer.io .NET Client

This project is intended to be a very lean .NET wrapper for [kilometer.io](https://kilometer.readme.io/docs/getting-started) API.

[![Build status](https://ci.appveyor.com/api/projects/status/1rgbjsq2mchv0kq9?svg=true)](https://ci.appveyor.com/project/spektrum/kilometer-dotnet-client)

## Get started

Install the package via the NuGet package manager console.

```
Install-Package Kilometer
```

Start by getting a new `KilometerClient` instance and set your app ID using `SetAppId` method.

```csharp
var client = new KilometerClient()
    .SetAppId("YOUR_APP_ID");
```

### Create new user
To create a new user, use the `CreateUser` method. The second parameter, `userProperties`is an object that must contains the properties you want to persist in kilometer.io.

```csharp
await client.CreateUser("USER_ID", new {
    Email = "client@email.com",
    Subscription = "Monthly",
    FullName = "Client's name"
});
```


### Update existing user

To update an existing user, user the `UpdateUser` method. The second parameter, `userProperties`is an object that must contains the properties you want to persist in kilometer.io.

```csharp
await client.UpdateUser("USER_ID", new {
   Email = "newemail@email.com"
});
```

### Cancelling a user

To cancel a user,  user the `CancelUser` method.

```csharp
await client.CancelUser("USER_ID");
```

### Billing a user

To add a new bill to user revenues, use the `BillUser` method. The second parameter is the bill amount.

```chsarp
await client.BillUser("USER_ID", 20m);
```