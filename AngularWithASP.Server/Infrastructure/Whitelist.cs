namespace AngularWithASP.Server.Infrastructure
{
  public static class Whitelist
  {
    public static List<string> URLs = new List<string>{
      OkUrls.StorePage,
      OkUrls.ChessCat,
      OkUrls.SoccerCat,
      OkUrls.WaterSportsCat,
      OkUrls.WaterSportsCat2,
      OkUrls.CartCheckout
    };
  }
}
