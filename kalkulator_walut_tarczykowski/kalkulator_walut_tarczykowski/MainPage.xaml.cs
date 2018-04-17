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
            if(dane!="")
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
            lbxZWaluty.SelectedIndex = 0;
            lbxNaWalute.SelectedIndex = 0;
        }

        private void txtKwota_TextChanged(object sender, TextChangedEventArgs e)
        {
            //odczyt wczytanej kwoty
            // konwersja na liczbe
            //przeliczenie z waluty na PLN oraz z PLN na waluete docelowa
            PozycjaTabeliA zWaluty = kursyAktualne[lbxZWaluty.SelectedIndex];
        }
    }
}
