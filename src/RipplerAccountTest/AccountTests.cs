using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RipplerAccountTest.AccountAggregate;
using RipplerES.CommandHandler;
using Shouldly;

namespace RipplerAccountTest
{
    [TestClass]
    public class AccountTests
    {
        readonly Dispatcher _dispatcher = new Dispatcher(new Bootstrapper().CreateServiceProvider());

        [TestMethod]
        public void Deposit()
        {
            var id = Guid.NewGuid();
            var result = _dispatcher.Execute(id, -1, new Deposit(amount: 10));
            result.ShouldBeOfType<CommandSuccessResult<Account>>();
        }


        [TestMethod]
        public void WithdrawWithNoMoney()
        {
            var id = Guid.NewGuid();
            var result = _dispatcher.Execute(id, -1, new Withdraw(amount: 10));

            result.ShouldBeOfType<CommandErrorResult<Account>>();
            var error = (CommandErrorResult<Account>) result;
            error.Error.ShouldBeOfType<InsufficientFunds>();
        }

        [TestMethod]
        public void WithdrawWithLotsOfMoney()
        {
            var id = Guid.NewGuid();
            _dispatcher.Execute(id, -1, new Deposit(amount: 10));
            _dispatcher.Execute(id,  1, new Deposit(amount: 20));
            _dispatcher.Execute(id,  2, new Deposit(amount: 30));
            _dispatcher.Execute(id,  3, new Deposit(amount: 60));
            _dispatcher.Execute(id,  4, new Deposit(amount: 120));

            var result = _dispatcher.Execute(id,  5, new Withdraw(amount:  80));

            result.ShouldBeOfType<CommandSuccessResult<Account>>();
        }


        [TestMethod]
        public void WithdrawWithNotEnoughMoney()
        {
            var id = Guid.NewGuid();
            _dispatcher.Execute(id, -1, new Deposit(amount: 10));
            _dispatcher.Execute(id, 1, new Deposit(amount: 20));
            _dispatcher.Execute(id, 2, new Deposit(amount: 30));
            _dispatcher.Execute(id, 3, new Deposit(amount: 60));

            var result = _dispatcher.Execute(id, 4, new Withdraw(amount: 800));

            result.ShouldBeOfType<CommandErrorResult<Account>>();
            var error = (CommandErrorResult<Account>)result;
            error.Error.ShouldBeOfType<InsufficientFunds>();
        }
    }
}
