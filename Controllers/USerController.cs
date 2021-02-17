using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Wallet.Data.Entities;
using Wallet.Data.Interface;
using Wallet_API.ReadDTO;
using Wallet_API.WriteDTO;

namespace Wallet_API.Controllers
{
    [ApiController]
    [Route("api/User")]
    public class USerController : ControllerBase
    {
        private readonly ISystemuserRepo _systemuser;
        private readonly UserManager<ApplicationUser> _userManager;

        public USerController(ISystemuserRepo systemuser, UserManager<ApplicationUser> userManager)
        {
            _systemuser = systemuser;
            _userManager = userManager;
        }


        [HttpPost("CreateWallet/{userId}")]
        public async Task<IActionResult> CreateWallet(int userId, [FromBody] WalletAccountDTOW model)
        {
            try
            {
                var usernamee = User.Identity.Name;
                StringBuilder strbld2 = new StringBuilder();
                var err2 = new List<string>();
                if (!ModelState.IsValid)
                {
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            err2.Add(error.ErrorMessage);
                            err2.ForEach(err => { strbld2.AppendFormat("•{0}", error.ErrorMessage); });
                        }
                    }

                    return BadRequest(new { message = strbld2 });
                }

                //check if system user exist 
                var userss = _systemuser.getSingleSystemUser(userId);
                if (userss == null)
                {
                    return NotFound(new
                    {
                        succes = false,
                        message = "User Not Found"
                    });
                }

                //To:do -- Check if this user has exisitng wallet  ---
                var walletAcct = _systemuser.getMyWallet(userId);
                if (walletAcct != null)
                {
                    return Ok(new
                    {
                        succes = false,
                        message = "You already have a Wallet Account"
                    });
                }

                decimal bal = 0.0000m; // explicit cast to decimal with the prefix "m", float is "f"

                //create wallet function
                var walletResponse = CreateAWallet(userId, model.Name);

