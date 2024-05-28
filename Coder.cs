using System.Text;

namespace ReedSolomon;

public class Coder
{
    public string _message;
    private readonly int [] _alfa = [1,2,4,8,16,5,10,20,13,26,17,7,14,28,29,31,27,19,3,6,12,24,21,15,30,25,23,11,22,9,18];
    private IEnumerable<string> _messageInBits;
    private RSPolynomial _gX = new ([1,2]);
    public RSPolynomial MessageCoded;
    
    public RSPolynomial GetGenerator => _gX;

    public Coder(string message) {
        RSgenpoly(4);
        Console.WriteLine($"Generator Polynomial: [ {_gX}] \n");
        _message = message;
        Console.WriteLine($"Message: {_message}");
        var bytes = Encoding.ASCII.GetBytes(message);
        var asciiString = bytes.Aggregate("", (current, value) => current + $"{value,5:b5}");
        Console.WriteLine($"Message in bits:\t {asciiString}");
        _messageInBits = StringToChunks(asciiString, 5);
        MessageCoded = BitsToPoly(_messageInBits) * _gX;
    }

    public Coder(int bitMessage) {
        RSgenpoly(4);
        Console.WriteLine($"Generator Polynomial: [ {_gX}] \n");
        Console.WriteLine($"Message: {bitMessage}");
        _message = Convert.ToString(bitMessage, 2);
        _messageInBits = StringToChunks(_message, 5);
        Console.WriteLine($"Message in bits:\t {_message}");
        MessageCoded = BitsToPoly(_messageInBits) * _gX;
    }

    protected Coder(){}

    public RSPolynomial BitsToPoly (IEnumerable<string> bitMessage) {
        List<int> bitsInDecimal = [];
        bitsInDecimal.AddRange(bitMessage.Select(value => Convert.ToInt32(value, 2)));
        return new RSPolynomial(bitsInDecimal);
    }
    private void RSgenpoly (int t) {
        for (var i = 1; i < 2*t; i++) {
            _gX *= new RSPolynomial ([1,_alfa[i+1]]);
        }
    }
    private static IEnumerable<string> StringToChunks(string str, int chunkSize) {
        for (int i = 0; i < str.Length; i+=chunkSize)
            yield return str.Substring(i, Math.Min(chunkSize, str.Length-i));
    }
}