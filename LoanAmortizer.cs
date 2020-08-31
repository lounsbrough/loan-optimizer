using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace loan_optimizer
{
    public interface ILoanAmortizer
    {
        Task<List<Loan>> CalculateAmortization(List<Loan> loans, float availablePayment);
    }

    public class LoanAmortizer : ILoanAmortizer
    {
        private readonly ILogger<LoanAmortizer> _logger;

        public LoanAmortizer(
            ILogger<LoanAmortizer> logger
            )
        {
            _logger = logger;
        }

        public Task<List<Loan>> CalculateAmortization(List<Loan> loans, float availablePayment)
        {
            foreach (var loan in loans)
            {
                loan.RemainingBalance = loan.OriginalBalance;
                loan.TotalPayments = 0;
                loan.TotalPrincipal = 0;
                loan.TotalInterest = 0;
            }

            while (loans.Any(loan => loan.RemainingBalance > 0))
            {
                var remainingAvailable = availablePayment;

                foreach (var loan in loans)
                {
                    var interestAccrued = loan.RemainingBalance * loan.InterestRate / 12;
                    var payoffAmount = loan.RemainingBalance + interestAccrued;
                    var principal = 0f;
                    var interest = 0f;

                    if (remainingAvailable < payoffAmount)
                    {
                        loan.RemainingBalance -= remainingAvailable - interestAccrued;
                        
                        interest = Math.Min(remainingAvailable, interestAccrued);
                        principal = remainingAvailable - interest;

                        remainingAvailable = 0;
                    }
                    else
                    {
                        remainingAvailable -= payoffAmount;
                        
                        interest = interestAccrued;
                        principal = loan.RemainingBalance;

                        loan.RemainingBalance = 0;
                    }

                    loan.TotalPayments += principal + interest;
                    loan.TotalPrincipal += principal;
                    loan.TotalInterest += interest;
                }
            }

            return Task.FromResult(loans);
        }
    }
}
