namespace ReedSolomon
{
    public class Polynomial
    {
        protected List<int> Coefficients = [];
        protected int PolyDegree;
        public Polynomial () {}
        public Polynomial (IEnumerable<int> coefficients) {
            var cof = coefficients.ToList(); 
            this.Coefficients = cof;
            this.PolyDegree=cof.Count-1;
        }
        public static Polynomial operator *(Polynomial a, Polynomial b){
            int aSize=a.PolyDegree+1, bSize=b.PolyDegree+1;
            int [] product = new int [aSize+bSize-1];
            for (int i = 0; i < aSize; i++) {
                for (int j = 0; j < bSize; j++) {
                    product[i+j] += a.Coefficients[i]*b.Coefficients[j]; 
                }
            }
            return new Polynomial(product);
        }
        public override string ToString()
        {
            return this.Coefficients.Aggregate("", (current, item) => current + item + " ");
        }
        public void Print () {
            Console.WriteLine(ToString()); 
        }

        public string PolyToBits() { //konwersja wartości w wielomianie na bity
            return Coefficients.Aggregate("", (current, value) => current + $"{value,5:b5}");
        }
        private static IEnumerable<string> StringToChunks(string str, int chunkSize) {  //funkcja dzieląca na segmenty po 5 znaków (w naszym przypadku po 5 bitów)
            for (int i = 0; i < str.Length; i+=chunkSize)
                yield return str.Substring(i, Math.Min(chunkSize, str.Length-i));
        }
    }
}