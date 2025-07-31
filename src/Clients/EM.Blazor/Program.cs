using EM.Blazor.Components;
using EM.SDK;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
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
})
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, o =>
{
    o.Authority = "http://localhost:8081/realms/tenant-1/";
    o.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

    o.ClientId = "blazor-1";
    o.ClientSecret = "5yJHp9EjICabeJ1jshyVaDfll4B4qryt";
    o.ResponseType = OpenIdConnectResponseType.Code;

    o.SaveTokens = true;
    o.GetClaimsFromUserInfoEndpoint = true;
    o.MapInboundClaims = false;
    o.RequireHttpsMetadata = builder.Environment.IsProduction();
    o.TokenValidationParameters.ValidateIssuer = builder.Environment.IsProduction();
    o.TokenValidationParameters.ValidateAudience = builder.Environment.IsProduction();

    o.Scope.Add("openid");
    o.Scope.Add("profile");
});

builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
builder.Services.AddCascadingAuthenticationState();

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

await app.RunAsync();
