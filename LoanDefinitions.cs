using System.Collections.Generic;

namespace loan_optimizer
{
    public static class LoanDefinitions
    {
        public static List<Loan> Loans =>
            new List<Loan> {
                new Loan
                {
                    OriginalBalance = 1486.37f,
                    InterestRate = 0.05f
                },
                new Loan
                {
                    OriginalBalance = 1000,
                    InterestRate = 0.15f
                },
                new Loan
                {
                    OriginalBalance = 10000,
                    InterestRate = 0.04f
                },
                new Loan
                {
                    OriginalBalance = 1000,
                    InterestRate = 0.1f
                },
                new Loan
                {
                    OriginalBalance = 1486.37f,
                    InterestRate = 0.05f
                },
                new Loan
                {
                    OriginalBalance = 100000,
                    InterestRate = 0.14f
                },
                new Loan
                {
                    OriginalBalance = 10000,
                    InterestRate = 0.04f
                },
                new Loan
                {
                    OriginalBalance = 1000,
                    InterestRate = 0.1f
                }
            };
    }
}
