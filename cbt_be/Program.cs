using cbt.be.Validator.Admin;

using cbt.entity;
using cbt.entity.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 1. Ambil Connection String
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Siapkan NpgsqlDataSourceBuilder SEBELUM mendaftarkan DbContext
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.EnableUnmappedTypes();

// PENTING: MapEnum harus dipanggil SEBELUM dataSourceBuilder.Build()
dataSourceBuilder.MapEnum<ActivityType>("public.activity_type");
dataSourceBuilder.MapEnum<AttemptStatus>("public.attempt_status");
dataSourceBuilder.MapEnum<ExamStatus>("public.exam_status");
dataSourceBuilder.MapEnum<QuestionType>("public.question_type");
dataSourceBuilder.MapEnum<SubmitType>("public.submit_type");
dataSourceBuilder.MapEnum<UserRole>("public.user_role");
dataSourceBuilder.MapEnum<UserStatus>("public.user_status");
dataSourceBuilder.MapEnum<ViolationType>("public.violation_type");

var dataSource = dataSourceBuilder.Build();

// 3. Daftarkan DbContext HANYA SEKALI menggunakan dataSource yang sudah dikonfigurasi
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(dataSource));

// 4. Daftarkan FluentValidation dan MediatR
builder.Services.AddValidatorsFromAssemblyContaining<GetActivityLogsValidator>();

builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();