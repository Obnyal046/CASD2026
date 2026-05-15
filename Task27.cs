using System;
using System.IO;
using System.Text.RegularExpressions;
using Task25;  // подключение готового хеш-множества из задачи 25

namespace Task27
{
    class Program
    {
        static void Main()
        {
            string filePath = "input.txt";
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл input.txt не найден.");
                return;
            }

            MyHashSet<string> uniqueWords = new MyHashSet<string>();
            Regex wordRegex = new Regex(@"[A-Za-z]+");

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    MatchCollection matches = wordRegex.Matches(line);
                    foreach (Match m in matches)
                    {
                        string word = m.Value.ToLower();
                        uniqueWords.Add(word);
                    }
                }
            }

            Console.WriteLine("Уникальные слова (регистронезависимо):");
            foreach (string word in uniqueWords.ToList())
            {
                Console.WriteLine(word);
            }
        }
    }
}
