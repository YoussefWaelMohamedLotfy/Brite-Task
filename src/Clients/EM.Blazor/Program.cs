using EM.Blazor;
using EM.Blazor.Components;
using EM.SDK;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

#if DEBUG
IdentityModelEventSource.ShowPII = true;
#endif

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddAuthorizationBuilder();

builder.Services.AddAuthentication(o =>
{
    o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
{
    o.Cookie.Name = ".employee-management";

    // automatically revoke refresh token at signout time
    o.Events.OnSigningOut = async e => await e.HttpContext.RevokeRefreshTokenAsync();
})
.AddKeycloakOpenIdConnect("keycloak", "tenant-1", OpenIdConnectDefaults.AuthenticationScheme, o =>
{
    o.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

    o.ClientId = "blazor-1";
    o.ClientSecret = "5yJHp9EjICabeJ1jshyVaDfll4B4qryt";
    o.ResponseType = OpenIdConnectResponseType.Code;

    o.SaveTokens = true;
    o.GetClaimsFromUserInfoEndpoint = true;
    o.MapInboundClaims = false;
    o.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;

    o.RequireHttpsMetadata = builder.Environment.IsProduction();
    o.TokenValidationParameters.ValidateIssuer = builder.Environment.IsProduction();
    o.TokenValidationParameters.ValidateAudience = builder.Environment.IsProduction();

    o.Scope.Add(OpenIdConnectScope.OpenIdProfile);
    o.Scope.Add(OpenIdConnectScope.OfflineAccess);
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddOpenIdConnectAccessTokenManagement();

builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
builder.Services.AddCascadingAuthenticationState();

// ConfigureCookieOidc attaches a cookie OnValidatePrincipal callback to get
// a new access token when the current one expires, and reissue a cookie with the
// new access token saved inside. If the refresh fails, the user will be signed
// out. OIDC connect options are set for saving tokens and the offline access
// scope.
builder.Services.ConfigureCookieOidc(CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);

builder.Services.AddEmployeeManagement();

builder.Services.AddBootstrapBlazor();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

WebApplication app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapGroup("/authentication").MapLoginAndLogout();

await app.RunAsync();
