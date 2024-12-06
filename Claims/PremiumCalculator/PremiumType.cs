namespace Claims.PremiumCalculator;

public interface IPremiumType
{
    public decimal Multiplier => 1.3m;
    public decimal FactorForDay31To180 => (1.0m - 0.02m);
    public decimal FactorForDay181To365 => (1 - 0.03m);
}

public class DefaultPremiumType : IPremiumType
{
    
}

public class YachtPremiumType : IPremiumType
{
    public decimal Multiplier => 1.1M;
    public decimal FactorForDay31To180 => (1.0m - 0.05m);
    public decimal FactorForDay181To365 => (1 - 0.08m);
}

public class PassengerShipPremiumType : IPremiumType
{
    public decimal Multiplier => 1.2m;
}

public class TankerPremiumType : IPremiumType
{
    public decimal Multiplier => 1.5m;
}