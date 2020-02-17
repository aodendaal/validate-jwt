# Validate a JWT using .NET

Code created from [Manually validating a JWT using .NET](https://www.jerriepelser.com/blog/manually-validating-rs256-jwt-dotnet/) by Jerrie Pelser

The input for the `Validate()` function are the _clientId_ and _tenantId_ as explained by MSAL which are re-labelled _auth0Audience_ and _auth0domain_ respectively.

The _rawIdToken_ is from the _idToken_ of the `accountIdentifier` object returned by MSAL for JavaScript.