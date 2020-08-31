using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace loan_optimizer
{
    public class LoanOptimizer
    {
        private readonly ILoanAmortizer _loanAmortizer;
        private readonly ILogger<LoanOptimizer> _logger;

        private List<Loan> _loans;
        private float _availablePayment;

        public LoanOptimizer(
            ILoanAmortizer loanAmortizer,
            ILogger<LoanOptimizer> logger
            )
        {
            _loanAmortizer = loanAmortizer;
            _logger = logger;
        }

        public async Task<List<Loan>> OptimizeLoansAsync(List<Loan> loans, float availablePayment)
        {
            _loans = loans;
            _availablePayment = availablePayment;

            if (_loans.Count < 2)
            {
                throw new ArgumentException("A minimum of 2 loans are required for optimization!");
            }

            for (var iteration = 1; iteration < _loans.Count; iteration++)
            {
                _logger.LogDebug($"Starting iteration {iteration} of {_loans.Count - 1}");

                await ProcessIterationAsync();
            }

            _logger.LogDebug("Loan optimization complete!");

            return _loans;
        }

        private async Task ProcessIterationAsync()
        {
            for (var index = _loans.Count - 1; index > 0; index--)
            {
                _logger.LogDebug($"Comparing index {index - 1} to {index}");

                await CompareIndexAsync(index);
            }
        }

        private async Task CompareIndexAsync(int index)
        {
            var loans1 = ShallowCopy(_loans);
            var loans2 = ShallowCopy(_loans);
            loans2[index] = ShallowCopy(_loans[index - 1]);
            loans2[index - 1] = ShallowCopy(_loans[index]);

            var amortizedLoans1Task = _loanAmortizer.CalculateAmortization(loans1, _availablePayment);
            var amortizedLoans2Task = _loanAmortizer.CalculateAmortization(loans2, _availablePayment);

            var amortizedLoans1 = await amortizedLoans1Task;
            var amortizedLoans2 = await amortizedLoans2Task;

            var totalPayments1 = amortizedLoans1.Sum(loan => loan.TotalPayments);
            var totalPayments2 = amortizedLoans2.Sum(loan => loan.TotalPayments);

            _logger.LogDebug($"Total payments with configuration {index - 1}: {totalPayments1}");
            _logger.LogDebug($"Total payments with configuration {index}: {totalPayments2}");

            _loans = (List<Loan>)(totalPayments1 < totalPayments2 ? ShallowCopy(loans1) : ShallowCopy(loans2));
        }

        private T ShallowCopy<T>(T item)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(item));
        }
    }
}
