using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Data.Dbcontext;
using Wallet.Data.Entities;
using Wallet.Data.Interface;

namespace Wallet.Services.Services
{
    public class SystemUserRepository : ISystemuserRepo
    {
        private readonly ApplicationDbContext _context;

        public SystemUserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //Create User in the system 
        public SystemUser Create(SystemUser user)
        {

            _context.SystemUsers.Add(user);

            return user;

        }

        public TransactionHistory CreateTransactHist(TransactionHistory history)
        {
              _context.TransactionHistories.Add(history);

            return history;
        }


        public TransactionHistory getSingleTransactHist(int historyId)
        {
            var myTransactHist = _context.TransactionHistories.Where(x => x.Id == historyId).FirstOrDefault();

            return myTransactHist;
        }

        public IEnumerable<TransactionHistory> GetTransactionHistories(int walletId, getTransactHistResourceParameters histResourceParameters)
        {
            var collectionOfHistories = _context.TransactionHistories as IQueryable<TransactionHistory>; ///load as Iqueryable to keep querying before its sent to DB

            collectionOfHistories = collectionOfHistories.Where(x => x.wallet.ID == walletId);

            return collectionOfHistories
                .Skip(histResourceParameters.pageSize * (histResourceParameters.PageNumber - 1))
                .Take(histResourceParameters.pageSize)
                .ToList();

            //var histories = _context.TransactionHistories.Where(x => x.wallet.ID == walletId).ToList();

            //return histories;
        }

        public List<TransactionHistory> GetTransactionHistories_ByRef(string reference) 
        {
            var histories = _context.TransactionHistories
                .Include(wallet => wallet.wallet)
                .Where(x => x.reference == reference).ToList();

            return histories;
        }

        public WalletAccount CreateWallet(WalletAccount wallet)
        {
            _context.WalletAccounts.Add(wallet);

            return wallet;
        }

        public WalletAccount getMyWallet(int userId)
        {
            var myWallet = _context.WalletAccounts.Where(x => x.user.ID == userId).FirstOrDefault();

            return myWallet;
            //return null;
        }

        public WalletAccount getMyWallet_ByID(int walletId) 
        {
            var myWallet = _context.WalletAccounts.Where(x => x.ID == walletId).FirstOrDefault();

            return myWallet;
            //return null;
        }

        //get wallet by username 
        public WalletAccount getMyWalletByUsername(string walletName)
        {
            var myWallet = _context.WalletAccounts.Where(x => x.Name == walletName).FirstOrDefault();

            return myWallet;
            //return null;
        }


        public SystemUser getSingleSystemUser(int userId)
        {
            var systemUser =  _context.SystemUsers.Where(x => x.ID == userId).FirstOrDefault();

            return  systemUser;
        }


        public SystemUser getSingleSystemUser_Byusername(string username) 
        {
            var systemUser = _context.SystemUsers.Where(x => x.Username == username).FirstOrDefault();

            return systemUser;
        }

        //DB changes will only be done when this method is called ...
        public async Task<int> SaveChanges()
        {
            return await _context.SaveChangesAsync();
        }

        //
        public void Update(WalletAccount walletAccount)
        {
            var acct = _context.WalletAccounts.Find(walletAccount.ID);

            acct.Balance = walletAccount.Balance;
            acct.Name = walletAccount.Name;
            
        }

        
    }
}
