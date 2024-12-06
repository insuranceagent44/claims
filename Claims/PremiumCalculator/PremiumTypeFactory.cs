using Claims.Models;

namespace Claims.PremiumCalculator;

public interface IPremiumTypeFactory
{
    IPremiumType Create(CoverType coverType);
}

public class PremiumTypeFactory : IPremiumTypeFactory
{
    public IPremiumType Create(CoverType coverType)
    {
        return coverType switch
        {
            CoverType.Yacht => new YachtPremiumType(),
            CoverType.PassengerShip => new PassengerShipPremiumType(),
            CoverType.Tanker => new TankerPremiumType(),
            _ => new DefaultPremiumType(),
        };
    }
}