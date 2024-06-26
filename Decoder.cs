namespace ReedSolomon;

public class Decoder (RSPolynomial received, RSPolynomial gX)
{
    private RSPolynomial _correctedMessage = new([]);
    private static readonly RSPolynomial X = new([1, 0]);
    private static readonly List<int> Alfa = [1,2,4,8,16,5,10,20,13,26,17,7,14,28,29,31,27,19,3,6,12,24,21,15,30,25,23,11,22,9,18,0];

    public RSPolynomial GetCorrectedMessage => _correctedMessage;

    public void Decode() 
    {
        var syndromes = GetSyndromes(received);
        if (syndromes.Sum() == 0)
        {
            _correctedMessage = received;
            return;
        }
        var lambda = Berlekamp_Massey(syndromes);
        var errorPositionsNegation = ChienSearch(lambda);
        var errorPoly = Forney(syndromes, Copy(lambda), errorPositionsNegation);
        // Console.WriteLine($"errorPoly: \t\t{errorPoly}");
        var correctedMessage = Copy(received) ^ errorPoly;
        _correctedMessage = correctedMessage;
        // Console.WriteLine($"correctedMessage: \t{correctedMessage}");
    }
    private int[] GetSyndromes(RSPolynomial messageReceived)
    {
        int t2 = gX.GetDegree;
        var syndromes = new int [t2];
        for (int i = 1; i < t2 + 1; i++)
        {
            syndromes[i - 1] = Copy(messageReceived).Substitute(Alfa[i]);
        }
        return syndromes;
    }

    // TO:DO:
    // optymalizacja -> zamiana tablic na dwie tymczasowe zmienne || zmiana tablic na stos?
    // funkcja niezmieniona od poprzedniego razu
    private RSPolynomial Berlekamp_Massey(int[] syndromes) // posiada wyjątki gdy L nie pokrywa się ze stopniem lambdy
    {
        List<int> l = [0];

        int iterator = 0; //r
        List<RSPolynomial> lambdaList = [new RSPolynomial([1])];
        var b = new RSPolynomial([1]);
        while (iterator < syndromes.Length)
        {
            // Console.WriteLine();
            iterator++;
            var previousIndex = iterator - 1;
            var d = Discrepancy(iterator, syndromes, lambdaList[previousIndex], l[previousIndex]);
            if (d.GetCoefficients[0] != 0)
            {
                lambdaList.Add(LfsrChange(lambdaList[previousIndex], d, b));
                var tempLam = Copy(lambdaList[previousIndex]);
                if (2 * l[previousIndex] <= previousIndex)
                {
                    l.Add(iterator - l[previousIndex]);
                    b = tempLam / d;
                }
                else
                {
                    l.Add(l[previousIndex]);
                    b *= X;
                }
            }
            if (d.GetCoefficients[0] ==  0)
            {
                lambdaList.Add(lambdaList[previousIndex]);
                l.Add(l[previousIndex]);
                b *= X;
            }
            // if (lambdaList[iterator].GetDegree > 4) throw new Exception("error not correctable");
            // Console.WriteLine($"lambda{iterator}: \t{lambdaList[iterator]}");
            // Console.WriteLine($"B: \t\t{b}");
            // Console.WriteLine($"Discrepancy: \t{d}");
            // Console.WriteLine($"L{iterator}: \t\t{l[iterator]}");
        }
        return lambdaList.Last();
    }
    static RSPolynomial Discrepancy(in int r, in int[] syndromes, in RSPolynomial lastLambda, in int previousL)
    {
        var sum = new RSPolynomial([0]);
        var lambdaCoefficients = lastLambda.GetCoefficients;
        var maxLoop = previousL + 1 > lambdaCoefficients.Count ? lambdaCoefficients.Count : previousL + 1;
        // if (previousL >= lastLambda.GetCoefficients.Count) return sum;
        for (var i = 1; i < maxLoop; i++)
        {
            sum ^= new RSPolynomial([syndromes[r - i - 1]]) * new RSPolynomial([lastLambda.GetCoefficients[^(i+1)]]);
        }
        sum ^= new RSPolynomial([syndromes[r - 1]]);
        return sum;
    }

