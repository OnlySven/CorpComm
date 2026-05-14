using CorpComm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using CorpComm.Application.Features.Users.Commands;
using CorpComm.Domain.Repositories;
using CorpComm.Infrastructure.Repositories;
using CorpComm.Application.Common.Behaviors;
using CorpComm.Infrastructure.Configuration;
using CorpComm.Infrastructure.Services;
using CorpComm.Application.Common.Services;
using CorpComm.WebAPI.Hubs;
using Scalar.AspNetCore;
using FluentValidation;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMeetingRepository, MeetingRepository>();
builder.Services.AddScoped<IInvitationRepository, InvitationRepository>();

builder.Services.AddMediatR(cfg => 
{
    cfg.RegisterServicesFromAssembly(typeof(CorpComm.Application.Features.Users.Commands.CreateUserCommand).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddScoped<InvitationSender, SmtpEmailInvitationSender>();

// Автоматична реєстрація всіх валідаторів з Application шару
builder.Services.AddValidatorsFromAssembly(typeof(CorpComm.Application.Features.Users.Commands.CreateUserCommand).Assembly);

builder.Services.AddSignalR(); 

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapPost("/api/users", async (CreateUserCommand command, IMediator mediator) =>
{
    try
    {
        var userId = await mediator.Send(command);
        return Results.Ok(new { Id = userId });
    }

    catch (ValidationException valEx)
    {
        return Results.BadRequest(new 
        { 
            Message = "Validation failed", 
            Errors = valEx.Errors.Select(e => new 
            { 
                Field = e.PropertyName, 
                Error = e.ErrorMessage 
            }) 
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Error = ex.Message });
    }
});

app.MapGet("/api/meetings/{roomId}/token", async (string roomId, string userName, MediatR.IMediator mediator) =>
{
    try
    {
        // Відправляємо запит у MediatR
        var query = new CorpComm.Application.Features.Meetings.Queries.GetMeetingTokenQuery(roomId, userName);
        var token = await mediator.Send(query);
        
        // Повертаємо токен у форматі JSON: { "token": "eyJhbGciOi..." }
        return Results.Ok(new { Token = token });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Error = ex.Message });
    }
});

app.MapPost("/api/meetings", async (CorpComm.Application.Features.Meetings.Commands.CreateMeetingCommand command, IMediator mediator) =>
{
    try
    {
        var meetingId = await mediator.Send(command);
        var meetingLink = $"http://localhost:5173/?room={meetingId}"; 
        
        return Results.Ok(new { 
            MeetingId = meetingId, 
            Link = meetingLink 
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Error = ex.Message });
    }
});

app.MapPost("/api/meetings/invite", async (CorpComm.Application.Features.Meetings.Commands.SendInvitationCommand command, MediatR.IMediator mediator) =>
{
    try
    {
        await mediator.Send(command);
        
        return Results.Ok(new { Message = $"Запрошення успішно відправлено на {command.GuestEmail}" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Error = ex.Message });
    }
})
.WithName("SendMeetingInvitation");

app.UseHttpsRedirection();

app.MapHub<MeetingHub>("/hubs/meeting");

app.Run();