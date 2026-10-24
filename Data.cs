using System.Text.Json;
namespace ConvertPro;
public record Unit(string Code, string Name, double Factor);
public record Fav(string Cat, string From, string To);
public record Entry(string Cat, string From, string To, double Value, double Result, DateTime At);
public record Row(string Title, string Sub, Fav? Fav = null);

public static class Data
{
    public static readonly Dictionary<string, Unit[]> Cats = new()
    {
        ["Longueur"] = [new("km","Kilomètre",1000), new("mi","Mile",1609.344), new("m","Mètre",1), new("cm","Centimètre",.01), new("mm","Millimètre",.001), new("in","Pouce",.0254), new("ft","Pied",.3048), new("yd","Yard",.9144)],
        ["Masse"] = [new("kg","Kilogramme",1), new("lb","Livre",.45359237), new("g","Gramme",.001), new("oz","Once",.0283495231)],
        ["Température"] = [new("°C","Celsius",1), new("°F","Fahrenheit",1), new("K","Kelvin",1)],
        ["Surface"] = [new("m²","Mètre carré",1), new("km²","Kilomètre carré",1e6), new("ha","Hectare",1e4)],
        ["Volume"] = [new("L","Litre",1), new("mL","Millilitre",.001), new("m³","Mètre cube",1000)],
        ["Vitesse"] = [new("km/h","km/h",1/3.6), new("m/s","m/s",1), new("mph","mph",.44704)],
        ["Temps"] = [new("s","Seconde",1), new("min","Minute",60), new("h","Heure",3600), new("jour","Jour",86400)],
        ["Données"] = [new("o","Octet",1), new("Ko","Kilooctet",1e3), new("Mo","Mégaoctet",1e6), new("Go","Gigaoctet",1e9)],
        // Devises : facteur = taux pour 1 EUR (jeu de démonstration)
        ["Devises"] = [new("EUR","Euro",1), new("USD","Dollar américain",1.1), new("GBP","Livre sterling",.86), new("CHF","Franc suisse",.94), new("CAD","Dollar canadien",1.5), new("JPY","Yen japonais",160)],
    };

    public static double Convert(string cat, string from, string to, double v)
    {
        var a = Cats[cat].First(x => x.Code == from); var b = Cats[cat].First(x => x.Code == to);
        if (cat == "Température")
        {
            double c = from switch { "°F" => (v - 32) * 5 / 9, "K" => v - 273.15, _ => v };
            return to switch { "°F" => c * 9 / 5 + 32, "K" => c + 273.15, _ => c };
        }
        return cat == "Devises" ? v / a.Factor * b.Factor : v * a.Factor / b.Factor;
    }
}

public static class Store
{
    public static Fav? Pending;
    static List<T> Load<T>(string k) => JsonSerializer.Deserialize<List<T>>(Preferences.Get(k, "[]")) ?? new();
    static void Save<T>(string k, List<T> l) => Preferences.Set(k, JsonSerializer.Serialize(l));

    public static List<Entry> History => Load<Entry>("hist");
    public static void AddHistory(Entry e) { var l = History; l.Insert(0, e); Save("hist", l.Take(50).ToList()); }
    public static void ClearHistory() => Preferences.Remove("hist");

    public static List<Fav> Favs => Load<Fav>("favs");
    public static bool IsFav(Fav f) => Favs.Contains(f);
    public static void ToggleFav(Fav f) { var l = Favs; if (!l.Remove(f)) l.Add(f); Save("favs", l); }

    public static int Decimals { get => Preferences.Get("dec", 2); set => Preferences.Set("dec", value); }
    public static bool CacheRates { get => Preferences.Get("cache", true); set => Preferences.Set("cache", value); }
    public static string PreferredCurrency { get => Preferences.Get("cur", "EUR"); set => Preferences.Set("cur", value); }
    public static DateTime RatesDate
    {
        get => DateTime.Parse(Preferences.Get("rd", "2026-10-06T09:30:00"));
        set => Preferences.Set("rd", value.ToString("s"));
    }
}
