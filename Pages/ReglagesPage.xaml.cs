namespace ConvertPro;
public partial class ReglagesPage : ContentPage
{
    public ReglagesPage()
    {
        InitializeComponent();
        Dec.ItemsSource = new[] { "0", "2", "4", "6" }; Dec.SelectedItem = Store.Decimals.ToString();
        Cur.ItemsSource = Data.Cats["Devises"].Select(u => u.Code).ToList(); Cur.SelectedItem = Store.PreferredCurrency;
        Cache.IsToggled = Store.CacheRates; ShowRates();
    }
    void ShowRates() => Rates.Text = $"Dernière mise à jour : {Store.RatesDate:d MMMM yyyy à HH:mm} (base EUR)\n" +
        string.Join("\n", Data.Cats["Devises"].Select(u => $"{u.Code} · {u.Name} : {u.Factor:0.0000}"));
    void OnDec(object? s, EventArgs e) { if (Dec.SelectedItem is string d) Store.Decimals = int.Parse(d); }
    void OnCur(object? s, EventArgs e) { if (Cur.SelectedItem is string c) Store.PreferredCurrency = c; }
    void OnCache(object? s, ToggledEventArgs e) => Store.CacheRates = e.Value;
    void OnRefresh(object? s, EventArgs e) { Store.RatesDate = DateTime.Now; ShowRates(); }
}
