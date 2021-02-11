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
        public async Task<IActionResult> TransferFund(int Id, FundWalletModel model)
        {

            return null;
        }
    }
}