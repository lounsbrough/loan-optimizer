namespace loan_optimizer
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    class Program
    {
        public static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            const float availablePayment = 200;

            var loanOptimizer = serviceProvider.GetService<LoanOptimizer>();

            var optimizedLoans = Task.Run(() => loanOptimizer.OptimizeLoansAsync(LoanDefinitions.Loans, availablePayment)).Result;

            var logger = serviceProvider.GetService<ILogger<Program>>();

            logger.LogInformation(JsonConvert.SerializeObject(optimizedLoans, Formatting.Indented));

            logger.LogInformation($"Total Payments: {optimizedLoans.Sum(loan => loan.TotalPayments)}");
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
