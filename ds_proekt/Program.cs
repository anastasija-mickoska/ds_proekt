using ds_proekt.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Builder.Extensions;

var pathToKey = Path.Combine("secrets", "ds-proekt-baa0c-firebase-adminsdk-fbsvc-f0350e675c.json");
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", pathToKey);


if (FirebaseApp.DefaultInstance == null)
{
    FirebaseApp.Create(new AppOptions
    {
        Credential = GoogleCredential.GetApplicationDefault()
    });
}
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<FirebaseAuthService>();
builder.Services.AddSingleton<FirebaseParfumeService>();
builder.Services.AddControllersWithViews();
//builder.Services.AddScoped<FirebaseAuthService>();
builder.Services.AddDistributedMemoryCache(); // Required for session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // optional
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
