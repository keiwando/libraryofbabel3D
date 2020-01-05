using ScottGarland;

namespace Keiwando.Lob {

  public static class Math {

    // MARK: - BitFlip

    public struct FlipParameters {
      internal int safeHighBits;
      internal int lowBits;
      internal int highBits;
      internal BigInteger lowMask;
      internal BigInteger invLowMask;
      internal BigInteger flipMask;
      internal BigInteger safeMask;

      public FlipParameters(int w, int safeHighBits, int lowBits) {
        this.safeHighBits = safeHighBits;
        this.lowBits = lowBits;
        this.highBits = w - lowBits - safeHighBits;
        this.lowMask = (new BigInteger(1) << lowBits) - 1;
        this.invLowMask = (new BigInteger(1) << highBits) - 1;
        this.flipMask = (new BigInteger(1) << (lowBits + highBits)) - 1;
        this.safeMask = ~flipMask;
      }
    }

    public static BigInteger FlipBits(BigInteger x, FlipParameters p) {
      var flipSegment = x & p.flipMask;
      var safeBits = x & p.safeMask;
      var highBits = flipSegment >> p.lowBits;
      var lowBits = p.lowMask & flipSegment;
      return (lowBits << p.highBits) + highBits + safeBits; 
    }

    public static BigInteger InverseFlipBits(BigInteger x, FlipParameters p) {
      var flipSegment = x & p.flipMask;
      var safeBits = x & p.safeMask;
      var highBits = flipSegment >> p.highBits;
      var lowBits = p.invLowMask & flipSegment;
  		return (lowBits << p.lowBits) + highBits + safeBits;
    }

    // MARK: - Linear Congruential Generator

    public struct LCGParameters {
      public BigInteger m;
      public BigInteger a;
      public BigInteger c;
      public BigInteger aInverse;
    }
    
    public static BigInteger LCG(BigInteger x, LCGParameters p) {
	  	return BigInteger.ActualModulus(p.a * x + p.c, p.m);
	  }

    public static BigInteger InverseLCG(BigInteger x, LCGParameters p) {
      return BigInteger.ActualModulus(p.aInverse * (x - p.c), p.m);
    }

    // MARK: - Euclid's Extended Algorithm

    public static EuclidExtendedSolution extendedGcd(BigInteger a, BigInteger b) {

      BigInteger x0 = 1, xn = 1;
      BigInteger y0 = 0, yn = 0;
      BigInteger x1 = 0;
      BigInteger y1 = 1;
      BigInteger q;
      BigInteger r = a % b;
      
      while (r > 0)
      {
        q = a / b;
        xn = x0 - q * x1;
        yn = y0 - q * y1;
        
        x0 = x1;
        y0 = y1;
        x1 = xn;
        y1 = yn;
        a = b;
        b = r;
        r = a % b;
      }
      
      return new EuclidExtendedSolution(xn, yn, b);
    }

    public struct EuclidExtendedSolution {
      public readonly BigInteger x;
      public readonly BigInteger y;
      public readonly BigInteger d;
      public EuclidExtendedSolution(BigInteger x, BigInteger y, BigInteger d) {
        this.x = x;
        this.y = y;
        this.d = d;
      }
    }
  }
}