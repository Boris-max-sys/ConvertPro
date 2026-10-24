using System.Globalization;
namespace ConvertPro;
public partial class HistoriquePage : ContentPage
{
    static readonly CultureInfo Fr = new("fr-FR");
    public HistoriquePage() { InitializeComponent(); }
    protected override void OnAppearing() { base.OnAppearing(); Load(); }
    void Load() => List.ItemsSource = Store.History.Select(h =>
        new Row($"{h.Value.ToString("0.##", Fr)} {h.From} → {h.Result.ToString("N" + Store.Decimals, Fr)} {h.To}", $"{h.At:dd/MM HH:mm} · {h.Cat}")).ToList();
    void OnClear(object? s, EventArgs e) { Store.ClearHistory(); Load(); }
}
