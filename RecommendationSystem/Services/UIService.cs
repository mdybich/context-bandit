using System;
using System.Collections.Generic;
using System.Text;

namespace RecommendationSystem.Services
{
    public class UIService
    {
        public void DisplayApplication()
        {
            DisplayApplicationInfo();
            DisplayMenu();
        }

        private void DisplayApplicationInfo()
        {
            Console.WriteLine("*****************************************");
            Console.WriteLine("*                                       *");
            Console.WriteLine("*         Recommendation System         *");
            Console.WriteLine("*                                       *");
            Console.WriteLine("*****************************************");
            Console.Write("\n\n\n");
        }

        private void DisplayMenu()
        {
            while(true)
            {
                Console.WriteLine("Age (Young Middle Adult):");
                var age = Console.ReadLine();
                Console.WriteLine("Sex (F, M)");
                var sex = Console.ReadLine();
                Console.WriteLine("Education (Primary Seceondary Higher)");
                var education = Console.ReadLine();

                Console.WriteLine("Your Recommendation:");
                Console.WriteLine("Another recommendation? (Y/N)");
                var option = Console.ReadLine();
                if (option == "N")
                {
                    break;
                }
            }
        }
    }
}
