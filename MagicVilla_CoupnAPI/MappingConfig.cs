using AutoMapper;
using MagicVilla_CoupnAPI.Models;
using MagicVilla_CoupnAPI.Models.DTO;
namespace MagicVilla_CoupnAPI
{
	public class MappingConfig : Profile
	{
        public MappingConfig()
        {
            CreateMap<Coupon,CouponCreateDTO>().ReverseMap();
			CreateMap<Coupon, CouponDTO>().ReverseMap();
		}
	}
}
