namespace AngularWithASP.Server
{
  public enum Cat
  {
    none,
    soccer,
    chess,
    waterSport
  }

  public class ProductCategory
  {
    public Int32 ID { get; set; }
    public string Title { get; set; }
    public string Segment { get; set; }

    public static Cat ParseCat(string input)
    {
      try
      {
        Cat parsedCat;
        if (Enum.TryParse(input, out parsedCat))
        {
          Cat result = Cat.none;
          switch (parsedCat)
          {
            case Cat.none: result = Cat.none; break;
            case Cat.soccer: result = Cat.soccer; break;
            case Cat.chess: result = Cat.chess; break;
            case Cat.waterSport: result = Cat.waterSport; break;
          }
          return result;
        }
        throw new Exception("could not parse string as Cat enum");
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public static string ParseCat(Cat cat)
    {
      string myCat = "";
      switch (cat)
      {
        case Cat.none: myCat = "None"; break;
        case Cat.soccer: myCat = "Soccer"; break;
        case Cat.chess: myCat = "Chess"; break;
        case Cat.waterSport: myCat = "WaterSport"; break;
      }
      return myCat;
    }
  }
}
