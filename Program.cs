
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
        }
    }
}