namespace ReedSolomon
{
    public class Polynomial
    {
        protected List<int> Coeficients = new List<int>();
        protected int PolyDegree = 0;
        public Polynomial () {}
        public Polynomial (IEnumerable<int> Coefs) {
            var Cof = Coefs.ToList(); 
            this.Coeficients = Cof;
            this.PolyDegree=Cof.Count-1;
        }
        public static Polynomial operator *(Polynomial a, Polynomial b){
            int a_size=a.PolyDegree+1, b_size=b.PolyDegree+1;
            int [] Product = new int [a_size+b_size-1];
            for (int i = 0; i < a_size; i++) {
                for (int j = 0; j < b_size; j++) {
                    Product[i+j] += a.Coeficients[i]*b.Coeficients[j]; 
                }
            }
            return new Polynomial(Product);
        }
        public override string ToString()
        {
            string OUT="";
            foreach (var item in this.Coeficients)
            {
                OUT+=item+" ";
            }
            return OUT;
        }
    }
}