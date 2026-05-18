using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using TemplateProject.Api.Middlewares;
using TemplateProject.Api.Swagger;
using TemplateProject.Application.Features.Persons.DTOs;
using TemplateProject.Application.Features.Persons.Interfaces;
using TemplateProject.Application.Features.Persons.Validators;
using TemplateProject.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreatePersonRequestValidator>();
builder.Services.AddScoped<IPersonRepository, PersonRepository>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.DocumentTitle = "TemplateProject API";
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "TemplateProject API v1");
});
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
