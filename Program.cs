using AdminPortalV8.Data;
using AdminPortalV8.Data.ExtendedIdentity;
using AdminPortalV8.Helpers;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Interfaces;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Services;
using AdminPortalV8.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Helpers;
using AdminPortalV8.Models.Restaurant;
using AdminPortalV8.Models.Epatrol;
using EPatrol.Services;
using Quartz;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.SignalR; // Add this for SignalR
using AdminPortalV8.Hubs; // Add this for your SignalR Hub namespace

var builder = WebApplication.CreateBuilder(args);
var SecretKey = "mysupersecret_secretkey!123";
var Issuer = "infologs.in";
var Audience = "global";

// Connection strings
var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var epatrolConnectionString = builder.Configuration.GetConnectionString("EPatrol_DevConnection")
    ?? throw new InvalidOperationException("Connection string 'EPatrol_DevConnection' not found.");

// Register ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(defaultConnectionString));

// Register EPatrol_DevContext
builder.Services.AddDbContext<EPatrol_DevContext>(options =>
    options.UseSqlServer(epatrolConnectionString));

// Add SignalR services
builder.Services.AddSignalR();

builder.Services.AddMvc(options =>
{
    options.ModelBinderProviders.Insert(0, new DateTimeModelBinderProvider());
}).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = Issuer,
                    ValidateAudience = true,
                    ValidAudience = Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey)),
                };
                options.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        context.Response.ContentType = context.Request.Headers["Accept"].ToString();
                        string _Message = "Authentication token is invalid.";
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            _Message = "Token has expired.";
                            return context.Response.WriteAsync(JsonConvert.SerializeObject(new
                            {
                                StatusCode = (int)HttpStatusCode.Unauthorized,
                                Message = _Message
                            }));
                        }
                        return context.Response.WriteAsync(JsonConvert.SerializeObject(new
                        {
                            StatusCode = (int)HttpStatusCode.Unauthorized,
                            Message = _Message
                        }));
                    }
                };
            });

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
})
    .AddRoles<ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

//builder.Services.AddControllers()
// .AddNewtonsoftJson(options =>
// {
//     options.SerializerSettings.ContractResolver = new DefaultContractResolver();
// });

//builder.Services.AddControllersWithViews().
//        AddJsonOptions(options =>
//        {
//            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
//            options.JsonSerializerOptions.PropertyNamingPolicy = null;
//        });

// Add session services
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

builder.Services.AddScoped<IUser, UserService>();
builder.Services.AddScoped<IAuth, AuthService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<SignInManager<ApplicationUser>>();
builder.Services.AddSingleton<IClaimsTransformation, ClaimsTransformer>();
builder.Services.AddSingleton<UserObj>();
builder.Services.AddScoped<IUserService, UserServices>();
builder.Services.AddScoped<IGeneral, GeneralService>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IRestaurant, RestaurantServices>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAnalytic, AnalyticServices>();
builder.Services.AddSingleton<RestaurantObj>();
builder.Services.AddSingleton<AnalyticObj>();
builder.Services.AddSingleton<IFfmpegProcessService, FfmpegProcessService>();
builder.Services.AddHttpClient<ITelegramService, TelegramService>();
builder.Services.AddHttpClient<IMessageService, WhatsAppService>();
builder.Services.AddHttpClient<ISMSService, SMSService>();
builder.Services.AddHttpClient<IAutoPtrolApiCalling, AutoPtrolApiCalling>();
builder.Services.AddScoped<IImageQualityAnalyzer, ImageBrightnessChecker>();
builder.Services.AddSingleton<IJobResultService, JobResultService>();
builder.Services.Configure<MailSetting>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IEncryption, EncryptionService>();
builder.Logging.AddConsole();

// Add Quartz services
builder.Services.AddQuartz(q => { });
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true,
    DefaultContentType = "application/octet-stream",
    ContentTypeProvider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider
    {
        Mappings =
        {
            [".m3u8"] = "application/vnd.apple.mpegurl",
            [".ts"] = "video/mp2t"
        }
    }
});

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// Map SignalR hub
app.MapHub<NotificationHub>("/notificationHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.MapRazorPages();

AuthManager.Initialize();

app.Run();