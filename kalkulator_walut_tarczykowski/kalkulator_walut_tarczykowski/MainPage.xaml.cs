using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace kalkulator_walut_tarczykowski
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        const string daneNBP = "http://www.nbp.pl/kursy/xml/LastA.xml";
        List<PozycjaTabeliA> kursyAktualne = new List<PozycjaTabeliA>();
        public MainPage()
        {
            this.InitializeComponent();
        }
        private void zmianaWaluty()
        {
            //odczyt wczytanej kwoty
            // konwersja na liczbe
            //przeliczenie z waluty na PLN oraz z PLN na waluete docelowa
            double kwota;
            if (double.TryParse(txtKwota.Text, out kwota))
            {
                var zWaluty = lbxZWaluty.SelectedItem as PozycjaTabeliA;
                var naWalute = lbxNaWalute.SelectedItem as PozycjaTabeliA;
                string sepAkt = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
                double kurs;
                double.TryParse(zWaluty.kurs_sredni.Replace(",", sepAkt), out kurs);
                double kwotaPrzeliczona = kwota * kurs;
                double.TryParse(naWalute.kurs_sredni.Replace(",", sepAkt), out kurs);
                kwotaPrzeliczona = kwotaPrzeliczona / kurs;
                tbPrzeliczona.Text = string.Format("{0:F2} {1}", kwotaPrzeliczona, naWalute.kod_waluty);
            }
            else
            {
                tbPrzeliczona.Text = "";
            }
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var serwerNBP = new HttpClient();
            string dane = "";
            try
            {
                dane = await serwerNBP.GetStringAsync(new Uri(daneNBP));
            }
            catch (Exception)
            {
                throw;
            }
            if (dane != "")
            {
                kursyAktualne.Clear();
                XDocument daneKursowe = XDocument.Parse(dane);
                kursyAktualne = (from item in daneKursowe.Descendants("pozycja")
                                 select new PozycjaTabeliA()
                                 {
                                     przelicznik = (item.Element("przelicznik").Value),
                                     kod_waluty = item.Element("kod_waluty").Value,
                                     kurs_sredni = item.Element("kurs_sredni").Value
                                 }).ToList();
            }
            kursyAktualne.Insert(0, new PozycjaTabeliA() { kurs_sredni = "1,0000", kod_waluty = "PLN", przelicznik = "1" });
            lbxZWaluty.ItemsSource = kursyAktualne;
            lbxNaWalute.ItemsSource = kursyAktualne;
            string klucz = lbxZWaluty.Name;
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(klucz))
            {
                lbxZWaluty.SelectedIndex = (int)ApplicationData.Current.LocalSettings.Values[klucz];
            }
            else
            {
                lbxZWaluty.SelectedIndex = 0;
            }
            klucz = lbxNaWalute.Name;
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(klucz))
            {
                lbxNaWalute.SelectedIndex = (int)ApplicationData.Current.LocalSettings.Values[klucz];
            }
            else
            {
                lbxNaWalute.SelectedIndex = 0;
            }
        }

        private void txtKwota_TextChanged(object sender, TextChangedEventArgs e)
        {
            zmianaWaluty();
        }

        private void lbxZWaluty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            zmianaWaluty();
            var listBox = sender as ListBox;
            int co = listBox.SelectedIndex;
            string gdzie = listBox.Name;
            ApplicationData.Current.LocalSettings.Values[gdzie] = co;
        }
    }
}
