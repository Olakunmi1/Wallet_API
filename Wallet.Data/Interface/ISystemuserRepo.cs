using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wallet.Data.Entities;

namespace Wallet.Data.Interface
{
   public interface ISystemuserRepo
    {
        SystemUser Create(SystemUser user);
        SystemUser getSingleSystemUser(int userId);
        SystemUser getSingleSystemUser_Byusername(string username);
        WalletAccount CreateWallet(WalletAccount wallet);
        WalletAccount getMyWallet(int walletId);
        WalletAccount getMyWallet_ByID(int walletId); 
        void Update(WalletAccount walletAccount);

        TransactionHistory CreateTransactHist(TransactionHistory history);
        TransactionHistory getSingleTransactHist(int historyId);
        IEnumerable<TransactionHistory> GetTransactionHistories(int walletId, getTransactHistResourceParameters histResourceParameters);
        List<TransactionHistory> GetTransactionHistories_ByRef(string reference);
        WalletAccount getMyWalletByUsername(string walletName);
        Task<int> SaveChanges();

    }
}
