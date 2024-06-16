

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
        public RSPolynomial(RSPolynomial a)
        {
            Coefficients = a.Coefficients;
            PolyDegree = a.PolyDegree;
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
        public static RSPolynomial operator ^(in RSPolynomial a, in RSPolynomial b)
        {
            var tempa = a;
            var tempb = b;
            if (a.PolyDegree > b.PolyDegree)
            {
                tempb = new RSPolynomial(b.Coefficients, b.PolyDegree+1, a.PolyDegree);
            }
            else if (a.PolyDegree < b.PolyDegree)
            {
                tempa = new RSPolynomial(a.Coefficients, a.PolyDegree+1, a.PolyDegree);
            }
            for (int i = 0; i <= a.PolyDegree; i++)
            {
                tempa.Coefficients[i] ^= tempb.Coefficients[i];
            }
            return tempa; 
        }
        public (RSPolynomial result, RSPolynomial remainder) Division (RSPolynomial a, RSPolynomial b)
        {
            RSPolynomial remainder;
            var aSize = a.PolyDegree+1;
            var bSize = b.PolyDegree+1;
            var maxElement = Alfa.Count;
            var productSize = a.Coefficients.Count < b.Coefficients.Count ? bSize - aSize + 1: aSize-bSize + 1;
            var product = new int [productSize];
            var currentA = new RSPolynomial(a);
            var currentB = new RSPolynomial(b);
            for (int i = 0; i < product.Length; i++)
            {
                var aVal = currentA.Coefficients[i];
                if (aVal == 0)
                {
                    product[i] = 0;
                    currentA ^= new RSPolynomial([0], product.Length-i,product.Length-1);
                    continue;
                }
                var degreeDiv = Alfa.IndexOf(aVal)-Alfa.IndexOf(currentB.Coefficients[0]);
                var index = degreeDiv < 0 ? (degreeDiv%(maxElement-1) + (maxElement-1))%(maxElement-1): degreeDiv%(maxElement-1);
                var divisor = Alfa[index];
                product[i] = divisor;
                remainder = currentB * new RSPolynomial ([divisor],product.Length-i,product.Length-1);     
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
            var currentA = new RSPolynomial(a);
            var currentB = new RSPolynomial(b);
            for (int i = 0; i < product.Length; i++)
            {
                var aVal = currentA.Coefficients[i];
                if (aVal == 0)
                {
                    product[i] = 0;
                    currentA ^= new RSPolynomial([0], product.Length-i,product.Length-1);
                    continue;
                }
                var degreeDiv = Alfa.IndexOf(aVal)-Alfa.IndexOf(currentB.Coefficients[0]);
                var index = degreeDiv < 0 ? (degreeDiv%(maxElement-1) + (maxElement-1))%(maxElement-1): degreeDiv%(maxElement-1);
                var divisor = Alfa[index];
                product[i] = divisor;
                var remainder = currentB * new RSPolynomial ([divisor],product.Length-i,product.Length-1);     
                currentA ^= remainder;
            }
            return new RSPolynomial(product);
        }

        public static bool operator ==(RSPolynomial a, RSPolynomial b)
        {
            return a.GetDegree == b.GetDegree && a.GetCoefficients.Zip(b.GetCoefficients).All(valueTuple => valueTuple.First == valueTuple.Second);
        }

        public static bool operator !=(RSPolynomial a, RSPolynomial b)
        {
            return !(a == b);
        }
        public int Substitute(int alfaVal)
        {
            var coefficients = Coefficients;
            var maxElement = Alfa.Count;
            int[] product;
            List<int> coefficientsS = [];
            for (int i = 0; i < PolyDegree; i++)
            {
                coefficientsS.Add(alfaVal);
            }
            coefficientsS.Add(0);
            for (int i = 0; i < coefficients.Count; i++)
            {
                if (coefficients[i] == 0) continue;
                var aVal = Alfa.IndexOf(coefficients[i]);
                var bVal = Alfa.IndexOf(coefficientsS[i]);
                
                coefficients[i] = Alfa[(aVal+bVal*(coefficients.Count-1-i))%(maxElement-1)];
            }
            var value = coefficients.Aggregate((x, y) => x ^ y);
            return value;
        }
        // public override string ToString()
        // {
        //     return this.Coefficients.Aggregate("", (current, item) => current + $"{Alfa.IndexOf(item):00}" + " ");
        // }
    }
}