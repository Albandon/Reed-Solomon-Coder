namespace ReedSolomon // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main()
        {
            int [] alfa = new int [31] {1,2,4,8,16,5,10,20,13,26,17,7,14,28,29,31,27,19,3,6,12,24,21,15,30,25,23,11,22,9,18};

            var G_x = RSgenpoly (alfa,4);
            Console.WriteLine(G_x.ToString());
            return;

        }
        static public RSPolynomial RSgenpoly (int[] alfa, int t) {
            var RSG_x = new RSPolynomial (new int []{1,2});
            for (int i = 1; i < 2*t; i++)
            {
                RSG_x *= new RSPolynomial (new int []{1,alfa[i+1]});
            }
            return RSG_x;
        } 
    }
}