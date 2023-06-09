using HotelBooking.Business;
using HotelBooking.Filter;
using HotelBooking.Persistence;
using HotelBooking.Persistence.SqlServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionFilter>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IBookingBusiness, BookingBusiness>();
builder.Services.AddScoped<IRoomBusiness, RoomBusiness>();
builder.Services.AddScoped<IBookingsDataManager, BookingsDataManager>();
builder.Services.AddScoped<IGuestsDataManager, GuestsDataManager>();
builder.Services.AddScoped<IRoomsDataManager, RoomsDataManager>();
builder.Services.AddScoped<IDateTimeUtcProvider, DateTimeUtcProvider>();
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

ConfigurationManager configuration = builder.Configuration;
builder.Services.AddScoped<ISqlConnection>(_ => new SqlConnectionWrapper(configuration.GetConnectionString("HotelBooker")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
