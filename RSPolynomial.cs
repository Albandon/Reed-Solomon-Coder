using System.Reflection.Metadata;

namespace ReedSolomon
{
    public class RSPolynomial : Polynomial
    {
        //Lista jest szybsza
        private static readonly List<int> Alfa = [1,2,4,8,16,5,10,20,13,26,17,7,14,28,29,31,27,19,3,6,12,24,21,15,30,25,23,11,22,9,18,0]; //Lista alfa^i wartości, tak i==index
        public RSPolynomial (IEnumerable<int> Coefs) {      //Dodać wyjątek na konstrukcję z elementem > 31
            var Cof = Coefs.ToList(); 
            this.Coeficients = Cof;
            this.PolyDegree=Cof.Count-1;
        }
        public RSPolynomial (IEnumerable<int> Coefs,int Pos, int Degree) {
            int [] Cof = new int [Degree+1];
            var index = Degree-Pos+1;
            var i = 0;
            while (index < Coefs.Count()+Degree-Pos+1) {
                Cof[index] = Coefs.ElementAt(i);
                i++;
                index++;
            }
            this.Coeficients=Cof.ToList();
            this.PolyDegree=Degree;
        }
        public static RSPolynomial operator *(RSPolynomial a, RSPolynomial b) {
            var a_size = a.PolyDegree+1;
            int b_size = b.PolyDegree+1;
            var MaxElement = Alfa.Count;
            int [] Product = new int [a_size + b_size-1];
            for (int i = 0; i < a_size; i++) {                                                      //troche funny fucky-wucky, shuffluje potęgi(index) i jej wartości 
                var aVal = Alfa.IndexOf(a.Coeficients[i]);
                for (int j=0; j < b_size; j++) {
                    if (a.Coeficients[i]!=0 && b.Coeficients[j]!=0) {
                        Product[i+j] ^= Alfa[(aVal+Alfa.IndexOf(b.Coeficients[j]))%(MaxElement-1)];     //Szukam wartości dla alfy po "przemnożeniu", zapętlam potęgę do 30 (31 elementów) i xoruje

                    }
                }
            }
            return new RSPolynomial(Product);
        }
        public static RSPolynomial operator ^(RSPolynomial a, RSPolynomial b) {
            var Temp = a.PolyDegree>b.PolyDegree ? new RSPolynomial (b.Coeficients, b.PolyDegree,a.PolyDegree): b;
            for (int i = 0; i < a.PolyDegree+1; i++)
            {
                a.Coeficients[i]^=b.Coeficients[i];
            }
            return a;
        }
        public (RSPolynomial, RSPolynomial) Division (RSPolynomial a, RSPolynomial b) {
            var a_size = a.PolyDegree+1;
            var b_size = b.PolyDegree+1;
            var MaxElement = Alfa.Count;
            var Product = new int [a_size-b_size+1];
            var CurrentA = a;
            RSPolynomial Remainder = new([]); 
            for (int i = 0; i < Product.Length; i++)
            {
                var DegreeDiv = Alfa.IndexOf(CurrentA.Coeficients[i])-Alfa.IndexOf(b.Coeficients[0]);
                var Divisor = Alfa[DegreeDiv%(MaxElement-1)];
                Product[i] = Divisor;
                Remainder = b * new RSPolynomial ([Divisor],Product.Length-i,Product.Length-1);     
                CurrentA ^= Remainder;
            }
            Remainder = CurrentA;
            var Result = new RSPolynomial (Product);
            return (Result,Remainder);
        }
        public static RSPolynomial operator /(RSPolynomial a, RSPolynomial b) {
            var a_size = a.PolyDegree+1;
            var b_size = b.PolyDegree+1;
            var MaxElement = Alfa.Count;
            var Product = new int [a_size-b_size+1];
            var CurrentA = a;
            RSPolynomial Remainder = new([]); 
            for (int i = 0; i < Product.Length; i++)
            {
                var DegreeDiv = Alfa.IndexOf(CurrentA.Coeficients[i])-Alfa.IndexOf(b.Coeficients[0]);
                var Divisor = Alfa[DegreeDiv%(MaxElement-1)];
                Product[i] = Divisor;
                Remainder = b * new RSPolynomial ([Divisor],Product.Length-i,Product.Length-1);     
                CurrentA ^= Remainder;
            }
            Remainder = CurrentA;
            Remainder.Print();
            return new RSPolynomial(Product);
        }
    }
}