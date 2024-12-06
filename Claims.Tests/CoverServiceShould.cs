using Claims.Models;
using Claims.PremiumCalculator;
using Claims.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using FluentAssertions;

namespace Claims.Tests;

public class CoverServiceShould
{
    [Theory]
    [InlineData("2023-01-01", "2023-01-30", CoverType.Yacht, 41250)] // 30 days, 30*1375
    [InlineData("2023-01-01", "2023-02-01", CoverType.Yacht, 43862.5)] // 32 days, 30*1375 + 2*1306.25 
    [InlineData("2023-01-01", "2023-05-31", CoverType.Yacht, 199306.25)] // 151 days, 30*1375 + 121*1306.25 
    [InlineData("2023-01-01", "2023-08-01", CoverType.Yacht, 278932.5)] // 243 days, 30*1375 + 150*1306.25 + 63*1265 
    [InlineData("2023-01-01", "2024-12-31", CoverType.Yacht, 471212.5)] // 365 days, 30*1375 + 150*1306.25 + 185*1265
    [InlineData("2023-01-01", "2023-01-30", CoverType.PassengerShip, 45000)] // 30 days, 30*1500
    [InlineData("2023-01-01", "2023-02-01", CoverType.PassengerShip, 47940)] // 32 days, 30*1500 + 2*1470 
    [InlineData("2023-01-01", "2023-05-31", CoverType.PassengerShip, 222870)] // 151 days, 30*1500 + 121*1470
    [InlineData("2023-01-01", "2023-08-01", CoverType.PassengerShip, 313515)] // 243 days, 30*1500 + 150*1470 + 63*1455 
    [InlineData("2023-01-01", "2024-12-31", CoverType.PassengerShip, 534675)] // 365 days, 30*1500 + 150*1470 + 185*1455 
    public void ComputeCorrectPremiumForVariousCoverTypes(string start, string end, CoverType coverType, decimal expectedPremium)
    {
        var service = new PremiumCalculator.PremiumCalculator(new PremiumTypeFactory());
        var startDate = DateTime.Parse(start);
        var endDate = DateTime.Parse(end);

        var result = service.Compute(startDate, endDate, coverType);
        
        result.Should().Be(expectedPremium);
    }
}