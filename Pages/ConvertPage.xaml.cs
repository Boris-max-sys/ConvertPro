using System.Globalization;
namespace ConvertPro;
public partial class ConvertPage : ContentPage
{
    static readonly CultureInfo Fr = new("fr-FR");
    string cat = "Longueur"; bool busy = true;

    public ConvertPage()
    {
        InitializeComponent();
        foreach (var c in Data.Cats.Keys)
        {
            var b = new Button { Text = c, Padding = new Thickness(14, 6), BackgroundColor = Colors.White, TextColor = Color.FromArgb("#1B2A49") };
            b.Clicked += (_, _) => SetCat(c);
            Chips.Children.Add(b);
        }
        SetCat(cat);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (Store.Pending is { } p) { Store.Pending = null; SetCat(p.Cat, p.From, p.To); }
        Refresh();
    }

    void SetCat(string c, string? f = null, string? t = null)
    {
        busy = true; cat = c;
        var codes = Data.Cats[c].Select(u => u.Code).ToList();
        From.ItemsSource = codes; To.ItemsSource = codes;
        From.SelectedIndex = Math.Max(0, codes.IndexOf(f ?? codes[0]));
        To.SelectedIndex = Math.Max(0, codes.IndexOf(t ?? codes[1]));
        foreach (Button b in Chips.Children) { bool on = b.Text == c; b.BackgroundColor = Color.FromArgb(on ? "#1F3A68" : "#FFFFFF"); b.TextColor = on ? Colors.White : Color.FromArgb("#1B2A49"); }
        busy = false; Refresh();
    }

    bool TryGet(out string f, out string t, out double v)
    {
        f = From.SelectedItem as string ?? ""; t = To.SelectedItem as string ?? ""; v = 0;
        return f != "" && t != "" && double.TryParse(Input.Text?.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out v);
    }

    void Refresh()
    {
        if (busy) return;
        Offline.IsVisible = cat == "Devises" && Connectivity.NetworkAccess != NetworkAccess.Internet;
        if (!TryGet(out var f, out var t, out var v)) { Result.Text = "\u2014"; return; }
        var r = Data.Convert(cat, f, t, v);
        Result.Text = $"{r.ToString("N" + Store.Decimals, Fr)} {t}";
        var one = Data.Convert(cat, f, t, 1).ToString("0.######", Fr);
        Formula.Text = cat == "Devises" ? $"Taux en cache \u00b7 1 {f} = {one} {t} \u00b7 {Store.RatesDate:d MMM yyyy HH:mm}"
                     : cat == "Temp\u00e9rature" ? "\u00b0F = (\u00b0C \u00d7 9/5) + 32 \u00b7 K = \u00b0C + 273,15" : $"1 {f} = {one} {t}";
        Star.Text = Store.IsFav(new(cat, f, t)) ? "\u2605" : "\u2606";
    }

    void OnChange(object? s, EventArgs e) => Refresh();
    void OnSwap(object? s, EventArgs e) { busy = true; (From.SelectedIndex, To.SelectedIndex) = (To.SelectedIndex, From.SelectedIndex); busy = false; Refresh(); }
    void OnStar(object? s, EventArgs e) { if (TryGet(out var f, out var t, out _)) { Store.ToggleFav(new(cat, f, t)); Refresh(); } }
    void OnSave(object? s, EventArgs e)
    {
        if (!TryGet(out var f, out var t, out var v)) return;
        Store.AddHistory(new(cat, f, t, v, Data.Convert(cat, f, t, v), DateTime.Now));
        DisplayAlert("Historique", "Conversion enregistr\u00e9e.", "OK");
    }
}
