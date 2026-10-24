namespace ConvertPro;
public partial class ReglagesPage : ContentPage
{
    public ReglagesPage()
    {
        InitializeComponent();
        Dec.ItemsSource = new[] { "0", "2", "4", "6" }; Dec.SelectedItem = Store.Decimals.ToString();
        Cur.ItemsSource = Data.Cats["Devises"].Select(u => u.Code).ToList(); Cur.SelectedItem = Store.PreferredCurrency;
        Theme.ItemsSource = new[] { "Syst\u00e8me", "Clair", "Sombre" }; Theme.SelectedItem = Store.Theme;
        Cache.IsToggled = Store.CacheRates; ShowRates();
    }
    void ShowRates() => Rates.Text = $"Derni\u00e8re mise \u00e0 jour : {Store.RatesDate:d MMMM yyyy \u00e0 HH:mm} (base EUR)\n" +
        string.Join("\n", Data.Cats["Devises"].Select(u => $"{u.Code} \u00b7 {u.Name} : {u.Factor:0.0000}"));
    void OnDec(object? s, EventArgs e) { if (Dec.SelectedItem is string d) Store.Decimals = int.Parse(d); }
    void OnCur(object? s, EventArgs e) { if (Cur.SelectedItem is string c) Store.PreferredCurrency = c; }
    void OnTheme(object? s, EventArgs e) { if (Theme.SelectedItem is string t) { Store.Theme = t; Store.ApplyTheme(); } }
    void OnCache(object? s, ToggledEventArgs e) => Store.CacheRates = e.Value;
    void OnRefresh(object? s, EventArgs e) { Store.RatesDate = DateTime.Now; ShowRates(); }
}
