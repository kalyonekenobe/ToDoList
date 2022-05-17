using ToDoList;
using ToDoList.Business.Repositories;

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
	pattern: "{controller=Tasks}/{action=List}/{category?}");


app.Run();
