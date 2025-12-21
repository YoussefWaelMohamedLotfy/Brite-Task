using System.Security.Claims;
using System.Threading.RateLimiting;
using Duende.AccessTokenManagement.OpenIdConnect;
using EM.YARP;
using EM.YARP.Transformers;
using EM.YARP.UserModule;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Yarp.ReverseProxy.Transforms;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-XSRF-TOKEN";
    options.Cookie.SameSite = SameSiteMode.Strict;
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddOpenIdConnectAccessTokenManagement();

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie(
        CookieAuthenticationDefaults.AuthenticationScheme,
        options =>
        {
            options.Cookie.Name = ".employee-management.yarp";
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

            // automatically revoke refresh token at signout time
            options.Events.OnSigningOut = async e => await e.HttpContext.RevokeRefreshTokenAsync();
        }
    )
    //.AddKeycloakOpenIdConnect("keycloak", "tenant-1", OpenIdConnectDefaults.AuthenticationScheme, options =>
    //{
    //    options.ClientId = builder.Configuration.GetValue<string>("OpenIDConnectSettings:ClientId");
    //    options.ClientSecret = builder.Configuration.GetValue<string>("OpenIDConnectSettings:ClientSecret");

    //    options.ResponseType = OpenIdConnectResponseType.Code;
    //    options.ResponseMode = OpenIdConnectResponseMode.Query;

    //    options.GetClaimsFromUserInfoEndpoint = true;
    //    options.SaveTokens = true;
    //    options.MapInboundClaims = false;

    //    options.TokenValidationParameters = new TokenValidationParameters
    //    {
    //        NameClaimType = ClaimTypes.NameIdentifier,
    //        RoleClaimType = ClaimTypes.Role,
    //    };

    //    options.Scope.Clear();
    //    options.Scope.Add(OpenIdConnectScope.OpenIdProfile);
    //    options.Scope.Add(OpenIdConnectScope.OfflineAccess);

    //    options.Events = new()
    //    {
    //        OnRedirectToIdentityProviderForSignOut = (context) =>
    //        {
    //            var logoutUri = $"https://{builder.Configuration.GetValue<string>("OpenIDConnectSettings:Domain")}/oidc/logout?client_id={builder.Configuration.GetValue<string>("OpenIDConnectSettings:ClientId")}";
    //            var redirectUrl = context.HttpContext.BuildRedirectUrl(context.Properties.RedirectUri);
    //            logoutUri += $"&post_logout_redirect_uri={redirectUrl}";

    //            context.Response.Redirect(logoutUri);
    //            context.HandleResponse();
    //            return Task.CompletedTask;
    //        },
    //        OnRedirectToIdentityProvider = (context) =>
    //        {
    //            // Auth0 specific parameter to specify the audience
    //            context.ProtocolMessage.SetParameter("audience", builder.Configuration.GetValue<string>("OpenIDConnectSettings:Audience"));
    //            return Task.CompletedTask;
    //        },
    //    };
    //})
    .AddOpenIdConnect(
        OpenIdConnectDefaults.AuthenticationScheme,
        options =>
        {
            options.Authority = builder.Configuration.GetValue<string>(
                "OpenIDConnectSettings:Domain"
            );
            options.ClientId = builder.Configuration.GetValue<string>(
                "OpenIDConnectSettings:ClientId"
            );
            options.ClientSecret = builder.Configuration.GetValue<string>(
                "OpenIDConnectSettings:ClientSecret"
            );

            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.ResponseType = OpenIdConnectResponseType.Code;
            options.ResponseMode = OpenIdConnectResponseMode.Query;

            options.GetClaimsFromUserInfoEndpoint = true;
            options.SaveTokens = true;
            options.MapInboundClaims = false;
            options.CallbackPath = "/signin-oidc";
            options.RequireHttpsMetadata = builder.Environment.IsProduction();

            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = ClaimTypes.NameIdentifier,
                RoleClaimType = ClaimTypes.Role,
            };

            options.Scope.Clear();
            options.Scope.Add(OpenIdConnectScope.OpenIdProfile);
            options.Scope.Add(OpenIdConnectScope.OfflineAccess);

            options.Events = new()
            {
                OnAuthenticationFailed = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<
                        ILogger<JwtBearerEvents>
                    >();
                    logger.LogError(
                        context.Exception,
                        "Authentication failed: {Message}",
                        context.Exception.Message
                    );
                    return Task.CompletedTask;
                },
                OnRedirectToIdentityProviderForSignOut = (context) =>
                {
                    var logoutUri =
                        $"{builder.Configuration.GetValue<string>("OpenIDConnectSettings:Domain")}/protocol/openid-connect/logout?client_id={builder.Configuration.GetValue<string>("OpenIDConnectSettings:ClientId")}";
                    var redirectUrl =
                        $"{context.HttpContext.BuildRedirectUrl(context.Properties.RedirectUri)}";
                    logoutUri += $"&post_logout_redirect_uri={redirectUrl}";

                    context.Response.Redirect(logoutUri);
                    context.HandleResponse();
                    return Task.CompletedTask;
                },
                OnRedirectToIdentityProvider = (context) =>
                {
                    // Auth0 specific parameter to specify the audience
                    context.ProtocolMessage.SetParameter(
                        "audience",
                        builder.Configuration.GetValue<string>("OpenIDConnectSettings:Audience")
                    );
                    return Task.CompletedTask;
                },
            };
        }
    );

builder
    .Services.AddAuthorizationBuilder()
    .SetDefaultPolicy(
        new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .Build()
    );

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy(
        "user-or-ip",
        httpContext =>
        {
            var partitionKey =
                httpContext.User.Identity?.IsAuthenticated == true
                    ? httpContext.User.FindFirstValue("name") ?? "anonymous"
                    : httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: partitionKey,
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 100,
                    Window = TimeSpan.FromMinutes(1),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0,
                }
            );
        }
    );

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddSingleton<AddBearerTokenToHeadersTransform>();

builder
    .Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(builderContext =>
    {
        if (!string.IsNullOrEmpty(builderContext.Route.AuthorizationPolicy))
        {
            builderContext.RequestTransforms.Add(
                builderContext.Services.GetRequiredService<AddBearerTokenToHeadersTransform>()
            );
        }

        builderContext.RequestTransforms.Add(new RequestHeaderRemoveTransform("Cookie"));
    })
    .AddServiceDiscoveryDestinationResolver();

WebApplication app = builder.Build();

app.UseStatusCodePages();
app.UseExceptionHandler();
app.UseAntiforgery();

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("bff").MapUserEndpoints();

app.MapReverseProxy();
app.MapDefaultEndpoints();
app.MapGet("/", () => "Hello YARP!").AllowAnonymous();

await app.RunAsync();
