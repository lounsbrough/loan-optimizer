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
                loan.UnpaidInterest = 0;
                loan.TotalPayments = 0;
                loan.TotalPrincipal = 0;
                loan.TotalInterest = 0;
                loan.MonthsToComplete = 0;
            }

            while (loans.Any(loan => loan.RemainingBalance > 0))
            {
                var remainingAvailable = availablePayment;

                foreach (var loan in loans.Where(loan => loan.RemainingBalance > 0))
                {
                    loan.MonthsToComplete++;

                    var interestAccrued = loan.RemainingBalance * loan.InterestRate / 12;
                    loan.UnpaidInterest += interestAccrued;
                    var payoffAmount = loan.RemainingBalance + loan.UnpaidInterest;

                    float principal;
                    float interest;

                    if (remainingAvailable < payoffAmount)
                    {
                        if (remainingAvailable < loan.UnpaidInterest)
                        {
                            interest = remainingAvailable;
                            principal = 0;
                            loan.UnpaidInterest -= remainingAvailable;
                        }
                        else
                        {
                            interest = Math.Min(remainingAvailable, loan.UnpaidInterest);
                            principal = remainingAvailable - loan.UnpaidInterest;
                            loan.RemainingBalance -= remainingAvailable - loan.UnpaidInterest;
                            loan.UnpaidInterest = 0;
                        }

                        remainingAvailable = 0;
                    }
                    else
                    {
                        remainingAvailable -= payoffAmount;
                        
                        interest = loan.UnpaidInterest;
                        principal = loan.RemainingBalance;

                        loan.UnpaidInterest = 0;
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
