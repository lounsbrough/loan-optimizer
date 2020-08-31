namespace loan_optimizer
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    class Program
    {
        public static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILogger<Program>>();

            const float availablePayment = 200;

            var loanOptimizer = serviceProvider.GetService<LoanOptimizer>();
            var loanAmortizer = serviceProvider.GetService<ILoanAmortizer>();

            var optimizedLoans = Task.Run(() => loanOptimizer.OptimizeLoansAsync(LoanDefinitions.Loans, availablePayment)).Result;

            var deoptimizedLoans = loanOptimizer.ShallowCopy(optimizedLoans);
            deoptimizedLoans.Reverse();

            deoptimizedLoans = Task.Run(() => loanAmortizer.CalculateAmortization(deoptimizedLoans, availablePayment)).Result;

            logger.LogInformation(JsonConvert.SerializeObject(optimizedLoans, Formatting.Indented));
            logger.LogInformation($"Total payments after optimization: {optimizedLoans.Sum(loan => loan.TotalPayments)}");
            logger.LogInformation($"Total payments in the worst configuration: {deoptimizedLoans.Sum(loan => loan.TotalPayments)}");
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ILoggerFactory, LoggerFactory>();
            serviceCollection.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            serviceCollection.AddLogging(loggingBuilder => loggingBuilder
                .AddConsole()
                .SetMinimumLevel(LogLevel.Trace));

            serviceCollection.AddTransient<LoanOptimizer>();
            serviceCollection.AddTransient<ILoanAmortizer, LoanAmortizer>();
        }
    }
}
