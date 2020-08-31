using System.Collections.Generic;

namespace loan_optimizer
{
    public static class LoanDefinitions
    {
        public static List<Loan> Loans =>
            new List<Loan> {
                new Loan
                {
                    OriginalBalance = 1500,
                    InterestRate = 0.04f
                },
                new Loan
                {
                    OriginalBalance = 2500,
                    InterestRate = 0.03f
                },
                new Loan
                {
                    OriginalBalance = 9000,
                    InterestRate = 0.035f
                },
                new Loan
                {
                    OriginalBalance = 6000,
                    InterestRate = 0.043f
                },
                new Loan
                {
                    OriginalBalance = 3000,
                    InterestRate = 0.027f
                },
                new Loan
                {
                    OriginalBalance = 2000,
                    InterestRate = 0.032f
                },
                new Loan
                {
                    OriginalBalance = 5000,
                    InterestRate = 0.035f
                },
                new Loan
                {
                    OriginalBalance = 1000,
                    InterestRate = 0.03f
                }
            };
    }
}
