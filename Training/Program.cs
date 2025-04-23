using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Training.Data;
using Training.Service;
using Newtonsoft.Json.Linq;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson();
builder.Services.AddControllersWithViews();

// Cấu hình kết nối đến cơ sở dữ liệu
builder.Services.AddDbContext<TrainingDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Training"));
});

// Thêm dịch vụ Session và bộ nhớ phân tán
builder.Services.AddDistributedMemoryCache(); // Cấu hình bộ nhớ phân tán cho session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(70); // Thời gian hết hạn của session
    options.Cookie.HttpOnly = true;  // Bảo vệ cookie session khỏi JavaScript
    options.Cookie.IsEssential = true; // Đảm bảo session luôn được gửi với yêu cầu
});


builder.Services.AddScoped<AccountService, AccountServiceImpl>();
builder.Services.AddScoped<CourseService, CourseServiceImpl>();
// Khi đăng nhập thành công
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseSession();

app.UseHttpsRedirection();
app.UseRouting();

// Cấu hình middleware Session (phải đặt trước UseAuthorization)


app.UseAuthorization();

app.MapStaticAssets();

// Cấu hình Route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
