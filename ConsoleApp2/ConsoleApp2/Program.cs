using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleApp2
{
    class Program
    {
        public static List<string>[] hashTable = new List<string>[256];
        static void Main(string[] args)
        {
            Console.WriteLine("Введите название папки: ");

            string path = Console.ReadLine(); ;

            try
            {
                fillHashTable(path);
            }
            catch(DirectoryNotFoundException ex)
            {
                Console.WriteLine("Указано имя несуществующей папки!");
                Console.ReadLine();
                return;
            }
            catch(UnauthorizedAccessException exception)
            {
                Console.WriteLine("Указано имя недоступной папки!");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("R - Продолжить" + "\t" + "Q - Выход");
            string str = Console.ReadLine();

            if (str == "R")
            {
                for (int i = 0; i < hashTable.Length; i++)
                {
                    hashTable[i] = new List<string>();
                }

                for (int i = 0; i < hashTable.Length; i++)
                {
                    for (int j = 0; j < hashTable[i].Count; j++)
                    {
                        Console.WriteLine("hash: " + i);
                        Console.WriteLine("\t" + hashTable[i][j]);
                    }
                }

                string fileName = Console.ReadLine();

                byte hashOfFile = getHashCode(fileName.ToLower());

                try
                {
                    List<string> pathList = getPathsByHash(hashOfFile);
                    foreach (string p in pathList)
                    {
                        if (Path.GetFileName(p).ToLower() == fileName)
                            Console.WriteLine(p);
                    }
                }
                catch (EmptyResultListException e)
                {
                    Console.WriteLine("Файл не найден!");
                }
            }
            else
                return;
            Console.ReadLine();
        }

        class EmptyResultListException : Exception { }

        public static void fillHashTable(string path)
        {
            string[] dirs = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                hashTable[getHashCode(Path.GetFileName(file).ToLower())].Add(Path.GetFullPath(file));
            }

            foreach (string dir in dirs)
            {
                fillHashTable(dir);
            }
        }

        public static byte getHashCode(string fileName)
        {
            uint sum = 0;
            for (int i = 0; i < fileName.Length; i++)
                sum += fileName[i];
            return ((byte)(sum % 256));
        }

        public static List<string> getPathsByHash(byte hash)
        {
            if (hashTable[hash].Count != 0)
            {
                return hashTable[hash];
            }
            else
                throw new EmptyResultListException();
        }
    }
}
