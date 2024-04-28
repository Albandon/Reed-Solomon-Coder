namespace ReedSolomon
{
    public class RSPolynomial : Polynomial
    {
        private static readonly List<int> Alfa = new List<int>() {1,2,4,8,16,5,10,20,13,26,17,7,14,28,29,31,27,19,3,6,12,24,21,15,30,25,23,11,22,9,18};
        public RSPolynomial (IEnumerable<int> Coefs) {
            var Cof = Coefs.ToList(); 
            this.Coeficients = Cof;
            this.PolyDegree=Cof.Count-1;
        }
        public static RSPolynomial operator *(RSPolynomial a, RSPolynomial b) {
            int a_size = a.PolyDegree+1;
            int b_size = b.PolyDegree+1;
            int [] Product = new int [a_size+b_size-1];
            for (int i = 0; i < a_size; i++) {
                for (int j=0; j < b_size; j++) {
                    if (a.Coeficients[i]!=0 && b.Coeficients[j]!=0) {
                        Product[i+j] ^= Alfa[(Alfa.IndexOf(a.Coeficients[i])+Alfa.IndexOf(b.Coeficients[j]))%31];
                    }
                }
            }
            return new RSPolynomial(Product);
        }
        public static RSPolynomial operator /(RSPolynomial a, RSPolynomial b) {
            return new RSPolynomial(new int[]{1});
        } 
    }
}