namespace Vasil_Stoyanov_employees.Services
{
    using Models;

    public interface ICalculatorService
    {
        BestPairServiceModel GetTheBestPair(string fileName);
    }
}
