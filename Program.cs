using System.Diagnostics;
namespace ReedSolomon // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main()
        {
            var test = new RSPolynomial ([7,1,2],3,4);
            test.Print();
            int [] alfa = [1,2,4,8,16,5,10,20,13,26,17,7,14,28,29,31,27,19,3,6,12,24,21,15,30,25,23,11,22,9,18];
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            var G_x = RSgenpoly (alfa,4);
            //sw.Stop();
            //Console.WriteLine("Elapsed={0}",sw.Elapsed);
            Console.WriteLine(G_x.ToString());
            var InCODED = new RSPolynomial ([1,2,10]);
            var CodedVAL = Coder(InCODED, G_x);
            Console.WriteLine(CodedVAL.ToString());
            (CodedVAL/G_x).Print();
            return;

        }
        public static RSPolynomial RSgenpoly (int[] alfa, int t) {
            var RSG_x = new RSPolynomial ([1,2]);
            for (var i = 1; i < 2*t; i++)
            {
                RSG_x *= new RSPolynomial ([1,alfa[i+1]]);
            }
            return RSG_x;
        }
        public static RSPolynomial Coder (RSPolynomial a, RSPolynomial genpoly) {
            var coded = a*genpoly;
            return coded;
        } 
    }
}