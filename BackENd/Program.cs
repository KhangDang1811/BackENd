using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Twitter;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddCors(p => p.AddPolicy("corspolicy", build =>
{
    build.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader();
}));
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = TwitterDefaults.AuthenticationScheme; // Sử dụng Twitter để xác thực
})
.AddCookie(options =>
{
    options.LoginPath = "/signin-twitter"; // Đường dẫn đến endpoint xác thực Twitter
    options.AccessDeniedPath = "/account/accessdenied"; // Đường dẫn khi truy cập bị từ chối
})
.AddTwitter("Twitter", options =>
{
    options.ConsumerKey = "L3DHX8ePw3KmmFgJDVHC6SvOi";
    options.ConsumerSecret = "hYamPM564IxdgZbNfSswmtHFSBdqarzgkfffsLlqxDtm5hmB5Z";
    options.RetrieveUserDetails = true;
    options.SaveTokens = true;

});

var app = builder.Build();


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console() // Ghi log ra Console
    .CreateLogger();

builder.Services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.AddDebug();
});
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// ...
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseCors("corspolicy");
app.UseAuthentication(); // Thêm middleware xác thực
app.UseAuthorization(); // Thêm middleware quyền hạn
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});
// ...


app.Run();
