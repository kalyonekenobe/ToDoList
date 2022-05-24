using ToDoList;
using ToDoList.Business.Repositories;
using ToDoList.API.GraphQL;
using GraphQL;
using GraphQL.Types;
using GraphQL.Server;
using GraphQL.MicrosoftDI;
using GraphQL.SystemTextJson;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromSeconds(60);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

builder.Services.AddTransient<ITaskRepository, ToDoList.MsSqlDb.Repositories.TaskRepository>();
builder.Services.AddTransient<ICategoryRepository, ToDoList.MsSqlDb.Repositories.CategoryRepository>();
builder.Services.AddTransient<IStatusRepository, ToDoList.MsSqlDb.Repositories.StatusRepository>();

builder.Services.AddTransient<ITaskRepository, ToDoList.Xml.Repositories.TaskRepository>();
builder.Services.AddTransient<ICategoryRepository, ToDoList.Xml.Repositories.CategoryRepository>();
builder.Services.AddTransient<IStatusRepository, ToDoList.Xml.Repositories.StatusRepository>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddSingleton<ISchema, ToDoListSchema>(services => new ToDoListSchema(new SelfActivatingServiceProvider(services)));

builder.Services.AddGraphQL(options => options.AddSystemTextJson()
											  .AddErrorInfoProvider(opts =>
											  {
												  opts.ExposeExceptionStackTrace = true;
											  })
											  .AddSchema<ToDoListSchema>()
											  .AddGraphTypes(typeof(ToDoListSchema).Assembly));
											  

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

app.UseGraphQLAltair();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Tasks}/{action=List}/{category?}");


app.Run();
