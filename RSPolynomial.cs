

namespace ReedSolomon
{
    public class RSPolynomial : Polynomial
    {
        //Lista jest szybsza
        private static readonly List<int> Alfa = [1,2,4,8,16,5,10,20,13,26,17,7,14,28,29,31,27,19,3,6,12,24,21,15,30,25,23,11,22,9,18,0]; //Lista alfa^i wartości, tak i==index
        public RSPolynomial (IEnumerable<int> coefficients) {      //Dodać wyjątek na konstrukcję z elementem > 31
            var cof = coefficients.ToList(); 
            this.Coefficients = cof;
            this.PolyDegree=cof.Count - 1;
        }
        public RSPolynomial (IEnumerable<int> coefficients,int pos, int degree) {
            int [] cof = new int [degree + 1];
            var index = degree - pos + 1;
            var i = 0;
            var coefs = coefficients.ToArray();
            while (index < coefs.Length + degree-pos + 1 ) {
                cof[index] = coefs.ElementAt(i);
                index++;
                i++;
            }
            this.Coefficients=cof.ToList();
            this.PolyDegree=degree;
        }
        public List<int> GetCoefficients => Coefficients;
        public int GetDegree => PolyDegree;
        
        public static RSPolynomial operator *(RSPolynomial a, RSPolynomial b) {
            var aSize = a.PolyDegree+1;
            int bSize = b.PolyDegree+1;
            var maxElement = Alfa.Count;
            int [] product = new int [aSize + bSize-1];
            for (int i = 0; i < aSize; i++) {                                                      //trochę funny fucky-wucky, shuffleuje potęgi(index) i jej wartości 
                var aVal = Alfa.IndexOf(a.Coefficients[i]);
                for (int j=0; j < bSize; j++) {
                    if (a.Coefficients[i]!=0 && b.Coefficients[j]!=0) {
                        product[i+j] ^= Alfa[(aVal+Alfa.IndexOf(b.Coefficients[j]))%(maxElement-1)];     //Szukam wartości dla alfy po "przemnożeniu", zapętlam potęgę do 30 (31 elementów) i wykonuję operacje xor

                    }
                }
            }
            return new RSPolynomial(product);
        }
        public static RSPolynomial operator ^(RSPolynomial a, RSPolynomial b) {
            if (a.PolyDegree > b.PolyDegree)
            {
                b = new RSPolynomial(b.Coefficients, b.PolyDegree, a.PolyDegree);
            }
            else if (a.PolyDegree < b.PolyDegree)
            {
                a = new RSPolynomial(a.Coefficients, a.PolyDegree, a.PolyDegree);
            }
            for (int i = 0; i <= a.PolyDegree; i++)
            {
                a.Coefficients[i] ^= b.Coefficients[i];
            }
            return a;
        }
        public static (RSPolynomial, RSPolynomial) Division (RSPolynomial a, RSPolynomial b) {
            var aSize = a.PolyDegree+1;
            var bSize = b.PolyDegree+1;
            var maxElement = Alfa.Count;
            var product = new int [aSize-bSize+1];
            var currentA = a;
            RSPolynomial remainder; 
            for (int i = 0; i < product.Length; i++)
            {
                var degreeDiv = Alfa.IndexOf(currentA.Coefficients[i])-Alfa.IndexOf(b.Coefficients[0]);
                var divisor = Alfa[degreeDiv%(maxElement-1)];
                product[i] = divisor;
                remainder = b * new RSPolynomial ([divisor],product.Length-i,product.Length-1);     
                currentA ^= remainder;
            }
            remainder = currentA;
            var result = new RSPolynomial (product);
            return (result,remainder);
        }
        public static RSPolynomial operator /(RSPolynomial a, RSPolynomial b) {
            var aSize = a.PolyDegree+1;
            var bSize = b.PolyDegree+1;
            var maxElement = Alfa.Count;
            var productSize = a.Coefficients.Count < b.Coefficients.Count ? bSize - aSize + 1: aSize-bSize + 1;
            var product = new int [productSize];
            var currentA = a;
            for (int i = 0; i < product.Length; i++)
            {
                var degreeDiv = Alfa.IndexOf(currentA.Coefficients[i])-Alfa.IndexOf(b.Coefficients[0]);
                var divisor = Alfa[degreeDiv%(maxElement-1)];
                product[i] = divisor;
                var remainder = b * new RSPolynomial ([divisor],product.Length-i,product.Length-1);     
                currentA ^= remainder;
            }
            return new RSPolynomial(product);
        }
    }
}