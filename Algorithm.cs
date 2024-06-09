using ReedSolomon;
namespace ConsoleApp1;

internal static class Program
{
    private static readonly RSPolynomial X = new RSPolynomial([1,0]);
    static void Main() //Berlekamp-Massey algorithm
    {
        List<int> alfa = [1,2,4,8,3,6,12,11,5,10,7,14,15,13,9,0];
        int[] syndromes = [alfa[6],alfa[9],alfa[7],alfa[3],alfa[6],alfa[4],1,alfa[3]]; //help data
        for (int i = 0; i < alfa.Count; i++)
        {
            Console.WriteLine($"Alfa^{i}: {alfa[i]}");
        }
        List<int> l = [0];

        int iterator = 0; //r
        List<RSPolynomial> lambdaList = [new RSPolynomial([1])];
        var b = new RSPolynomial([1]);
        iterator++;
        var t = Discrepancy(iterator, syndromes, lambdaList, 1);
        if (t.GetCoefficients[0] != 0)
        {
            lambdaList.Add(LfsrChange(lambdaList[0], t, b));
            l.Add(Math.Max(l[iterator - 1],iterator - l[iterator - 1]));
            b = Copy(lambdaList[iterator - 1]) / t;
        }
        while (iterator < syndromes.Length)
        {
            Console.WriteLine();
            iterator++;
            var previousIndex = iterator - 1;
            var d = DiscrepancyAltAlt(iterator, syndromes, lambdaList[previousIndex], l[previousIndex]);
            if (d.GetCoefficients[0] != 0)
            {
                lambdaList.Add(LfsrChange(lambdaList[previousIndex], d, b));
                l.Add(Math.Max(l[previousIndex],iterator - l[previousIndex]));
                var tempLam = Copy(lambdaList[previousIndex]);
                if (2 * l[previousIndex] <= previousIndex)
                {
                    b = tempLam / d;    
                }
                if (2 * l[previousIndex] > previousIndex)
                {
                    b *= X;
                }
            }
            if (d.GetCoefficients[0] ==  0)
            {
                lambdaList.Add(lambdaList[previousIndex]);
                l.Add(l[previousIndex]);
                b *= X;
            }
            Console.WriteLine($"lambda{iterator}: \t{lambdaList[iterator]}");
            Console.WriteLine($"B: \t\t{b}");
            Console.WriteLine($"Discrepancy: \t{d}");
            Console.WriteLine($"L{iterator}: \t\t{l[iterator]}");
        }
        Console.WriteLine("Hello World!");
        //Chien
        var errorPossitions = ChienSearch(lambdaList.Last());
        Console.WriteLine(errorPossitions.Aggregate("",((s, i) => s + i + " ")));
        // Forney 
        var twT = syndromes.Length;
        int [] syndromesreverse = new int[twT];
        for (int i = 0; i < syndromes.Length; i++)
        {
            syndromesreverse[i] = syndromes[^(i+1)];
        }
        var syndromesPoly = new RSPolynomial(syndromesreverse);
        var omegaMod = syndromesPoly * lambdaList.Last();
        omegaMod *= X;
        var omegaModCoefs = omegaMod.GetCoefficients;
        
        var omega = new int[omegaModCoefs.Count-twT];
        for (int i = 0; i < omega.Length; i++)
        {
            omega[i] = omegaModCoefs[i + twT];
        }
        // lambda derivative
        var lambdaCoeficients = lambdaList.Last().GetCoefficients;
        var lambdaDerivative = new int [lambdaCoeficients.Count - 1];
        for (int i = 1; i < twT/2; i++)
        {
            lambdaDerivative[i - 1] = lambdaCoeficients[^(i+1)];
        }

        var lambdaDerivativeReal = new int [lambdaDerivative.Length];
        for (int i = 0; i < lambdaDerivativeReal.Length; i++)
        {
            lambdaDerivativeReal[i] = lambdaDerivative[^(i+1)];
        }
        var lambdaDerivativePoly = new RSPolynomial(lambdaDerivativeReal);
        var omegaPoly = new RSPolynomial(omega);
        var temp = new int[15];
        for (int j = 0; j < errorPossitions.Count; j++)
        {
            temp[alfa.IndexOf(errorPossitions[j])-1] = alfa[15-alfa.IndexOf(errorPossitions[j])]*Copy(omegaPoly).Substitute(errorPossitions[j])/Copy(lambdaDerivativePoly).Substitute(errorPossitions[j]);
        }
        foreach (var VARIABLE in temp)
        {
            Console.Write(VARIABLE + ", ");
        }

        var code = new RSPolynomial([
            alfa[6], alfa[1], alfa[0], alfa[2], alfa[1], alfa[1], alfa[2], alfa[3], alfa[7], alfa[9], alfa[5], alfa[9],
            alfa[0], alfa[10]
        ]);
        var received = new RSPolynomial([
            alfa[11], alfa[1], alfa[0], alfa[2], alfa[1], alfa[5], alfa[2], alfa[3], alfa[7], alfa[9], alfa[10],
            alfa[9], alfa[0], alfa[10]
        ]);
        Console.WriteLine();
        received.Print();
        var errorPoly = new RSPolynomial(temp);
        var corrected = received ^ errorPoly;
        corrected.Print();
        code.Print();
    }

    static RSPolynomial Discrepancy(in int r, in int[] syndromes, in List<RSPolynomial> lambdaList, in int previousL)
    {
        var sum = new RSPolynomial([0]);
        for (var i = 0; i < previousL; i++)
        {
            sum ^=  new RSPolynomial([syndromes[r - i - 1]]) * lambdaList[r - 1];
        }
        return sum;
    } static RSPolynomial DiscrepancyAltAlt(in int r, in int[] syndromes, in RSPolynomial lastLambda, in int previousL)
    {
        var sum = new RSPolynomial([0]);
        for (var i = 1; i <= previousL; i++)
        {
            sum ^= new RSPolynomial([syndromes[r - i - 1]]) * new RSPolynomial([lastLambda.GetCoefficients[^(i+1)]]);
        }
        sum ^= new RSPolynomial([syndromes[r - 1]]);
        return sum;
    }

    static RSPolynomial LfsrChange(in RSPolynomial previousLambda, in RSPolynomial discrepancy, in RSPolynomial b)
    {
        var result = discrepancy * X * b ^ previousLambda;
        return result;
    } 
    static RSPolynomial Copy(RSPolynomial poly) => new RSPolynomial(poly.GetCoefficients);

    static List<int> ChienSearch(RSPolynomial locatorPolynomial)
    {
        List<int> rootIndexes = [];
        for (var i = 0; i < 15; i++)
        {
            var sub = Copy(locatorPolynomial).Substitute(i);
            if (sub == 0)
                rootIndexes.Add(i);
        }
        return rootIndexes;
    }
}