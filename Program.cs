
using System.Diagnostics;

namespace ReedSolomon
{
    internal static class Program
    {
        static void Main()
        {
            var k = new RSPolynomial([1, 4, 5, 1, 31]);
            k *= new RSPolynomial([0]);
            k.Print();
            var coder = new Coder(0b10000_00001_10001_00001);
            var c = new RSPolynomial([10,1,15,6,4,23,6,21,1,23,31,2,4,8,20,21,18,23,9,5,10,11,1]);
            var kl = new RSPolynomial(c);
            //c.Print();
            
            c *= coder.GetGenerator;
            Console.WriteLine($"Sent: \t\t\t{c}");
            // c /= coder.GetGenerator;
            // c.Print();
            
            //var codedMessage = coder.MessageCoded; //zapis liczby pomija zera na początku
            //codedMessage.Print();
            var received = ErrorSim(new RSPolynomial(c),1);
            Console.WriteLine($"Received: \t\t{received}");
            var decoder = new Decoder(received, coder.GetGenerator);
            decoder.Decode();
            Console.WriteLine(c==decoder.GetCorrectedMessage);
        }

        static RSPolynomial ErrorSim(RSPolynomial message, int nErrors)
        {
            int messageSize = message.GetDegree + 1;
            int[] errors = new int [messageSize];
            var rand = new Random();
            int i = 0;
            while (i < nErrors)
            {
                var randIndex = rand.Next(messageSize);
                if (errors[randIndex] != 0) continue;
                errors[randIndex] = rand.Next(0,32);
                i++;
            }
            return new RSPolynomial(errors) ^ message;
        }
    }
}