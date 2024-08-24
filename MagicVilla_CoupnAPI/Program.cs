using MagicVilla_CoupnAPI.Data;
using MagicVilla_CoupnAPI.Models;
using MagicVilla_CoupnAPI.Models.DTO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//get all the coupons
app.MapGet("/api/coupon", () =>
{
	return Results.Ok(CouponStore.coupons);
}).WithName("GetCoupons").Produces<IEnumerable<Coupon>>(200);

//get the coupons by id
app.MapGet("/api/coupon/{id:int}", (int id) =>
{
	return Results.Ok(CouponStore.coupons.FirstOrDefault(c=>c.Id==id));
}).WithName("GetCoupon").Produces<Coupon>(200);


//create coupon
app.MapPost("/api/coupon", ([FromBody] CouponCreateDTO coupon_c_dto ) =>
{
	if (string.IsNullOrEmpty(coupon_c_dto.Name))
	{
		return Results.BadRequest("Invalid Id Or Coupon Name");
	}
	if (CouponStore.coupons.FirstOrDefault(u => u.Name.ToLower() == coupon_c_dto.Name.ToLower()) != null)
	{
		return Results.BadRequest("Coupon Name is Already Exists");
	}

	Coupon coupon = new ()
	{
		Name=coupon_c_dto.Name,
		IsActive=coupon_c_dto.IsActive,
		Percent = coupon_c_dto.Percent
	};

	coupon.Id = CouponStore.coupons.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
	CouponStore.coupons.Add(coupon);

	CouponDTO couponDTO = new()
	{
		IsActive = coupon.IsActive,
		Name = coupon.Name,
		Percent = coupon.Percent,
		Id = coupon.Id,
		Created = coupon.Created
    };

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

