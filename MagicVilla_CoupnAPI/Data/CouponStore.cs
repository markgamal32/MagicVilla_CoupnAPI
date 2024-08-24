using MagicVilla_CoupnAPI.Models;

namespace MagicVilla_CoupnAPI.Data
{
	public static class CouponStore
	{
		public static List<Coupon>  coupons = new List<Coupon> {
			new Coupon{Id=1,Name="10ff",Percent=10,IsActive=true},
			new Coupon{Id=2,Name="20ff",Percent=20,IsActive=false}
		};
		


	}
}
