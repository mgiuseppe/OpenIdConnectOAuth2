# Securing ASP.NET Core 2 with OAuth 2 and OpenID Connect
Fully functioning finished sample code for my Securing ASP.NET Core 2 with OAuth2 and OpenID Connect course.  Make sure you start up / deploy the IDP, Client & API project when running the finished solution.

The course can be found at https://app.pluralsight.com/library/courses/securing-aspdotnet-core2-oauth2-openid-connect/table-of-contents

Happy coding! :)

# Content

- IDP and CLIENT
  - Create an Identity provider using identity server 4
  - Add UI to IDP
  - Add a Client to IDP
  - Configuring the client (authentication middleware)
  - Login and Logout in the client
  - Set the client authentication middleware to reach the UserInfo Endpoint and get the claims you need
  - Claim transformation
  - Cookies and accessing the claims
  - Manually calling the Userinfo Endpoint
  - RBAC using claims
    - set the RoleClaimType
    - using the role in views and controllers
- IDP and API
  - Add authorization to API
    - api scope
    - configure the middleware to use access token
  - Pass access token as a bearer
  - Using the claims (sub and others) to filter the API results
  - Adding claims to the access token
  - RBAC using claims in the API
  - ABAC using claims in the API (Policy Based)
    - custom requirements and handlers
    - using policies in views and controllers
 - Handler token lifetime
   - Modify tokens' lifetime in the IDP
   - Refresh token
   - Reference token and revocation endpoint (see the branch "reference tokens")
   
