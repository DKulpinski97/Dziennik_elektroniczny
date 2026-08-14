using System.Text.RegularExpressions;

namespace Dziennik_szkolny.Application.Walidacja.Uzytkownik
{
    public class WalidacjaDanychUzytkownika
    {
        public bool CzyPoprawnyTelefon(string telefon)
        {
            return !string.IsNullOrWhiteSpace(telefon)
                && Regex.IsMatch(telefon, @"^\d{9}$");
        }
        public bool CzyPoprawnyPesel(string pesel)
        {
            if (string.IsNullOrWhiteSpace(pesel) || pesel.Length != 11 || !pesel.All(char.IsDigit))
            {
                return false;
            }


            int rok = int.Parse(pesel.Substring(0, 2));
            int miesiac = int.Parse(pesel.Substring(2, 2));
            int dzien = int.Parse(pesel.Substring(4, 2));


            int stulecie;


            if (miesiac >= 1 && miesiac <= 12)
            {
                stulecie = 1900;
            }
            else if (miesiac >= 21 && miesiac <= 32)
            {
                stulecie = 2000;
                miesiac -= 20;
            }
            else if (miesiac >= 41 && miesiac <= 52)
            {
                stulecie = 2100;
                miesiac -= 40;
            }
            else if (miesiac >= 61 && miesiac <= 72)
            {
                stulecie = 2200;
                miesiac -= 60;
            }
            else if (miesiac >= 81 && miesiac <= 92)
            {
                stulecie = 1800;
                miesiac -= 80;
            }
            else
            {
                return false;
            }


            try
            {
                _ = new DateTime(
                    stulecie + rok,
                    miesiac,
                    dzien);
            }
            catch
            {
                return false;
            }


            int[] wagi =
            {
                1, 3, 7, 9,
                1, 3, 7, 9,
                1, 3
            };


            int suma = 0;


            for (int i = 0; i < 10; i++)
            {
                suma += (pesel[i] - '0') * wagi[i];
            }


            int kontrolna =
                (10 - (suma % 10)) % 10;


            return kontrolna == pesel[10] - '0';
        }
    }
}
