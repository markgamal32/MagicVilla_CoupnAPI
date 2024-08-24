using MagicVilla_CoupnAPI;
using MagicVilla_CoupnAPI.Data;
using MagicVilla_CoupnAPI.Models;
using MagicVilla_CoupnAPI.Models.DTO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingConfig));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//get all the coupons and injecting ILogger
app.MapGet("/api/coupon", (ILogger<Program> _logger) =>
{
	// using built-in  ILogger to print Information massage 'Get all Coupons'
	_logger.Log(LogLevel.Information, "Get all Coupons");
	return Results.Ok(CouponStore.coupons);
}).WithName("GetCoupons").Produces<IEnumerable<Coupon>>(200);

//get the coupons by id
app.MapGet("/api/coupon/{id:int}", (int id) =>
{
	return Results.Ok(CouponStore.coupons.FirstOrDefault(c=>c.Id==id));
}).WithName("GetCoupon").Produces<Coupon>(200);


//create coupon and injecting ( IMapper _IMapper )
app.MapPost("/api/coupon", (IMapper _IMapper, [FromBody] CouponCreateDTO coupon_c_dto ) =>
{
	if (string.IsNullOrEmpty(coupon_c_dto.Name))
	{
		return Results.BadRequest("Invalid Id Or Coupon Name");
	}
	if (CouponStore.coupons.FirstOrDefault(u => u.Name.ToLower() == coupon_c_dto.Name.ToLower()) != null)
	{
		return Results.BadRequest("Coupon Name is Already Exists");
	}

	// using AutoMapper to convert from coupon_c_dto obj to Coupon obj
	Coupon coupon = _IMapper.Map<Coupon>(coupon_c_dto);

	coupon.Id = CouponStore.coupons.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
	CouponStore.coupons.Add(coupon);

	// using AutoMapper to convert from coupon obj to CouponDTO obj
	CouponDTO couponDTO = _IMapper.Map<CouponDTO>(coupon);

	return Results.CreatedAtRoute("GetCoupon", new {id= couponDTO.Id}, couponDTO);
}).WithName("CreatedCoupon").Accepts<CouponCreateDTO>("/application/json").Produces<CouponDTO>(201).Produces(400);


//edit coupon
app.MapPut("/api/coupon",() =>
{


});

//delete coupon
app.MapDelete("/api/coupon/{id:int}",(int id) =>
{
	 //return Results.Ok(CouponStore.coupons.FirstOrDefault(c => c.Id == id));
});

app.Run();

