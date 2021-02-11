using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Wallet.Data.Entities;
using Wallet.Data.Interface;
using Wallet_API.WriteDTO;

namespace Wallet_API.Controllers
{
    //To:Do Convert all DB interactions to asynchronus operations Task<T>

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
            var userss = await _systemuser.getSingleSystemUser(userId);
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

            try
            {
                decimal bal = 0.0000m; // explicit cast to decimal with the prefix "m", float is "f"
                var newWallet = new WalletAccount
                {
                    Name = model.Name,
                    userID = userss.ID,
                    Balance = bal,
                    Created_at = DateTime.Now
                };
                //create Wallet
                var Wallet = _systemuser.CreateWallet(newWallet);
                await _systemuser.SaveChanges();

                if (Wallet != null)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Wallet created Succesfully",
                        WalletName = Wallet.Name,
                        Current_Balance = bal
                    });
                }

                return Ok(new
                {
                    success = false,
                    message = "Something went wrong we could not create your wallet Pls try again"
                });

            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    succes = false,
                    message = "Something went wrong we couldnot create your wallet Pls try again"
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
        public async Task<IActionResult> Update(int id, [FromBody]UpdateWalletModel model)
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
                //To:Do -- move this into a function for robustness

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

                // make changes to wallet
                walletAcct.Balance = newbalance;

                //create Transaction history 
                var newhistory = new TransactionHistory
                {
                    Purpose = "Deposit",
                    reference = refff,
                    Txn_type = "Credit",
                    walletID = walletAcct.ID,
                    balance_before = balanceBefore,
                    balance_after = newbalance,
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                };

                try
                {
                    var newHist = _systemuser.CreateTransactHist(newhistory);
                    if (newHist == null)
                    {

                        return Ok(new
                        {
                            succes = false,
                            message = "Somethhing went wrong pls try again later"
                        });
                    }

                    //call save changes 
                    await _systemuser.SaveChanges();


                }
                catch (Exception ex)
                {
                    return Ok(new
                    {
                        succes = false,
                        message = ex.Message
                    });
                }

                //return transaction summary to user --this way 
                return Ok(new
                {
                    success = true,
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
                //To:Do -- move credit and debit into a fucntion 
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
                if(amountToSend  > currentBalance)
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

                var balancebefore_Sender = walletAcct1.Balance;
                var newBalance = (balancebefore_Sender) - (bal);
                walletAcct1.Balance = newBalance;

                //Create TrasanctionHist for Sender
                //create Transaction history 
                var newhistory_Sender = new TransactionHistory
                {
                    Purpose = "Transfer",
                    reference = refff,
                    Txn_type = "Debit",
                    walletID = walletAcct1.ID,
                    balance_before = balancebefore_Sender,
                    balance_after = newBalance,
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                };

                try
                {

                    var newHist_Sender = _systemuser.CreateTransactHist(newhistory_Sender);
                }
                catch (Exception ex)
                {
                    return Ok(new { success = false, message = "Something went wrong pls try again later" });
                }

                //process transaction for Receiever 
                var balancebefore_Receiever = walletAcct2.Balance;
                var balanceAfter = (balancebefore_Receiever) + (bal);
                walletAcct2.Balance = balanceAfter;

                //Create TrasanctionHist for Receiver
                var newhistory_Receiever = new TransactionHistory
                {
                    Purpose = "Deposit",
                    reference = refff,
                    Txn_type = "Credit",
                    walletID = walletAcct2.ID,
                    balance_before = balancebefore_Receiever,
                    balance_after = balanceAfter,
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                };

                try
                {
                    var newHist_Receiever = _systemuser.CreateTransactHist(newhistory_Receiever);
                }
                catch (Exception ex)
                {
                    return Ok(new { success = false, message = "Something went wrong pls try again later" });
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
            catch(Exception ex)
            {
                return Ok(new { succes = false, messsage = ex.Message });
            }
          
        }
    }
}