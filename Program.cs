namespace ReedSolomon
{
    internal static class Program
    {
        static void Main()
        {
            // var coder = new Coder(0b10000_00001_10001_00001);
            var coder = new Coder([10, 1, 15, 6, 4, 23, 6, 21, 1, 23, 31, 2, 4, 8, 20, 21, 18, 23, 9, 5, 10, 11, 1]);
            var received = ErrorSim(new RSPolynomial(coder.MessageCoded), 2);
            var decoder = new Decoder(received, coder.GetGenerator);
            decoder.Decode();
            Console.WriteLine(coder.MessageCoded == decoder.GetCorrectedMessage);
            const int nErrors = 1;
            const int nTests = 75_00;
            var correctedPolies = 0;
            for (int i = 0; i < nTests; i++)
            {
                received = ErrorSim(new RSPolynomial(coder.MessageCoded), nErrors);
                // received = BurstErrorSim(new RSPolynomial(coder.MessageCoded), 16);
                decoder = new Decoder(received, coder.GetGenerator);
                decoder.Decode();
                if (coder.MessageCoded == decoder.GetCorrectedMessage) correctedPolies++;
                // if (c != decoder.GetCorrectedMessage) Console.WriteLine(i);
            }

            Console.WriteLine($"Corrected for {nErrors} error positions {correctedPolies}/{nTests}");
            Console.WriteLine($"{(double)correctedPolies/nTests:P}");
            
            // test kodowania stringa
            coder = new Coder("slabo troche? "); //14 znaków powinno być niezawodnie dla większych -> dzielenie na pakiety pewnie, dla mniejszych -> uzupełnianie zerami (może)
            decoder = new Decoder(ErrorSim(new RSPolynomial(coder.MessageCoded),2), coder.GetGenerator);
            decoder.Decode();
            var decodedMessage = decoder.GetCorrectedMessage / coder.GetGenerator;
            Console.WriteLine(BitsToString(decodedMessage.PolyToBits()));
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
            // new RSPolynomial(errors).Print();
            return new RSPolynomial(errors) ^ message;
        }

        static int Pow2Sum(int Power)
        {
            var sum = 0; 
            for (var i = 0; i <= Power; i++)
            {
                sum += (int)Math.Pow(2, i);
            }
            return sum;
        }
        static RSPolynomial BurstErrorSim (RSPolynomial message, int nBits)
        {
            
            int messageSize = message.GetDegree + 1;
            int[] errors = new int [messageSize];
            var rand = new Random();
            var i = 0;
            var nMaxVal = nBits / 5;
            var remainder = nBits % 5; 
            var randIndex = rand.Next(0, messageSize - nMaxVal - 1);
            var endIndex = randIndex + nMaxVal;
            while (i < nMaxVal)
            {
                errors[randIndex + i] = 31;
                i++;
            } 
            i = rand.Next(5 - remainder);
            var alfaP = 31;
            var alfaK = 0;
            var pom = 16;
            var pom2 = (int)Math.Pow(2, 4 - remainder);
            alfaK = 31 - Pow2Sum(4 - remainder);
            while (i > 0)
            {
                alfaP -= pom;
                alfaK += pom2;
                pom /= 2;
                pom2 /= 2;
                i--;
            }
            // alfaK += remainder != 0 ? pom2 : 0;
            errors[randIndex] = alfaP;
            errors[endIndex] = alfaK;

            Console.WriteLine($"{new RSPolynomial(errors)}");
            return new RSPolynomial(errors) ^ message;
        }

        static string BitsToString(string bitString)
        {
            var bytes = StringToChunks(bitString, 8);
            // return bytes.Aggregate("", (current, x) => (current + $"{x}"));
            return bytes.Aggregate("", (current, x) => (current + $"{(char)Convert.ToInt32(x, 2)}"));

        }
        public static IEnumerable<string> StringToChunks(string str, int chunkSize) {  //funkcja dzieląca na segmenty po 5 znaków (w naszym przypadku po 5 bitów)
            for (int i = 0; i < str.Length; i+=chunkSize)
                yield return str.Substring(i, Math.Min(chunkSize, str.Length-i));
        }
    }
}