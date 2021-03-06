﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleApp2
{
    class Program
    {
        // Хеш-таблица
        public static List<string>[] hashTable = new List<string>[256];

        static void Main(string[] args)
        {
            Console.WriteLine("Введите название папки: ");

            string path = Console.ReadLine();

            for (int i = 0; i < hashTable.Length; i++)
            {
                hashTable[i] = new List<string>();
            }

            try
            {
                fillHashTable(path); 
            }
            // Если введен путь к несуществующей папке
            catch(DirectoryNotFoundException ex)
            {
                Console.WriteLine("Указано имя несуществующей папки!");
                Console.ReadLine();
                return;
            }
            // Если папка недоступна  
            catch(UnauthorizedAccessException ex)
            {
                Console.WriteLine("Указано имя недоступной папки!");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("R - Поиск файла по имени \t Q - Выход");
            string str = Console.ReadLine();

            if (str == "Q") return;

            Console.WriteLine("Введите имя файла для поиска");

            // Ввод имени файлов, пути к которым мы хотим получить 
            string fileName = Console.ReadLine();

            // Хеш-код введеного имени файла
            byte hashOfFile = getHashCode(fileName.ToLower());
           
            try
            {
                List<string> pathList = getPathsByHash(hashOfFile);
                foreach (string p in pathList)
                {
                    // Проверка на случай возникновения коллизии
                    if (Path.GetFileName(p).ToLower() == fileName.ToLower())
                        Console.WriteLine(p);
                }
            }
            /* 
             * Если не найдено файлов с таким хешом (т.е в хеш-таблице по индексу hashOfFile 
             * находится пустой List) 
             */
            catch (EmptyResultListException ex)
            {
                Console.WriteLine("Файл не найден!");
            }

            Console.ReadLine();
        }

        class EmptyResultListException : Exception { }

        // Заполнение таблицы с рекурсивным обходом содержимого папки
        public static void fillHashTable(string path)
        {
            string[] dirs = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                byte hash = getHashCode(Path.GetFileName(file).ToLower());
                hashTable[hash].Add(Path.GetFullPath(file));
            }

            foreach (string dir in dirs)
            {
                fillHashTable(dir);
            }
        }

        // Функция получения хеш-кода имени файла
        public static byte getHashCode(string fileName)
        {
            uint sum = 0;
            for (int i = 0; i < fileName.Length; i++)
                sum += fileName[i];
            return ((byte)(sum % 256));
        }

        // Получить List путей по хешу имени файла
        public static List<string> getPathsByHash(byte hash)
        {
            if (hashTable[hash].Count == 0)
                throw new EmptyResultListException();
            return hashTable[hash];      
        }

        // Функция вывода хеш-таблицы (для проверки правильности работы программы)
        public static void printHashTable(List<string>[] hashTable)
        {
            for (int i = 0; i < hashTable.Length; i++)
            {
                for (int j = 0; j < hashTable[i].Count; j++)
                {
                    Console.WriteLine("hash: " + i);
                    Console.WriteLine("\t" + hashTable[i][j]);
                }
            }
        }
    }
}
