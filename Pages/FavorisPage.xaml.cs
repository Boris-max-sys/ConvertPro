namespace ConvertPro;
public partial class FavorisPage : ContentPage
{
    public FavorisPage() { InitializeComponent(); }
    protected override void OnAppearing() { base.OnAppearing(); List.ItemsSource = Store.Favs.Select(f => new Row($"{f.From} → {f.To}", f.Cat, f)).ToList(); }
    async void OnTap(object? s, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Row { Fav: { } f }) { Store.Pending = f; List.SelectedItem = null; await Shell.Current.GoToAsync("//convert"); }
    }
}
