using Claims.Models;
using Claims.Validators;
using Xunit;
using FluentAssertions;
using MongoDB.Driver.Linq;

namespace Claims.Tests;

public class CoverValidatorShould
{
    public CoverValidator Setup()
    {
        var validator = new CoverValidator();
        return validator;
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(30)]
    [InlineData(31)]
    [InlineData(400)]
    public async void ReturnTrueIfTheStartDateIsInTheFuture(int days)
    {
        var validator = Setup();
        
        var cover = new Cover
        {
            StartDate = DateTime.Now.AddDays(days),
            EndDate = DateTime.Now.AddDays(days).AddDays(10),
        };
        
        var validationResult = await validator.ValidateAsync(cover);
        
        validationResult.IsValid.Should().BeTrue();
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(-30)]
    [InlineData(-31)]
    [InlineData(-400)]
    public async void ReturnFalseIfTheStartDateIsInThePast(int days)
    {
        var validator = Setup();
        
        var cover = new Cover
        {
            StartDate = DateTime.Now.AddDays(days),
            EndDate = DateTime.Now.AddDays(days).AddDays(10),
        };
        
        var validationResult = await validator.ValidateAsync(cover);
        
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Select(e => e.ErrorMessage).ToList()
            .Should().Contain("Start date cannot be in the past.");
    }
    
    [Theory]
    [InlineData("2026-01-01", "2027-01-02")]
    [InlineData("2026-01-01", "2027-06-18")]
    public async void ValidateReturnFalseIfInsurancePeriodExceed1Year(string startDate, string endDate)
    {
        var validator = Setup();
        
        var cover = new Cover
        {
            StartDate = DateTime.Parse(startDate),
            EndDate = DateTime.Parse(endDate),
        };
        
        var validationResult = await validator.ValidateAsync(cover);
        
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Select(e => e.ErrorMessage).ToList()
            .Should().Contain("Total insurance period cannot exceed 1 year.");
    }
    
    [Theory]
    [InlineData("2026-01-01", "2026-04-01")]
    [InlineData("2026-01-01", "2026-01-29")]
    public async void ValidateReturnTrueIfInsurancePeriodIsLessThen1Year(string startDate, string endDate)
    {
        var validator = Setup();
        
        var cover = new Cover
        {
            StartDate = DateTime.Parse(startDate),
            EndDate = DateTime.Parse(endDate),
        };
        
        var validationResult = await validator.ValidateAsync(cover);
        
        validationResult.IsValid.Should().BeTrue();
    }
}