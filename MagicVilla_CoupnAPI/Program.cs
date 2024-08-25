using MagicVilla_CoupnAPI;
using MagicVilla_CoupnAPI.Data;
using MagicVilla_CoupnAPI.Models;
using MagicVilla_CoupnAPI.Models.DTO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using FluentValidation;
using System.Net;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();


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
	// using built-in  ILogger to print Information message 'Get all Coupons'
	_logger.Log(LogLevel.Information, "Get all Coupons");
	ApiResponse response = new();
	response.Result = CouponStore.coupons;
	response.IsSuccess = true;
	response.StatusCode=HttpStatusCode.OK;
	
	return Results.Ok(response);
}).WithName("GetCoupons").Produces<ApiResponse>(200);

//get the coupons by id
app.MapGet("/api/coupon/{id:int}", (int id) =>
{
	ApiResponse response = new();
	response.Result = CouponStore.coupons.FirstOrDefault(c => c.Id == id);
	response.IsSuccess = true;
	response.StatusCode = HttpStatusCode.OK;
	return Results.Ok(response);
}).WithName("GetCoupon").Produces<ApiResponse>(200);


//create coupon and injecting ( IMapper _IMapper )
app.MapPost("/api/coupon",  async (IMapper _IMapper,IValidator<CouponCreateDTO> _validator ,[FromBody] CouponCreateDTO coupon_c_dto ) =>
{
	// initial values of the response obj 
	ApiResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
	
	// using Async , Await instead of =>>  .GetAwaiter().GetResult()
	var validationResult =await  _validator.ValidateAsync(coupon_c_dto);
	if (!validationResult.IsValid)
	{
		// using validationResult to return the error message
		response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
		return Results.BadRequest(response);
	}
	if (CouponStore.coupons.FirstOrDefault(u => u.Name.ToLower() == coupon_c_dto.Name.ToLower()) != null)
	{
		response.ErrorMessages.Add("Coupon Name is Already Exists");
		return Results.BadRequest(response);
	}

	// using AutoMapper to convert from coupon_c_dto obj to Coupon obj
	Coupon coupon = _IMapper.Map<Coupon>(coupon_c_dto);

	coupon.Id = CouponStore.coupons.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
	CouponStore.coupons.Add(coupon);

	// using AutoMapper to convert from coupon obj to CouponDTO obj
	CouponDTO couponDTO = _IMapper.Map<CouponDTO>(coupon);

	response.Result = couponDTO;
	response.IsSuccess = true;
	response.StatusCode = HttpStatusCode.Created;

	return Results.Ok(response);
}).WithName("CreatedCoupon").Accepts<CouponCreateDTO>("application/json").Produces<ApiResponse>(201).Produces(400);




//edit coupon
app.MapPut("/api/coupon",async (IMapper _IMapper, IValidator<CouponUpdateDTO> _validator, [FromBody] CouponUpdateDTO coupon_u_dto) =>
{
	ApiResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
	var validationResult = await _validator.ValidateAsync(coupon_u_dto);
	if (!validationResult.IsValid)
	{
		// using validationResult to return the error message
		response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
		return Results.BadRequest(response);
	}
	if (CouponStore.coupons.FirstOrDefault(u => u.Name.ToLower() == coupon_u_dto.Name.ToLower()) != null)
	{
		response.ErrorMessages.Add("Coupon Name is Already Exists");
		return Results.BadRequest(response);
	}

	Coupon couponFromStore = CouponStore.coupons.FirstOrDefault(u => u.Id == coupon_u_dto.Id);
	couponFromStore.Name = coupon_u_dto.Name;
	couponFromStore.Percent = coupon_u_dto.Percent;
	couponFromStore.IsActive = coupon_u_dto.IsActive;
	couponFromStore.LastUpdated = DateTime.Now;

	

	response.Result = _IMapper.Map<CouponDTO>(couponFromStore);
	response.IsSuccess = true;
	response.StatusCode = HttpStatusCode.OK;

	return Results.Ok(response);


}).WithName("UpdatedCoupon").Accepts<CouponUpdateDTO>("application/json").Produces<ApiResponse>(200).Produces(400); 


//delete coupon
app.MapDelete("/api/coupon/{id:int}",(int id) =>
{
	ApiResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
	Coupon couponFromStore = CouponStore.coupons.FirstOrDefault(u => u.Id == id);
    if (couponFromStore != null)
    {
		CouponStore.coupons.Remove(couponFromStore);
		response.IsSuccess = true;
		response.StatusCode = HttpStatusCode.NoContent;
		return Results.Ok(response);
	}
    else
    {
		response.ErrorMessages.Add("Invalid Id");
		return Results.BadRequest(response);

	}

}).WithName("DeleteCoupon");

app.Run();

