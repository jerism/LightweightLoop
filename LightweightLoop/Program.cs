using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace LightweightLoop
{
    internal class Program
    {
        const string alphabetLower = "abcdefghijklmnopqrstuvwxyz";
        const string alphabetUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        static readonly Random random = new();
        const int LoopCount = 100;
        static int RunningCount = 0;
        static int CurrentLoop = 0;
        static readonly bool UseRealWords = true;
        static List<string> ValidWords;
        static int WordCount;

        static void Main(string[] args)
        {
            ValidWords = LoadWords();
            WordCount = ValidWords.Count;
            Loop();
        }

        private static void Loop()
        {
            while (true)
            {
                CurrentLoop++;
                RunningCount++;

                KeepAlive();
                Thread.Sleep(500);

                var word = UseRealWords ? RealWord() : RandomWord();

                if (!UseRealWords && IsARealWord(word))
                {
                    Console.WriteLine($"Congrats '{word}' is a real word. It took you {RunningCount} attempts");
                }
                else
                {
                    Console.WriteLine(word);
                }

                if (CurrentLoop == LoopCount)
                {
                    Console.Clear();
                    CurrentLoop = 0;
                }
            }
        }

        private static string RandomWord()
        {
            var length = random.Next(0, 10);
            var word = string.Empty;

            for (var i = 0; i < length; i++)
            {
                int characterPos = random.Next(0, alphabetLower.Length);
                var character = i == 0 ? alphabetUpper[characterPos] : alphabetLower[characterPos];
                word = $"{word}{character}";
            }

            return word;
        }

        private static string RealWord()
        {
            var length = random.Next(0, WordCount);
            var word = ValidWords[length];
            return word;
        }

        private static bool IsARealWord(string word)
        {
            return ValidWords.Any(s => s == word);
        }

        private static List<string> LoadWords()
        {
            var lines = File.ReadAllLines($"{Directory.GetCurrentDirectory()}/words.txt");
            return lines.ToList();
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern uint SetThreadExecutionState(EXECUTION_STATE esFlags);

        private static void KeepAlive()
        {
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_SYSTEM_REQUIRED);
        }

        [Flags]
        public enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001
        }
    }
}
