
using System.Diagnostics;

namespace ReedSolomon
{
    internal static class Program
    {
        static void Main()
        {
            var coder = new Coder(0b10000_00001_10001_00001);
            var codedMessage = coder.MessageCoded; //zapis liczby pomija zera na początku
            var decodedMessage = codedMessage / coder.GetGenerator;
            var str = decodedMessage.PolyToBits();
            Console.WriteLine($"Decoded message in bits: {str}");

            int[] vectorA = [1, 2, 3, 4];
            int[] vectorB = [5, 2, 3, 4];
            var distance = HammingDistance(vectorA, vectorB);
            
            Console.WriteLine($"Hamming Distance between {vectorA.Aggregate("",(current, item) => current + item + " ")} ({vectorA.Aggregate("",(current, item) => current + $"{item:b5}" + " ")}) and {vectorB.Aggregate("",(current, item) => current + item + " ")} ({vectorB.Aggregate("",(current, item) => current + $"{item:b5}" + " ")}) is {distance.Aggregate("", (current, item) => current + item + " ")}");
            
        }
        static IEnumerable<int> HammingDistance(IEnumerable<int> a, IEnumerable<int> b) { // funkcja liczy dystans Hamminga między odpowiednimi liczbami w dwóch tablicach 
            var arrays = a.Zip(b, (first, second) => new { a = first, b = second });
            foreach (var pair in arrays)
            {
                yield return int.PopCount(pair.a ^ pair.b); //PopCount zlicza ustawione bity maski uzyskanej przez działanie, a następnie zwraca wynik
            }
        }
    }
}