                if (await walletResponse == false)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Something went wrong we could not create your wallet Pls try again"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Wallet Created Succesfully",
                    WalletName = model.Name,
                    Current_Balance = bal
                });
            }

            catch(Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    message = "Something went wrong we could not create your wallet Pls try again"
                });
            }
           
        }

        //systemt user id needed
        [HttpGet("getMyWallet/{Id}")]
        public IActionResult getMyWallet(int Id)
        {
            try
            {
                //check if system user exist 
                var userss = _systemuser.getSingleSystemUser(Id);
                if (userss == null)
                {
                    return NotFound(new
                    {
                        succes = false,
                        message = "User Not Found"
                    });
                }
                var walletAcct = _systemuser.getMyWallet(Id);
                if (walletAcct == null)
                {
                    return NotFound(new
                    {
                        succes = false,
                        message = "You dont have a Wallet Account yet, pls create one"
                    });
                }

                return Ok(new
                {
                    success = true,
                    WalletName = walletAcct.Name,
                    Current_Balance = walletAcct.Balance
                });

            }

            catch (Exception ex)
            {
                return Ok(new
                {
                    succes = false,
                    message = ex.Message
                });
            }

        }

        [HttpPut("UpdateWalletInfo/{id}")]
        public async Task<IActionResult> UpdatewalletInfo(int id, [FromBody]UpdateWalletModel model)
        {
            //check if system user exist 
            var userss = _systemuser.getSingleSystemUser(id);
            if (userss == null)
            {
                return NotFound(new
                {
                    succes = false,
                    message = "User Not Found"
                });
            }
            //get singlewallet 
            var walletAcct = _systemuser.getMyWallet(id);
            if (walletAcct == null)
            {
                return Ok(new
                {
                    succes = false,
                    message = "You dont have a Wallet Account"
                });
            }

            //entitycore tracks this chnage and when save chaneg is called its saved
            walletAcct.Name = model.WalletName;

            try
            {
                // update user 
                // _systemuser.Update(updatedWalet);
                await _systemuser.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return Ok(new { success = false, message = ex.Message });
            }
        }


        //systemt user id needed
        [HttpPost("FundMyWallet/{Id}")]
        public async Task<IActionResult> Fundwallet(int Id, FundWalletModel model)
        {
            try
            {
                //check if user exist within the system 
                var userss = _systemuser.getSingleSystemUser(Id);
                if (userss == null)
                {
                    //bounce back if they dont exist 
                    return NotFound(new
                    {
                        succes = false,
                        message = "User Not Found"
                    });
                }

                //grabb usesr wallet
                var walletAcct = _systemuser.getMyWallet(Id);
                if (walletAcct == null)
                {
                    //bounce back if they dont have a wallet yet 
                    return NotFound(new
                    {
                        succes = false,
                        message = "You dont have a Wallet Account, Pls create one"
                    });
                }

                var refff = Guid.NewGuid().ToString(); //ref for this transaction
                decimal bal = 0.0000m;   // explicit cast to decimal

                bal = model.Amount; //new amount to fund with 
                var balanceBefore = walletAcct.Balance;
                var newbalance = (balanceBefore + bal);
                string purpose = "Deposit";
                //--call Credit function 
                var creditResponse = await Credit(walletAcct, balanceBefore, newbalance, refff, purpose, bal);

                if(!creditResponse == true)
                {
                    return Ok(new { success = false, messgae = "Something went wrong pls try again later" });
                }

                //then save changes
                await _systemuser.SaveChanges();

                //return transaction summary to user --this way 
                return Ok(new
                {
                    success = creditResponse,
                    type = "Credit",
                    message = "You have Sucesfully fund your wallet",
                    balance_before = balanceBefore,
                    balance_after = newbalance,
                    Beneficiary = "Self",
                    narration = model.Narration
                });

            }

            catch (Exception ex)
            {
                return Ok(new { succes = false, messsage = ex.Message });
            }
        }

        //systemt user id needed
        [HttpPost("TransferFund/{Id}")]
        public async Task<IActionResult> TransferFund(int Id, TransferFundDTOW model)
        {
            try
            {
                var refff = Guid.NewGuid().ToString(); //ref for this transaction

                //check if user exist 
                var userss = _systemuser.getSingleSystemUser(Id);
                if (userss == null)
                {
                    //bounce back if they dont exist 
                    return NotFound(new
                    {
                        succes = false,
                        message = "User Not Found"
                    });
                }
                //check if wallet exist
                //grabb Sender wallet
                var walletAcct1 = _systemuser.getMyWallet(Id);
                if (walletAcct1 == null)
                {
                    //bounce back if they dont have a wallet yet 
                    return NotFound(new
                    {
                        succes = false,
                        message = "You dont have a Wallet Account, Pls create one"
                    });
                }

                //grabb Receiver wallet
                var walletAcct2 = _systemuser.getMyWalletByUsername(model.WalletName);
                if (walletAcct2 == null)
                {
                    //bounce back if they receiver have a wallet yet 
                    return NotFound(new
                    {
                        succes = false,
                        message = "Wallet Account not found. The Receiever doesn't have a Wallet, Pls check with the Receiver"
                    });
                }

                //grab sender details and receiver details
                var currentBalance = walletAcct1.Balance;
                var amountToSend = model.Amount;
                if (amountToSend > currentBalance)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Insufficient balance"
                    });

                }

                //process transaction for sender 
                decimal bal = 0.0000m;   // explicit cast to decimal
                bal = model.Amount; //amount user intend to send 

                var balancebefore_Sender = currentBalance;
                var newBalance = (balancebefore_Sender) - (bal);
                string purpose = "Transfer";

                //-- Call Debit function
              
                var DebitResponse = await Debit(walletAcct1, balancebefore_Sender, newBalance, refff, purpose, amountToSend);

                if (!DebitResponse == true)
                {
                    return Ok(new { success = false, messgae = "Something went wrong pls try again later" });
                }

                //process transaction for Receiever 
                var balancebefore_Receiever = walletAcct2.Balance;
                var balanceAfter = (balancebefore_Receiever) + (bal);
                string purposee ="Deposit";

                // call Credit function 
                var CreditResponse = await Credit(walletAcct2, balancebefore_Receiever, balanceAfter, refff, purposee, amountToSend);

                if (!CreditResponse == true)
                {
                    return Ok(new { success = false, messgae = "Something went wrong pls try again later" });
                }

                //call save changes
                await _systemuser.SaveChanges();

                //retun response body --this way
                return Ok(new
                {
                    success = true,
                    type = "Debit",
                    message = "Transfer Succesful",
                    balance_before = balancebefore_Sender,
                    balance_after = newBalance,
                    Beneficiary = walletAcct2.Name,
                    narration = model.Narration
                });

            }
            catch (Exception ex)
            {
                return Ok(new { succes = false, messsage = ex.Message });
            }

        }

        //systemt user id needed
        [HttpGet("getMyTransactionhistory/{Id}")]
        public IActionResult getMyTransactionHistory(int Id, [FromQuery] getTransactHistResourceParameters getTransact) 
        {
            try
            {
                //check if system user exist 
                var userss = _systemuser.getSingleSystemUser(Id);
                if (userss == null)
                {
                    return NotFound(new
                    {
                        succes = false,
                        message = "User Not Found"
                    });
                }
                var walletAcct = _systemuser.getMyWallet(Id);
                if (walletAcct == null)
                {
                    return NotFound(new
                    {
                        succes = false,
                        message = "You dont have a Wallet Account yet, pls create one"
                    });
                }

                try
                {
                    var getTransactHistory = _systemuser.GetTransactionHistories(walletAcct.ID, getTransact);

                    var getTransactHistory_ReadDto = getTransactHistory
                        .Select(x => new TransactHistDTO
                        {
                            Txn_type = x.Txn_type,
                            balance_before = x.balance_before,
                            balance_after = x.balance_after,
                            Purpose = x.Purpose,
                            created_at = x.created_at,
                            updated_at = x.updated_at,
                            WalletId = x.wallet.ID,
                            Amount = x.Amount
                        });

                    return Ok(new
                    {
                        success = true,
                        MyTransactionHistories = getTransactHistory_ReadDto
                    });
                }
                catch (Exception ex)
                {
                    return Ok(new { succes = false, message = ex.Message });
                }
            }

            catch (Exception ex)
            {
                return Ok(new
                {
                    succes = false,
                    message = ex.Message
                });
            }

        }



        //systemt user id needed
        [HttpPost("WithdrawFund/{Id}")]
        public async Task<IActionResult> Withdraw(int Id, WithdrawFundDTOW model)
        {
            try
            {
                var refff = Guid.NewGuid().ToString(); //ref for this transaction

                //check if user exist 
                var userss = _systemuser.getSingleSystemUser(Id);
                if (userss == null)
                {
                    //bounce back if they dont exist 
                    return NotFound(new
                    {
                        succes = false,
                        message = "User Not Found"
                    });
                }
                //check if wallet exist
                //grabb Sender wallet
                var walletAcct = _systemuser.getMyWallet(Id);
                if (walletAcct == null)
                {
                    //bounce back if they dont have a wallet yet 
                    return NotFound(new
                    {
                        succes = false,
                        message = "You dont have a Wallet Account, Pls create one"
                    });
                }

                //grab sender balance details
                var currentBalance = walletAcct.Balance;
                var amountToWithdraw = model.Amount; 
                if (amountToWithdraw > currentBalance)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Insufficient balance"
                    });

                }

                //process transaction for USer 
                decimal bal = 0.0000m;   // explicit cast to decimal
                bal = model.Amount; //amount user intend to withdraw 

                var balancebefore = currentBalance; 
                var newBalance = (balancebefore) - (bal);
                string purpose = "Withdraw";

                //-- Call Debit function
                var DebitResponse = await Debit(walletAcct, balancebefore, newBalance, refff, purpose, amountToWithdraw);

                if (!DebitResponse == true)
                {
                    return Ok(new { success = false, messgae = "Something went wrong pls try again later" });
                }

                //call save changes
                await _systemuser.SaveChanges();

                //retun response body --this way
                return Ok(new
                {
                    success = true,
                    type = "Debit",
                    message = "Withdraw Succesful",
                    balance_before = balancebefore,
                    balance_after = newBalance,
                    Beneficiary = "Self",
                });

            }
            catch (Exception ex)
            {
                return Ok(new { succes = false, messsage = ex.Message });
            }

        }


        //systemt user id needed
        [HttpPost("ReverseTransaction/{Id}")]
        public async Task<IActionResult> Reversal(int Id, ReverseTransactionDTOW model)  
        {
            try
            {
                StringBuilder strbld2 = new StringBuilder();
                var err2 = new List<string>();

                if (!ModelState.IsValid)
                {
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            err2.Add(error.ErrorMessage);
                            err2.ForEach(err => { strbld2.AppendFormat("•{0}", error.ErrorMessage); });
                        }
                    }

                    return BadRequest(new { message = strbld2 });
                }
                //check if user exist 
                var userss = _systemuser.getSingleSystemUser(Id);
                if (userss == null)
                {
                    //bounce back if they dont exist 
                    return NotFound(new
                    {
                        succes = false,
                        message = "User Not Found"
                    });
                }
                //check if wallet exist
                //grabb wallet
                var walletAcct = _systemuser.getMyWallet(Id);
                if (walletAcct == null)
                {
                    //bounce back if they dont have a wallet yet 
                    return NotFound(new
                    {
                        succes = false,
                        message = "You dont have a Wallet Account, Pls create one"
                    });
                }

              
                //get transactions by ref 
                var getTransactHistory = _systemuser.GetTransactionHistories_ByRef(model.Reference);
                if(getTransactHistory == null)
                {
                    //bounce back if no history 
                    return Ok(new
                    {
                        success = false,
                        message = "Invalid reference, No history"
                    });
                }
                //create a foreach loop and check transaction type, 
                foreach(var history in getTransactHistory)
                {
                    //check transactionType
                    if(history.Txn_type == "Debit")
                    {
                        var reff1 = Guid.NewGuid().ToString(); //ref for this transaction
                        //grab balance details
                        var currentBalance = walletAcct.Balance;
                        var amountToCredit = history.Amount; 
                        var newbal = (currentBalance + amountToCredit);
                        string purposee = "Reversal";
                        //call Credit function
                        var creditResponse = await Credit(walletAcct, currentBalance, newbal, reff1, purposee, amountToCredit);
                        if (!creditResponse == true)
                        {
                            return Ok(new { success = false, message = "Something went wrong, pls try again later" });
                        }
                    }

                    if(history.Txn_type == "Credit")
                    {
                        //call debit function 
                        var reff2 = Guid.NewGuid().ToString(); //ref for this transaction

                        var walletId_Receipeint = history.wallet.ID;
                        //check if wallet exist
                        //grabb wallet
                        var walletAcct2 = _systemuser.getMyWallet_ByID(walletId_Receipeint);
                        if (walletAcct2 == null)
                        {
                            //bounce back if they dont have a wallet yet 
                            return NotFound(new
                            {
                                succes = false,
                                message = "Wallet Account doesnt exist"
                            });
                        }

                        //grab balance details
                        var currentBalance2 = walletAcct2.Balance;
                        var amountTodebit = history.Amount;
                        var newbal2 = (currentBalance2 - amountTodebit);
                        string purpose2 = "Reversal";
                        //call Debit function
                        var debitResponse = await Debit(walletAcct2, currentBalance2, newbal2, reff2, purpose2, amountTodebit);
                        if (!debitResponse == true)
                        {
                            return Ok(new { success = false, message = "Something went wrong, pls try again later" });
                        }
                    } 

                }
                //now savechanges
                await _systemuser.SaveChanges();
 
                //return response body
                return Ok(new
                {
                    success = true,
                    type = "Reversal",
                    message = "Reversal Succesful"
                });

            }
            catch (Exception ex)
            {
                return Ok(new { succes = false, messsage = ex.Message });
            }

        }

        //credit wallet 
        private async Task<bool> CreateAWallet(int userId, string walletName)
        {
            //get system user 
            var userss = _systemuser.getSingleSystemUser(userId);
            if (userss == null)
            {
                //var walletresponse = new CreateWalletDTO
                //{
                //    Success = false,
                //    Message = "User Not Found"
                //};
                return false;
            }
            var walletAcct = _systemuser.getMyWallet(userId);
            if (walletAcct != null)
            {
                //var walletresponse = new CreateWalletDTO
                //{
                //    Success = false,
                //    Message = "You have an already existing wallet Account"
                //};
                return false;
            }

            try
            {
                decimal bal = 0.0000m; // explicit cast to decimal with the prefix "m", float is "f"
                var newWallet = new WalletAccount
                {
                    Name = walletName,
                    user = userss,
                    Balance = bal,
                    Created_at = DateTime.Now
                };

                //create Wallet
                var Wallet = _systemuser.CreateWallet(newWallet);
                await _systemuser.SaveChanges();

                var walletresponse = new CreateWalletDTO
                {
                    Success = true,
                    Message = "wallet Created Sucesfully"
                };

               return true;
              
            }

            catch (Exception ex)
            {
                //var walletresponse = new CreateWalletDTO
                //{
                //    Success = false,
                //    Message = ex.Message
                //};
                return false;
            }
        }

        //credit wallet 
        private async Task<bool> Credit(WalletAccount walletAccount, decimal balbefore, decimal balAfter, string refff, string purpose, decimal Amountt)
        {
            // make changes to wallet
            walletAccount.Balance = balAfter;

            //create Transaction history 
            var newhistory = new TransactionHistory
            {
                Purpose = purpose,
                reference = refff,
                Txn_type = "Credit",
                wallet = walletAccount,
                balance_before = balbefore,
                balance_after = balAfter,
                created_at = DateTime.Now,
                updated_at = DateTime.Now,
                Amount = Amountt
            };

            try
            {
                var newHist = _systemuser.CreateTransactHist(newhistory);
                if (newHist == null)
                {
                    return false;
                }

                ////call save changes 
                //await _systemuser.SaveChanges();
            }
            catch (Exception ex)
            {
                return false;
            }

            //
            return true;
        }

        //Debit wallet 
        private async Task<bool> Debit(WalletAccount walletAccount, decimal balbefore, decimal balAfter, string refff, string purpose, decimal amountt)
        {
            // make changes to wallet
            walletAccount.Balance = balAfter;

            //create Transaction history 
            var newhistory = new TransactionHistory
            {
                Purpose = purpose,
                reference = refff,
                Txn_type = "Debit",
                wallet = walletAccount,
                balance_before = balbefore,
                balance_after = balAfter,
                created_at = DateTime.Now,
                updated_at = DateTime.Now,
                Amount = amountt
            };

            try
            {
                var newHist = _systemuser.CreateTransactHist(newhistory);
                if (newHist == null)
                {
                    return false;
                }

                ////call save changes 
                //await _systemuser.SaveChanges();
            }
            catch (Exception ex)
            {
                return false;
            }

            // 
            return true;
        }
    }
}