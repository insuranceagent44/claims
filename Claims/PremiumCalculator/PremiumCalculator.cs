using Claims.Models;

namespace Claims.PremiumCalculator;

public interface IPremiumCalculator
{
    public decimal Compute(DateTime startDate, DateTime endDate, CoverType coverType);
}

public class PremiumCalculator : IPremiumCalculator
{
    private readonly IPremiumTypeFactory _factory;

    public PremiumCalculator(IPremiumTypeFactory factory)
    {
        _factory = factory;
    }

    public decimal Compute(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        IPremiumType premiumType = _factory.Create(coverType);
        
        var basePremiumPerDay = 1250;
        var premiumPerDay = basePremiumPerDay * premiumType.Multiplier;
        var insuranceLength = (endDate - startDate).TotalDays + 1;

        var totalPremium = Enumerable.Range(1, (int) insuranceLength)
            .Sum(dayNumber =>
            {
                return dayNumber switch
                {
                    <= 30 => premiumPerDay,
                    <= 180 => premiumPerDay * premiumType.FactorForDay31To180,
                    <= 365 => premiumPerDay * premiumType.FactorForDay181To365,
                    _ => 0
                };
            });

        return totalPremium;
    }
}