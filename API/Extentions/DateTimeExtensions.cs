using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Extentions
{
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateTime dateOfBirth)
        {
            //ova extenstion metoda racuna broj godina u odnosu na datum rodjenja
            
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;

            if (dateOfBirth.Date > today.AddYears(-age)) age--; //ako mu je rodj posle danasnjeg datuma onda jos nije slavio rodj
            return age;                                         //pa mu smanjujemo broj godina za 1 

        }







    }
}