    private RSPolynomial LfsrChange(in RSPolynomial previousLambda, in RSPolynomial discrepancy, in RSPolynomial b)
    {
        var result = discrepancy * X * b ^ previousLambda;
        return result;
    }
    private List<int> ChienSearch(RSPolynomial locatorPolynomial) // zero brane pod uwagę?
    {
        List<int> rootIndexes = [];
        for (var i = 0; i < 32; i++)
        {
            var sub = Copy(locatorPolynomial).Substitute(i);
            if (sub == 0)
                rootIndexes.Add(i);
        }
        return rootIndexes;
    }
    private RSPolynomial Forney(int[] syndromes, RSPolynomial lambda, List<int> errorsPosNeg) // taaa...., działa; przydałby się cleanup
    {
        var t2 = syndromes.Length;
        Array.Reverse(syndromes);
        var syndromesPoly = new RSPolynomial(syndromes);
        // kreacja syndromu od x^0, nie zwiększamy potęgi sztucznie
        var omegaMod = syndromesPoly * lambda;
        var omegaModCoefs = omegaMod.GetCoefficients;
        var omega = new int[t2];
        for (int i = 0; i < omega.Length; i++) // mod x^2t
        {
            omega[i] = omegaModCoefs[i + (omegaModCoefs.Count - t2)];
        }
        var lambdaCoefficients = lambda.GetCoefficients;
        var lambdaDerivative = new int [lambdaCoefficients.Count - 1];
        var power = lambdaCoefficients.Count - 1;
        for (int i = 0; i < lambdaDerivative.Length; i++)   // Wzór na pochodną z innych źródeł. Działa sprawniej
        {
            if ((power - i) % 2 == 0) continue;
            lambdaDerivative[i] = lambdaCoefficients[i];
        }
        // for (int i = 0; i < lambdaDerivative.Length; i++)
        // {
        //     lambdaDerivative[i] = lambdaCoefficients[i];
        // }
        var lambdaDerivativePoly = new RSPolynomial(lambdaDerivative);
        var omegaPoly = new RSPolynomial(omega);
        var maxElement = 31; // Dodanie śledzenia max. elementu dla RS Poly
        var temp = new int[maxElement];
        foreach (var t in errorsPosNeg) // generator kreowany od alfy^1 więc X^1-b u nas X^1-1 znika, stąd taka forma
        {
            temp[((Alfa.IndexOf(t) - 1) % 31 + 31)%31] =
                Alfa[((Alfa.IndexOf(Copy(omegaPoly).Substitute(t)) -
                       Alfa.IndexOf(Copy(lambdaDerivativePoly).Substitute(t)))%31 + 31)%31];
            // if (Alfa.IndexOf(t) == 0)
            //     temp[0] = Copy(omegaPoly).Substitute(t) /
            //               Copy(lambdaDerivativePoly).Substitute(t);
            // else
            //     temp[Alfa.IndexOf(t) - 1] = Copy(omegaPoly).Substitute(t) /
            //                                 Copy(lambdaDerivativePoly).Substitute(t); 
            
            // if (Alfa.IndexOf(t) == 0)
            //     temp[0] = Alfa[((Alfa.IndexOf(Copy(omegaPoly).Substitute(t)) -
            //                      Alfa.IndexOf(Copy(lambdaDerivativePoly).Substitute(t)))%31 + 31)%31];
            // else
            //     temp[Alfa.IndexOf(t) - 1] =
            //         Alfa[((Alfa.IndexOf(Copy(omegaPoly).Substitute(t)) -
            //                Alfa.IndexOf(Copy(lambdaDerivativePoly).Substitute(t)))%31 + 31)%31];
            
            // if (Alfa.IndexOf(t) == 0)
            //     temp[0] = Alfa[maxElement - Alfa.IndexOf(t)] * Copy(omegaPoly).Substitute(t) /
            //               Copy(lambdaDerivativePoly).Substitute(t);
            // else
            //     temp[Alfa.IndexOf(t) - 1] = Alfa[maxElement - Alfa.IndexOf(t)] * Copy(omegaPoly).Substitute(t) /
            //                                 Copy(lambdaDerivativePoly).Substitute(t);
        }
        return new RSPolynomial(temp);
    }
    private static RSPolynomial Copy(RSPolynomial poly) => new RSPolynomial(poly.GetCoefficients); // nienawidzę, serio, ale musiałbym najpierw to naprawić w operatorach, a nie chce mi się
}