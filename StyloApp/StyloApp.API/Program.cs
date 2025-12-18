
using StyloApp.API.Services;
using StyloApp.API.Data;
using Microsoft.EntityFrameworkCore;
using StyloApp.API.Core.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);
// --- 1. CẤU HÌNH SECRET KEY (Nên để trong appsettings.json, nhưng ở đây tôi fix cứng để bạn test) ---
var jwtSecretKey = "Day_La_Chuoi_Bi_Mat_Sieu_Cap_Vip_Pro_Cua_Stylo_App_Nam_2025_Ngon_Lanh_Canh_Dao";
var key = Encoding.UTF8.GetBytes(jwtSecretKey);

// 1. KHAI BÁO CHÍNH SÁCH CORS (Thêm vào trước builder.Build())
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFlutterApp",
        policy =>
        {
            policy.AllowAnyOrigin() // Cho phép tất cả các nguồn (Origin)
                  .AllowAnyHeader() // Cho phép tất cả các Header
                  .AllowAnyMethod(); // Cho phép tất cả các phương thức (GET, POST,...)
        });
});
// Add services to the container.
builder.Services.AddDbContext<FashionShopContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddControllers();
builder.Services.AddHttpClient();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Stylo App API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập Token theo cú pháp: Bearer {your_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});
builder.Services.AddMemoryCache();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<ProductService>();
// --- 4. CẤU HÌNH AUTHENTICATION (Xác thực JWT) ---
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero // Loại bỏ thời gian trễ mặc định (5p) để test hết hạn chính xác hơn
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseMiddleware<ExceptionMiddleware>();
}

app.UseHttpsRedirection();

// 2. SỬ DỤNG MIDDLEWARE CORS (Phải đặt ĐÚNG THỨ TỰ)
// Nó phải nằm SAU UseHttpsRedirection và TRƯỚC UseAuthentication/UseAuthorization
app.UseCors("AllowFlutterApp");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
