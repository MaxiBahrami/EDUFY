public class City
{
    public int id { get; set; }
    public string name { get; set; }
    public string latitude { get; set; }
    public string longitude { get; set; }
}

public class CountryDto
{
    public int id { get; set; }
    public string name { get; set; }
    public string iso3 { get; set; }
    public string iso2 { get; set; }

    public List<City> cities { get; set; }
}