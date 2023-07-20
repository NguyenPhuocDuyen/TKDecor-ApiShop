using BE_TKDecor.Core.Config.Automapper;
using BE_TKDecor.Core.Config.JWT;
using BE_TKDecor.Hubs;
using DataAccess.Data;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Utility.Mail;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// add signalR
builder.Services.AddSignalR();

// config automapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// config send mail
builder.Services.AddOptions();                                         // Kích hoạt Options
var mailsettings = builder.Configuration.GetSection("MailSettings");  // đọc config
builder.Services.Configure<MailSettings>(mailsettings);
builder.Services.AddTransient<ISendMailService, SendMailService>();

// config setting jwt
var jwtsettings = builder.Configuration.GetSection("JwtSettings");  // đọc config jwt setting
builder.Services.Configure<JwtSettings>(jwtsettings);

// Configure JWT authentication
byte[] key = Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:SecretKey"]);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        //options.RequireHttpsMetadata = false;
        //options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Tự cấp token (nếu có xài dịch vụ đăng nhập ngoài(google,..) thì set thành true)
            ValidateIssuer = false,
            ValidateAudience = false,

            // Ký token
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),

            ClockSkew = TimeSpan.Zero
        };
    });

// service Database
builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductReviewRepository, ProductReviewRepository>();
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<IProductFavoriteRepository, ProductFavoriteRepository>();
builder.Services.AddScoped<IUserAddressRepository, UserAddressRepository>();
builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IProductReportRepository, ProductReportRepository>();
builder.Services.AddScoped<IReportProductReviewRepository, ReportProductReviewRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IProductReviewInteractionRepository, ProductReviewInteractionRepository>();
builder.Services.AddScoped<IProduct3DModelRepository, Product3DModelRepository>();
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();

// config json no loop data
builder.Services.AddControllersWithViews()
        .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
    );
// config bearer token
builder.Services.AddSwaggerGen(swagger =>
{
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
    });

    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}
//add
using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    await dbInitializer.Initialize();
}

app.UseRouting();
app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// add hub
app.MapHub<ChatHub>("/hubs/chat");
app.MapHub<UserHub>("/hubs/user");
//app.MapHub<NotificationHub>("/hubs/notification");

app.Run();
